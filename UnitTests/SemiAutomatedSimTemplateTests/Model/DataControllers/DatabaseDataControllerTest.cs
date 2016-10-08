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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SimTemplate.Model.DataControllers.EventArguments;
using SimTemplate.Model.DataControllers;
using SimTemplate.DataTypes.Enums;
using SimTemplate.Model.DataControllers.Local;

namespace AutomatedSimTemplateTests.Model
{
    [TestClass]
    public class DatabaseDataControllerTest
    {
        #region Constants

        private const string DATABASE_PATH = @"C:\SimPrints\Data\mainDb_yesFMR_noPNG.sqlite";
        private const string IMAGE_FILES_DIRECTORY = @"C:\SimPrints\Images";
        private const string NO_MATCHING_IMAGES_DIRECTORY = @"C:\SimPrints\NoImages";

        private static TimeSpan INITIALISATION_TIMEOUT = TimeSpan.FromSeconds(1);
        private static TimeSpan GET_CAPTURE_TIMEOUT = TimeSpan.FromSeconds(1);

        #endregion

        IDataController m_DataController;
        InitialisationCompleteEventArgs m_InitCompleteArgs;
        GetCaptureCompleteEventArgs m_GetCaptureArgs;
        AutoResetEvent m_InitialisationCompleteResetEvent;
        AutoResetEvent m_GetCaptureRequestCompleteResetEvent;

        [TestInitialize]
        public void TestSetup()
        {
            m_DataController = new LocalDataController();
            m_InitCompleteArgs = null;
            m_GetCaptureArgs = null;
            m_DataController.InitialisationComplete += DataController_InitialisationComplete;
        }

        [TestMethod]
        public void TestInitialise_Success()
        {
            // Call BeginInitialise
            DataControllerConfig config = new DataControllerConfig(
                DATABASE_PATH,
                IMAGE_FILES_DIRECTORY);
            m_DataController.BeginInitialise(config);

            // Wait for the initialisation to complete.
            m_InitialisationCompleteResetEvent.WaitOne(INITIALISATION_TIMEOUT);

            // Assertions
            Assert.AreEqual(InitialisationResult.Initialised, m_InitCompleteArgs.Result);
        }

        [TestMethod]
        public void TestInitiliase_Fail()
        {
            // Call BeginInitialise
            DataControllerConfig config = new DataControllerConfig(
                "blah", // pass an invalid file path
                IMAGE_FILES_DIRECTORY);
            m_DataController.BeginInitialise(config);

            // Wait for the initialisation to complete.
            m_InitialisationCompleteResetEvent.WaitOne(INITIALISATION_TIMEOUT);

            // Assertions
            Assert.AreEqual(InitialisationResult.Error, m_InitCompleteArgs.Result);
        }

        [TestMethod]
        public void TestGetImageFile_Success()
        {
            // Setup
            ConnectGoodDatabase();

            // Call BeginGetCapture
            Guid guid = m_DataController.BeginGetCapture(ScannerType.None);

            // Wait for the request to complete.
            m_GetCaptureRequestCompleteResetEvent.WaitOne(GET_CAPTURE_TIMEOUT);

            // Assertions
            Assert.IsNotNull(m_GetCaptureArgs.Capture);
            Assert.IsNotNull(m_GetCaptureArgs.Capture.ImageData);
        }

        [TestMethod]
        public void TestGetImageFile_Fail()
        {
            ConnectGoodDatabaseNoMatchingImages();

            // Call BeginGetCapture
            Guid guid = m_DataController.BeginGetCapture(ScannerType.None);

            // Wait for the request to complete.
            m_GetCaptureRequestCompleteResetEvent.WaitOne(GET_CAPTURE_TIMEOUT);

            Assert.IsNull(m_GetCaptureArgs.Capture);
        }

        #region Private Methods

        private void ConnectGoodDatabase()
        {
            DataControllerConfig config = new DataControllerConfig(
                DATABASE_PATH,
                IMAGE_FILES_DIRECTORY);
            m_DataController.BeginInitialise(config);

            // Wait for the initialisation to complete.
            m_InitialisationCompleteResetEvent.WaitOne(INITIALISATION_TIMEOUT);

            // Assertions
            Assert.AreEqual(InitialisationResult.Initialised, m_InitCompleteArgs.Result);
        }

        private void ConnectGoodDatabaseNoMatchingImages()
        {
            DataControllerConfig config = new DataControllerConfig(
                DATABASE_PATH,
                NO_MATCHING_IMAGES_DIRECTORY);
            m_DataController.BeginInitialise(config);

            // Wait for the initialisation to complete.
            m_InitialisationCompleteResetEvent.WaitOne(INITIALISATION_TIMEOUT);

            // Assertions
            Assert.AreEqual(InitialisationResult.Initialised, m_InitCompleteArgs.Result);
        }

        #endregion

        #region Event Handlers

        private void DataController_InitialisationComplete(object sender, InitialisationCompleteEventArgs e)
        {
            // Record the event args
            m_InitCompleteArgs = e;
            // Allow the test to proceed
            m_InitialisationCompleteResetEvent.Set();
        }

        private void DataController_GetCaptureRequestComplete(object sender, GetCaptureCompleteEventArgs e)
        {
            // Record the event args
            m_GetCaptureArgs = e;
            // Allow the test to proceed
            m_GetCaptureRequestCompleteResetEvent.Set();
        }

        #endregion
    }
}
