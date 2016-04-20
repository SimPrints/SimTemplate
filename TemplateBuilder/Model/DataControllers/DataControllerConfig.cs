using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimTemplate.Model.DataControllers
{
    public class DataControllerConfig
    {
        private readonly string m_DatabasePath;
        private readonly string m_ImageFilesDirectory;

        public string DatabasePath { get { return m_DatabasePath; } }
        public string ImageFilesDirectory { get { return m_ImageFilesDirectory; } }

        public DataControllerConfig(string databasePath, string imageFilesDirectory)
        {
            m_DatabasePath = databasePath;
            m_ImageFilesDirectory = imageFilesDirectory;
        }
    }
}
