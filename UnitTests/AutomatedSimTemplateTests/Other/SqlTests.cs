using log4net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Data.Linq;
using System.Data.SQLite;
using System.Linq;
using SimTemplate.Model.Database;

namespace AutomatedSimTemplateTests.OTher
{
    [TestClass]
    public class SqlTests
    {
        private static readonly ILog m_Log = LogManager.GetLogger(typeof(SqlTests));
        private const string DATABASE_PATH = @"C:\SimPrints\Data\mainDb_yesFMR_noPNG.sqlite";

        [TestMethod]
        public void TestConnectToDataContext()
        {
            // Connect to SQlite.
            SQLiteConnection dbConnection = new SQLiteConnection(
                String.Format("Data Source={0};Version=3;",
                DATABASE_PATH));

            // Set the LINQ data context to the database connection.
            DataContext db = new DataContext(dbConnection);

            // Make a basic query
            Table<CaptureDb> captures = db.GetTable<CaptureDb>();
            // Query for customers from London
            var q = from c in captures
                     where c.ScannerName == "LES"
                     select c;

            foreach (CaptureDb capt in q)
                Console.WriteLine("id = {0}, ScannerName = {1}, FingerNumber = {2}",
                    capt.Id,
                    capt.ScannerName,
                    capt.FingerNumber);
        }

        [TestMethod]
        public void TestConnectToDatabase()
        {
            // Connect to SQlite.
            SQLiteConnection dbConnection = new SQLiteConnection(
                String.Format("Data Source={0};Version=3;",
                DATABASE_PATH));

            // Set the LINQ data context to the database connection.
            SimPrintsDb db = new SimPrintsDb(dbConnection);

            // Make a basic query
            // Query for customers from London
            var q = from c in db.Captures
                    where c.ScannerName == "LES"
                    select c;

            foreach (CaptureDb capt in q)
            {
                Console.WriteLine("id = {0}, ScannerName = {1}, FingerNumber = {2}, Pid = {3}",
                    capt.Id,
                    capt.ScannerName,
                    capt.FingerNumber,
                    capt.Person.Pid);
            }
        }
    }
}
