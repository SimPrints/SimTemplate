using log4net;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemplateBuilder.Helpers;
using TemplateBuilder.ViewModel.MainWindow;

namespace TemplateBuilder.Model.Database
{
    public class DataController : IDataController
    {
        #region Constants

        private const int MAX_OPEN_FILE_ATTEMPTS = 1000;
        private const string CONNECTION_STRING = "Data Source={0};Version=3;";

        #endregion

        private enum State { None, Uninialised, Initialised, Indexed, Error };

        private static readonly ILog m_Log = LogManager.GetLogger(typeof(DataController));

        private DataControllerConfig m_Config;
        private SimPrintsDb m_Database;
        private State m_State;
        private IEnumerator<Capture> m_Query;
        private IEnumerable<string> m_ImageFiles;
        private IDictionary<string, string> m_IndexedImageFiles;

        private event EventHandler<InitialisationCompleteEventArgs> m_InitialisationComplete;

        #region Constructor

        public DataController()
        {
            m_State = State.Uninialised;
            m_IndexedImageFiles = new ConcurrentDictionary<string, string>();
        }

        #endregion

        #region IDataController

        /// <summary>
        /// Connects the controller to the SQLite database using the provided configuration.
        /// </summary>
        /// <param name="config">The configuration.</param>
        /// <param name="progress"></param>
        void IDataController.Initialise(DataControllerConfig config)
        {
            IntegrityCheck.IsNotNull(config, "config");
            IntegrityCheck.IsNotNullOrEmpty(config.DatabasePath, "config.DatabasePath");
            IntegrityCheck.IsNotNullOrEmpty(config.ImageFilesDirectory, "config.ImageFilesDirectory");

            m_Config = config;

            // Connect to SQlite.
            SQLiteConnection dbConnection = new SQLiteConnection(
                String.Format(CONNECTION_STRING, m_Config.DatabasePath));

            // Set the LINQ data context to the database connection.
            m_Database = new SimPrintsDb(dbConnection);
            // Construct the query used to obtain a capture lacking a template.
            m_Query = ConstructQuery();

            if (m_Query != null)
            {
                // We connected to SQLite
                m_ImageFiles = GetImageFiles();
                if (m_ImageFiles != null &&
                    m_ImageFiles.Count() > 0)
                {
                    m_State = State.Initialised;
                }
            }
            else
            {
                m_State = State.Error;
                m_Log.Error("Failed to construct SQLite database query.");
            }

            // Start indexing the images for quick lookup.
            // Execute this lengthy process on another thread and yield UI thread.
            Task.Run(() =>
                {
                    GetIndexedFiles(m_Config.ImageFilesDirectory);
                    // Upon completing indexing process...
                    if (m_State == State.Initialised)
                    {
                        // If we successfully connected and obtained files before...
                        if (m_IndexedImageFiles != null &&
                        m_IndexedImageFiles.Count() > 0)
                        {
                            // If indexing was successful, update state to reflect this.
                            m_State = State.Indexed;
                        }
                        else
                        {
                            m_Log.Warn("Failed to index image files.");
                        }
                    }
                });

            OnInitialisationComplete(
                new InitialisationCompleteEventArgs(m_State == State.Initialised));

            // Notify subscribers whether we successfully initialised.
            //OnInitialisationComplete(new InitialisationCompleteEventArgs(m_State == State.Initialised));
            //OnInitialisationComplete(
            //    new InitialisationCompleteEventArgs(m_State == State.Initialised || m_State == State.Indexed));
        }

        /// <summary>
        /// Gets the next image file to process by iterating the results of the SQL query and
        /// searching for it on the local machine.
        /// </summary>
        /// <returns>
        /// filepath of the local file, null if no image can be found.
        /// </returns>
        string IDataController.GetImageFile()
        {
            string confirmedFile = null;
            bool isRunning = true;
            int attempts = 0;
            while (isRunning)
            {
                // Give up if the number of attemps exceeds limit.
                attempts++;
                if (attempts > MAX_OPEN_FILE_ATTEMPTS)
                {
                    isRunning = false;
                }

                // First query the database to get an image file name.
                string candidateName = GetNameFromDatabase();

                if (!String.IsNullOrEmpty(candidateName))
                {
                    // Try to find an image file using the filename obtained from the database.
                    string filepath;
                    bool isFound = TryGetPathFromName(candidateName, out filepath);

                    if (isFound)
                    {
                        if (File.Exists(filepath))
                        {
                            // The file still exists.
                            confirmedFile = filepath;
                            isRunning = false;
                        }
                        else
                        {
                            m_Log.WarnFormat(
                                "File {0} found in candidate files on local machine but file no longer exists.",
                                candidateName);
                        }
                    }
                    else
                    {
                        m_Log.WarnFormat("Found no image file containing {0}", candidateName);
                    }
                }
                else
                {
                    // Queries are not returning any more candidates, give up immediately.
                    m_Log.Warn("No candidate filename obtained from the database");
                    break;
                }
            }
            return confirmedFile;
        }

