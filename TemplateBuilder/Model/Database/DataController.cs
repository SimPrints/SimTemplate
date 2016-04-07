using log4net;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using TemplateBuilder.Helpers;
using TemplateBuilder.ViewModel.MainWindow;

namespace TemplateBuilder.Model.Database
{
    public class DataController : IDataController
    {
        #region Constants

        private const int MAX_OPEN_FILE_ATTEMPTS = 1000;
        private const string CONNECTION_STRING = @"Data Source={0};Version=3;";
        private const string CAPTURE_QUERY_STRING = @"SELECT * FROM Capture WHERE Capture.SimAfisTemplate IS {0} ORDER BY RANDOM() LIMIT 1;";
        private const string CAPTURE_GIVEN_SCANNER_QUERY_STRING = @"SELECT * FROM Capture WHERE Capture.SimAfisTemplate IS {0} AND ScannerName = '{1}' ORDER BY RANDOM() LIMIT 1;";

        #endregion

        private static readonly ILog m_Log = LogManager.GetLogger(typeof(DataController));

        private DataControllerConfig m_Config;
        private SimPrintsDb m_Database;
        private InitialisationResult result;
        private IEnumerable<string> m_ImageFiles;
        private IDictionary<Guid, CancellationTokenSource> m_TokenSourceLookup;

        private event EventHandler<InitialisationCompleteEventArgs> m_InitialisationComplete;
        private event EventHandler<GetCaptureCompleteEventArgs> m_GetCaptureComplete;

        #region Constructor

        public DataController()
        {
            result = InitialisationResult.Uninitialised;
            m_TokenSourceLookup = new Dictionary<Guid, CancellationTokenSource>();
        }

        #endregion

        #region IDataController

        /// <summary>
        /// Connects the controller to the SQLite database using the provided configuration.
        /// </summary>
        /// <param name="config">The configuration.</param>
        /// <param name="progress"></param>
        void IDataController.BeginInitialise(DataControllerConfig config)
        {
            m_Log.Debug("BeginInitialise(...) called.");
            IntegrityCheck.IsNotNull(config, "config");
            IntegrityCheck.IsNotNullOrEmpty(config.DatabasePath, "config.DatabasePath");
            IntegrityCheck.IsNotNullOrEmpty(config.ImageFilesDirectory, "config.ImageFilesDirectory");

            m_Config = config;

            // Connect to SQlite.
            SQLiteConnection dbConnection = new SQLiteConnection(
                String.Format(CONNECTION_STRING, m_Config.DatabasePath));

            // Set the LINQ data context to the database connection.
            m_Database = new SimPrintsDb(dbConnection);

            // Obtain image files on local machine, to be matched with database entries.
            m_ImageFiles = GetImageFiles(m_Config.ImageFilesDirectory);
            if (m_ImageFiles != null &&
                m_ImageFiles.Count() > 0)
            {
                result = InitialisationResult.Initialised;
            }
            else
            {
                m_Log.Error("Failed to get image files.");
                result = InitialisationResult.Error;
            }

            OnInitialisationComplete(
                new InitialisationCompleteEventArgs(result));
        }

        /// <summary>
        /// Gets the next image file to process by iterating the results of the SQL query and
        /// searching for it on the local machine.
        /// </summary>
        /// <returns>
        /// filepath of the local file, null if no image can be found.
        /// </returns>
        Guid IDataController.BeginGetCapture(ScannerType scannerType, bool isTemplated)
        {
            m_Log.DebugFormat("BeginGetCapture(scannterType={0}, isTemplated={1}) called",
                scannerType, isTemplated);
            IntegrityCheck.IsNotNull(scannerType);

            // Generate a request identifier, and cancellationTokenSource for the request
            Guid guid = Guid.NewGuid();
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

            // Create a cancellation token for notifying of a cancellation request.
            CancellationToken token = cancellationTokenSource.Token;

            // Define and run the task, passing in the token.
            Task getCaptureTask = Task.Run(() =>
            {
                m_Log.Debug("Get capture task running.");
                // Get a capture
                GetCaptureCompleteEventArgs args = GetCapture(isTemplated, scannerType, guid, token);
                // Raise GetCaptureComplete event.
                OnGetCaptureComplete(args);
            }, token);

            // Raise the GetCaptureComplete event in the case where the Task faults.
            getCaptureTask.ContinueWith((Task t) =>
            {
                if (t.IsFaulted)
                {
                    OnGetCaptureComplete(new GetCaptureCompleteEventArgs(null, guid, DataRequestResult.TaskFailed));
                }
            });

            // Store the token source and task under guid key so the token source may be retrieved later.
            m_TokenSourceLookup.Add(guid, cancellationTokenSource);

            // Return the task's unique identifier.
            return guid;
        }

