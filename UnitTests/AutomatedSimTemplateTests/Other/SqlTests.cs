// Copyright 2016 Sam Briggs
//
// This file is part of SimTemplate.
//
// SimTemplate is free software: you can redistribute it and/or modify it under the
// terms of the GNU General Public License as published by the Free Software 
// Foundation, either version 3 of the License, or (at your option) any later
// version.
//
// SimTemplate is distributed in the hope that it will be useful, but WITHOUT ANY 
// WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR 
// A PARTICULAR PURPOSE. See the GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License along with
// SimTemplate. If not, see http://www.gnu.org/licenses/.
//
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