        /// <summary>
        /// Saves the template.
        /// </summary>
        /// <param name="template">The template.</param>
        /// <returns></returns>
        bool IDataController.SaveTemplate(byte[] template)
        {
            bool isSuccessful = false;

            m_Query.Current.GoldTemplate = template;

            m_Database.SubmitChanges();

            return isSuccessful;
        }

        event EventHandler<InitialisationCompleteEventArgs> IDataController.InitialisationComplete
        {
            add { m_InitialisationComplete += value; }
            remove { m_InitialisationComplete -= value; }
        }

        #endregion

        #region Private Methods

        private IEnumerator<Capture> ConstructQuery()
        {
            IEnumerable<Capture> query = from c in m_Database.Captures
                                             //where c.ScannerName == "LES"
                                         where c.GoldTemplate == null
                                         select c;

            IEnumerator<Capture> queryEnumerator = null;
            try
            {
                return queryEnumerator = query.GetEnumerator();
            }
            catch (SQLiteException ex)
            {
                m_Log.WarnFormat("Failed to query SQLite database: {0}", ex);
            }
            return queryEnumerator;
        }

        private IEnumerable<string> GetImageFiles()
        {
            ConcurrentBag<string> threadSafeImages = null;
            if (Directory.Exists(m_Config.ImageFilesDirectory))
            {
                // The provided image path exists, so fetch images in that directory.
                IEnumerable<string> imageFiles = Directory.GetFiles(
                    m_Config.ImageFilesDirectory,
                    "*.png",
                    SearchOption.AllDirectories);
                // Save the image files in a thread-safe list.
                threadSafeImages = new ConcurrentBag<string>(imageFiles);
            }
            else
            {
                m_State = State.Error;
                m_Log.ErrorFormat(
                    "Supplied directory for image files does not exist ({0})",
                    m_Config.ImageFilesDirectory);
            }
            return threadSafeImages;
        }

        private string GetNameFromDatabase()
        {
            string filename = String.Empty;
            if (m_Query.MoveNext())
            {
                filename = m_Query.Current.ImageFileName;
            }
            else
            {
                m_Log.Warn("No captures available in the enumerator.");
            }
            return filename;
        }

        private bool TryGetPathFromName(string name, out string filepath)
        {
            filepath = String.Empty;
            bool isFound = false;
            switch (m_State)
            {
                case State.Indexed:
                    isFound = m_IndexedImageFiles.TryGetValue(name, out filepath);
                    break;

                case State.Initialised:
                    // Dictionary is still being indexed, so first check if name entry exists.
                    isFound = m_IndexedImageFiles.TryGetValue(name, out filepath);
                    // If the name entry doesn't exist, search for it in the list itself.
                    if (!isFound)
                    {
                        filepath = m_ImageFiles.FirstOrDefault(x => x.Contains(name));
                        isFound = !String.IsNullOrEmpty(filepath);
                    }
                    break;

                default:
                    throw IntegrityCheck.FailUnexpectedDefault(m_State);
            }
            return isFound;
        }

        private void GetIndexedFiles(string path)
        {
            foreach (string filename in m_ImageFiles)
            {
                string key = Path.GetFileNameWithoutExtension(filename);
                if (!m_IndexedImageFiles.ContainsKey(key))
                {
                    m_IndexedImageFiles.Add(key, filename);
                }
                else
                {
                    m_Log.WarnFormat("Duplicate file found. Ignoring duplicate {0}", key);
                }
            }
        }

        #endregion

        #region Event Helpers

        private void OnInitialisationComplete(InitialisationCompleteEventArgs e)
        {
            EventHandler<InitialisationCompleteEventArgs> temp = m_InitialisationComplete;
            if (temp != null)
            {
                temp.Invoke(this, e);
            }
        }

        #endregion
    }
}
