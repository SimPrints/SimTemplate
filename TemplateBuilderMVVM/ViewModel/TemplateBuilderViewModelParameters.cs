namespace TemplateBuilder.ViewModel
{
    public class TemplateBuilderViewModelParameters
    {
        private readonly string m_SqliteDatabase;
        private readonly string m_IdCol;
        private readonly string m_ScannerNameCol;
        private readonly string m_FingerNumberCol;
        private readonly string m_CaptureNumberCol;
        private readonly string m_GoldTemplateCol;

        /// <summary>
        /// Gets the full path of the sqlite database.
        /// </summary>
        public string SqliteDatabase { get { return m_SqliteDatabase; } }

        /// <summary>
        /// Gets the name of the ID column.
        /// </summary>
        public string IdCol { get { return m_IdCol; } }

        /// <summary>
        /// Gets the name of the Scanner Name column.
        /// </summary>
        public string ScannerNameCol { get { return m_ScannerNameCol; } }

        /// <summary>
        /// Gets the name of the Finger Number column.
        /// </summary>
        public string FingerNumberCol { get { return  m_FingerNumberCol; } }

        /// <summary>
        /// Gets the name of the Capture Number column.
        /// </summary>
        public string CaptureNumberCol { get { return m_CaptureNumberCol; } }

        /// <summary>
        /// Gets the name of the Gold Template column.
        /// </summary>
        public string GoldTemplateCol { get { return m_GoldTemplateCol; } }


        public TemplateBuilderViewModelParameters(
            string sqliteDatabase,
            string idCol,
            string scannerNameCol,
            string fingerNumberCol,
            string captureNumberCol,
            string goldTemplateCol)
        {
            m_SqliteDatabase = sqliteDatabase;
            m_IdCol = idCol;
            m_ScannerNameCol = scannerNameCol;
            m_FingerNumberCol = fingerNumberCol;
            m_CaptureNumberCol = captureNumberCol;
            m_GoldTemplateCol = goldTemplateCol;
        }
    }
}