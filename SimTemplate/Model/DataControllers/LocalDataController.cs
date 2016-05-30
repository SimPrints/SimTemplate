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
using SimTemplate.Helpers;
using SimTemplate.ViewModel.MainWindow;
using SimTemplate.Model.DataControllers.EventArguments;
using SimTemplate.Model.Database;

namespace SimTemplate.Model.DataControllers
{
    public class LocalDataController : DataController
    {
        #region Constants

        private const string SQLITE_DATABASE = @"C:\Users\Tristram\Documents\Field Data\Zambia + BDesh\ZambiaAndBangladesh_wFMR.sqlite";
        private const string IMAGES_FILE_PATH = @"C:\Users\Tristram\Documents\Field Data\Zambia + BDesh\Bangladesh AND Zambia mixed 2015";

        private const int MAX_OPEN_FILE_ATTEMPTS = 1000;
        private const string CONNECTION_STRING = @"Data Source={0};Version=3;";
        private const string CAPTURE_QUERY_STRING = @"SELECT * FROM Capture WHERE Capture.SimAfisTemplate IS {0} ORDER BY RANDOM() LIMIT 1;";
        private const string CAPTURE_GIVEN_SCANNER_QUERY_STRING = @"SELECT * FROM Capture WHERE Capture.SimAfisTemplate IS {0} AND ScannerName = '{1}' ORDER BY RANDOM() LIMIT 1;";

        #endregion

        private static readonly ILog m_Log = LogManager.GetLogger(typeof(LocalDataController));

        private SimPrintsDb m_Database;
        private InitialisationResult m_State;
        private IEnumerable<string> m_ImageFiles;

        #region Constructor

        public LocalDataController() : base()
        {
            m_State = InitialisationResult.Uninitialised;
        }

        #endregion

        #region IDataController

        /// <summary>
        /// Connects the controller to the SQLite database using the provided configuration.
        /// </summary>
        /// <param name="config">The configuration.</param>
        /// <param name="progress"></param>
        public override void BeginInitialise(DataControllerConfig config)
        {
            base.BeginInitialise(config);

            // Connect to SQlite.
            SQLiteConnection dbConnection = new SQLiteConnection(
                String.Format(CONNECTION_STRING, SQLITE_DATABASE));

            // Set the LINQ data context to the database connection.
            m_Database = new SimPrintsDb(dbConnection);

            // Obtain image files on local machine, to be matched with database entries.
            m_ImageFiles = GetImageFiles(IMAGES_FILE_PATH);
            if (m_ImageFiles != null &&
                m_ImageFiles.Count() > 0)
            {
                m_State = InitialisationResult.Initialised;
            }
            else
            {
                m_Log.Error("Failed to get image files.");
                m_State = InitialisationResult.Error;
            }

            OnInitialisationComplete(
                new InitialisationCompleteEventArgs(m_State));
        }

        #endregion

        #region Private Methods

        protected override void StartCaptureTask(ScannerType scannerType, Guid guid, CancellationToken token)
        {
            // Define and run the task, passing in the token.
            Task getCaptureTask = Task.Run(() =>
            {
                Log.Debug("Get capture task running.");
                // Get a capture
                CaptureInfo capture = GetCapture(scannerType, token);
                // Raise GetCaptureComplete event.
                OnGetCaptureComplete(new GetCaptureCompleteEventArgs(capture, guid, DataRequestResult.Success));
            }, token);

            // Raise the GetCaptureComplete event in the case where the Task faults.
            getCaptureTask.ContinueWith((Task t) =>
            {
                if (t.IsFaulted)
                {
                    Log.Error("Failed to save template: " + t.Exception.Message, t.Exception);
                    OnGetCaptureComplete(new GetCaptureCompleteEventArgs(null, guid, DataRequestResult.TaskFailed));
                }
            });
        }

        protected override void StartSaveTask(long dbId, byte[] template, Guid guid, CancellationToken token)
        {
            Task saveTask = Task.Run(() =>
            {
                m_Log.Debug("Save task running.");

                CaptureDb capture = (from c in m_Database.Captures
                                     where c.Id == dbId
                                     select c).FirstOrDefault();

                if (capture != null)
                {
                    // Update the template to that supplied
                    capture.GoldTemplate = template;
                    m_Database.SubmitChanges();
                }
                else
                {
                    m_Log.WarnFormat("Failed to find capture wtih DbId={0}. Not saving template", dbId);
                }
            }, token);

            // Raise the SaveTemplateComplete event in the case where the Task faults.
            saveTask.ContinueWith((Task t) =>
            {
                if (t.IsFaulted)
                {
                    m_Log.Error("Failed to save template: " + t.Exception.Message, t.Exception);
                    OnSaveTemplateComplete(new SaveTemplateEventArgs(guid, DataRequestResult.TaskFailed));
                }
            });
        }

        private CaptureInfo GetCapture(ScannerType scannerType, CancellationToken token)
        {
            m_Log.DebugFormat("GetCapture(scannerType={1}, token={2}) called",
                scannerType, token);
            CaptureInfo captureInfo = null;
            DataRequestResult result = DataRequestResult.None;
            bool isRunning = true;
            int attempts = 0;
            while (isRunning)
            {
                // Check if cancellation requested
                token.ThrowIfCancellationRequested();

                // First query the database to get an image file name.
                CaptureDb captureCandidate = GetCaptureFromDatabase(scannerType);

                if (captureCandidate != null)
                {
                    // Try to find an image file using the file name.
                    byte[] imageData;
                    bool isFound = TryGetImageFromName(captureCandidate.HumanId, out imageData);
                    if (isFound)
                    {
                        // Matching file found.
                        m_Log.DebugFormat("Matching file found for capture={0}", captureCandidate.HumanId);
                        isRunning = false;
                        captureInfo = new CaptureInfo(
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
            return captureInfo;
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

        private CaptureDb GetCaptureFromDatabase(ScannerType scannerType)
        {
            CaptureDb capture;
            string withTemplateString = "NULL";
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
    }
}
