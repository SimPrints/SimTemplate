using log4net;
using System;
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

        private const int MAX_OPEN_FILE_ATTEMPTS = 100000;

        #endregion

        private static readonly ILog m_Log = LogManager.GetLogger(typeof(DataController));
        private static readonly string CONNECTION_STRING = "Data Source={0};Version=3;";

        private DataControllerConfig m_Config;
        private SimPrintsDb m_Database;
        IEnumerator<Capture> m_Query;
        private IDictionary<string, string> m_CandidateFiles;

        public event EventHandler<InitialisationCompleteEventArgs> m_InitialisationComplete;

        #region Constructor

        public DataController()
        {
        }

        #endregion

        #region IDataController

        async Task IDataController.Initialise(DataControllerConfig config, IProgress<int> progress)
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

            bool returnValue = false;
            if (m_Query != null)
            {
                // Successfully connected to SQLite database
                // Prepare a list of candidate image files for matching with captures.
                // Allow thread to yield at this point as it is a slow operation (await).
                m_CandidateFiles = await Task.Run(() =>
                    GetCandidateFiles(m_Config.ImageFilesDirectory, progress));

                if (m_CandidateFiles != null &&
                    m_CandidateFiles.Count() > 0 &&
                    m_Query != null)
                {
                    returnValue = true;
                }
            }
            OnInitialisationComplete(new InitialisationCompleteEventArgs(returnValue));
        }

        string IDataController.GetImageFile()
        {
            string confirmedFile = null;
            bool isRunning = true;
            int attempts = 0;
            while (isRunning)
            {
                attempts++;
                if (attempts > MAX_OPEN_FILE_ATTEMPTS)
                {
                    isRunning = false;
                }

                // First query the database to get an image file name.
                string filename = GetFilename();

                // Now check for image file maches with the filename.
                if (!String.IsNullOrEmpty(filename))
                {
                    string match;
                    bool isFound = m_CandidateFiles.TryGetValue(filename, out match);

                    if (isFound)
                    {
                        // We have exactly one match.
                        if (File.Exists(match))
                        {
                            // The file still exists.
                            confirmedFile = match;
                            isRunning = false;
                        }
                        else
                        {
                            m_Log.WarnFormat(
                                "File {0} found in candidate files on local machine but file no longer exists.",
                                filename);
                        }
                    }
                    else
                    {
                        m_Log.WarnFormat("Found no image file containing {0}", filename);
                    }
                }
                else
                {
                    // No more candidate file names can be obtained from the database.
                    m_Log.Warn("No candidate filename obtained from the database");
                    break;
                }
            }
            return confirmedFile;
        }

        bool IDataController.SaveTemplate(byte[] template)
        {
            bool isSuccessful = false;

            m_Query.Current.GoldTemplate = template;

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

        private string GetFilename()
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

        #endregion

        #region Static Helper Methods

        private IDictionary<string, string> GetCandidateFiles(string path, IProgress<int> progress)
        {
            string[] candidateFiles = null;
            IDictionary<string, string> candidateFileLookup = new Dictionary<string, string>();
            if (Directory.Exists(path))
            {
                candidateFiles = Directory.GetFiles(path, "*.png", SearchOption.AllDirectories);

                int delta = (int)(candidateFiles.Count() / 20);
                for (int i = 0; i < candidateFiles.Count(); i++)
                {
                    string filename = candidateFiles[i];
                    string key = Path.GetFileNameWithoutExtension(filename);
                    if (!candidateFileLookup.ContainsKey(key))
                    {
                        candidateFileLookup.Add(key, filename);
                    }
                    else
                    {
                        m_Log.WarnFormat("Duplicate file found. Ignoring duplicate {0}", key);
                    }
                    if (i % delta == 0)
                    {
                        progress.Report((int)(i * 100 / candidateFiles.Count()));
                    }
                }
            }
            else
            {
                m_Log.WarnFormat("Provided image directory doesn't exist: {0}", path);
            }
            return candidateFileLookup;
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