        void IDataController.AbortCaptureRequest(Guid guid)
        {
            m_Log.DebugFormat("CancelCaptureRequest(guid={0}) called.", guid);
            IntegrityCheck.IsNotNull(guid);

            // Attempt to lookup the token.
            CancellationTokenSource tokenSource;
            bool isSuccessful = m_TokenSourceLookup.TryGetValue(guid, out tokenSource);

            if (isSuccessful)
            {
                // Request cancellation.
                IntegrityCheck.IsNotNull(tokenSource);
                tokenSource.Cancel();
                m_TokenSourceLookup.Remove(guid);
            }
            else
            {
                // TODO: What to do if Guid doesn't correspond to a current request?
                m_Log.WarnFormat("Cancellation of request (guid={0}) failed, token no longer exists.",
                    guid);
            }
        }

        /// <summary>
        /// Saves the template.
        /// </summary>
        /// <param name="template">The template.</param>
        /// <returns></returns>
        bool IDataController.SaveTemplate(string guid, long dbId, byte[] template)
        {
            m_Log.DebugFormat("SaveTemplate(guid={0}, dbId={1}, template={3}) called.",
                guid, dbId, template);
            IntegrityCheck.IsNotNullOrEmpty(guid);
            // The template is allowed to be null!
            //IntegrityCheck.IsNotNull(template);

            CaptureDb capture = (from c in m_Database.Captures
                                 where c.Id == dbId
                                 select c).FirstOrDefault();

            bool isSuccessful = false;
            if (capture != null)
            {
                if (capture.Guid == guid)
                {
                    // Update the template to that supplied
                    capture.GoldTemplate = template;
                    isSuccessful = true;
                }
                else
                {
                    m_Log.WarnFormat(
                        "GUID of capture with DbId={0} doesn't match GUID supplied ({0} instead of {1})",
                        capture.Id, capture.Guid, guid);
                }
            }
            else
            {
                m_Log.WarnFormat("Failed to find capture wtih DbId={0}. Not saving template", dbId);
            }

            m_Database.SubmitChanges();

            return isSuccessful;
        }

        event EventHandler<InitialisationCompleteEventArgs> IDataController.InitialisationComplete
        {
            add { m_InitialisationComplete += value; }
            remove { m_InitialisationComplete -= value; }
        }

        event EventHandler<GetCaptureCompleteEventArgs> IDataController.GetCaptureComplete
        {
            add { m_GetCaptureComplete += value; }
            remove { m_GetCaptureComplete -= value; }
        }

        #endregion

        #region Private Methods

