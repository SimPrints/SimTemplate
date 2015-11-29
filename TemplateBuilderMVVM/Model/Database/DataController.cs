using log4net;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemplateBuilder.Helpers;

namespace TemplateBuilder.Model.Database
{
    public class DataController : IDataController
    {
        private static readonly ILog m_Log = LogManager.GetLogger(typeof(DataController));
        private static readonly string CONNECTION_STRING = "Data Source={0};Version=3;";

        private DataControllerConfig m_Config;
        private SimPrintsDb m_Database;
        IEnumerator<Capture> m_Query;
        private string[] m_CandidateFiles;

        #region Constructor

        public DataController()
        {
        }

        #endregion

        #region Public Methods

        public bool Connect(DataControllerConfig config)
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
            // Prepare a list of candidate image files for matching with captures.
            m_CandidateFiles = GetCandidateFiles(m_Config.ImageFilesDirectory);

            bool returnValue = false;
            if (m_CandidateFiles.Count() > 0 && m_Query != null)
            {
                returnValue = true;
            }
            return returnValue;
        }

        public string GetImageFile()
        {
            string confirmedFile = null;
            while (confirmedFile == null)
            {
                // First query the database to get an image file name.
                string filename = GetFilename();

                // Now check for image file maches with the filename.
                if (!String.IsNullOrEmpty(filename))
                {
                    IEnumerable<string> matches = from f in m_CandidateFiles
                                                  where f.IndexOf(filename) > -1
                                                  select f;

                    if (matches.Count() > 0)
                    {
                        // We have at least one match.
                        if (matches.Count() == 1)
                        {
                            // We have exactly one match.
                            if (File.Exists(matches.First()))
                            {
                                // The file still exists.
                                confirmedFile = filename;
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
                            m_Log.WarnFormat("{0} matches for query file name {1}. Skipping.",
                                matches.Count(), filename);
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

        #endregion

        #region Private Methods

        private IEnumerator<Capture> ConstructQuery()
        {
            IEnumerable<Capture> query = from c in m_Database.Captures
                                         where c.ScannerName == "LES"
                                         select c;
            return query.GetEnumerator();
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

        private static string[] GetCandidateFiles(string path)
        {

            if (Directory.Exists(path))
            {

            }
            return Directory.GetFiles(path, "*.png", SearchOption.AllDirectories);
        }

        #endregion
    }
}