        private GetCaptureCompleteEventArgs GetCapture(bool isTemplated, ScannerType scannerType, Guid guid, CancellationToken token)
        {
            m_Log.DebugFormat("GetCapture(isTemplated={0}, scannerType={1}, guid={2}, token={3}) called",
                isTemplated, scannerType, guid, token);
            CaptureInfo captureInfo = null;
            DataRequestResult result = DataRequestResult.None;
            bool isRunning = true;
            int attempts = 0;
            while (isRunning)
            {
                // Check if cancellation requested
                token.ThrowIfCancellationRequested();

                // First query the database to get an image file name.
                CaptureDb captureCandidate = GetCaptureFromDatabase(isTemplated, scannerType);

                if (captureCandidate != null)
                {
                    // Try to find an image file using the file name.
                    byte[] imageData;
                    bool isFound = TryGetImageFromName(captureCandidate.Guid, out imageData);
                    if (isFound)
                    {
                        // Matching file found.
                        m_Log.DebugFormat("Matching file found for capture={0}", captureCandidate.Guid);
                        isRunning = false;
                        captureInfo = new CaptureInfo(
                            captureCandidate.Guid,
                            captureCandidate.Id,
                            imageData,
                            captureCandidate.GoldTemplate);
                        result = DataRequestResult.Success;
                    }
                    else
                    {
                        // Give up if the number of attemps exceeds limit.
                        attempts++;
                        if (attempts > MAX_OPEN_FILE_ATTEMPTS)
                        {
                            m_Log.WarnFormat("Exceeded maximum number of file searches (attempts={0})",
                                attempts);
                            isRunning = false;
                            result = DataRequestResult.Failed;
                        }
                    }
                }
                else
                {
                    // Queries are not returning any more candidates, give up immediately.
                    m_Log.Warn("No candidate filename obtained from the database");
                    result = DataRequestResult.Failed;
                    break;
                }
            }
            IntegrityCheck.AreNotEqual(DataRequestResult.None, result);
            return new GetCaptureCompleteEventArgs(captureInfo, guid, result);
        }

        private static IEnumerable<string> GetImageFiles(string directory)
        {
            ConcurrentBag<string> threadSafeImages = null;
            if (Directory.Exists(directory))
            {
                // The provided image path exists, so fetch images in that directory.
                IEnumerable<string> imageFiles = Directory.GetFiles(
                    directory,
                    "*.png",
                    SearchOption.AllDirectories);
                // Save the image files in a thread-safe list.
                threadSafeImages = new ConcurrentBag<string>(imageFiles);
            }
            else
            {
                m_Log.ErrorFormat(
                    "Supplied directory for image files does not exist ({0})",
                    directory);
            }
            return threadSafeImages;
        }

        private CaptureDb GetCaptureFromDatabase(bool isTemplated, ScannerType scannerType)
        {
            CaptureDb capture;
            string withTemplateString = isTemplated ? "NOT NULL" : "NULL";
            string query;
            if (scannerType == ScannerType.None)
            {
                query = String.Format(CAPTURE_QUERY_STRING, withTemplateString);
            }
            else
            {
                query = String.Format(CAPTURE_GIVEN_SCANNER_QUERY_STRING, withTemplateString, scannerType);
            }
            capture = m_Database.ExecuteQuery<CaptureDb>(query).FirstOrDefault();
            return capture;
        }

        private bool TryGetImageFromName(string name, out byte[] imageData)
        {
            // Try to find an image file using the filename obtained from the database.
            string filepath;
            bool isFound = TryGetPathFromName(name, out filepath);

            bool isSuccessful = false;
            imageData = null;
            if (isFound)
            {
                if (File.Exists(filepath))
                {
                    // The file exists.
                    m_Log.DebugFormat("An image file was found for image: {0}.", filepath);
                    try
                    {
                        imageData = File.ReadAllBytes(filepath);
                        isSuccessful = true;
                    }
                    catch (NotSupportedException ex)
                    {
                        m_Log.WarnFormat("Failed to read image file: {0}", ex);
                    }
                }
                else
                {
                    m_Log.WarnFormat(
                        "File {0} found in candidate files on local machine but file no longer exists.",
                        name);
                }
            }
            else
            {
                m_Log.WarnFormat("Found no image file containing {0}", name);
            }
            return isSuccessful;
        }

        private bool TryGetPathFromName(string name, out string filepath)
        {
            filepath = m_ImageFiles.FirstOrDefault(x => x.Contains(name));
            return !String.IsNullOrEmpty(filepath);
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

        private void OnGetCaptureComplete(GetCaptureCompleteEventArgs e)
        {
            EventHandler<GetCaptureCompleteEventArgs> temp = m_GetCaptureComplete;
            if (temp != null)
            {
                temp.Invoke(this, e);
            }
        }

        #endregion
    }
}
