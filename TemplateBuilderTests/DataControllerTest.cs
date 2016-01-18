using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemplateBuilder.Model.Database;

namespace TemplateBuilderTests
{
    [TestClass]
    public class DataControllerTest
    {
        #region Constants

        private const string DATABASE_PATH = @"C:\SimPrints\Data\mainDb_yesFMR_noPNG.sqlite";
        private const string IMAGE_FILES_DIRECTORY = @"C:\SimPrints\Images";
        private const string NO_MATCHING_IMAGES_DIRECTORY = @"C:\SimPrints\NoImages";

        #endregion

        IDataController m_DataController;
        bool m_IsSuccessful;

        [TestInitialize]
        public void TestSetup()
        {
            m_DataController = new DataController();
            m_IsSuccessful = false;
            m_DataController.InitialisationComplete += DataController_InitialisationComplete;
        }

        [TestMethod]
        public void TestInitialise_Success()
        {
            DataControllerConfig config = new DataControllerConfig(
                DATABASE_PATH,
                IMAGE_FILES_DIRECTORY);
            m_DataController.Initialise(config);

            // Assert succeeded to connect.
            Assert.IsTrue(m_IsSuccessful);
        }

        [TestMethod]
        public void TestInitiliase_Fail()
        {
            DataControllerConfig config = new DataControllerConfig(
                "blah", // pass an invalid file path
                IMAGE_FILES_DIRECTORY);
            m_DataController.Initialise(config);

            // Assert failed to connect.
            Assert.IsFalse(m_IsSuccessful);
        }

        [TestMethod]
        public void TestGetImageFile_Success()
        {
            ConnectGoodDatabase();

            string filename = m_DataController.GetImageFile();

            Assert.IsFalse(String.IsNullOrEmpty(filename));
        }

        [TestMethod]
        public void TestGetImageFile_Fail()
        {
            ConnectGoodDatabaseNoMatchingImages();

            string filename = m_DataController.GetImageFile();

            Assert.IsTrue(String.IsNullOrEmpty(filename));
        }

        #region Private Methods

        private void ConnectGoodDatabase()
        {
            DataControllerConfig config = new DataControllerConfig(
                DATABASE_PATH,
                IMAGE_FILES_DIRECTORY);
            m_DataController.Initialise(config);
            Assert.IsTrue(m_IsSuccessful);
        }

        private void ConnectGoodDatabaseNoMatchingImages()
        {
            DataControllerConfig config = new DataControllerConfig(
                DATABASE_PATH,
                NO_MATCHING_IMAGES_DIRECTORY);
            m_DataController.Initialise(config);
            Assert.IsTrue(m_IsSuccessful);
        }

        #endregion

        #region Event Handlers

        private void DataController_InitialisationComplete(object sender, InitialisationCompleteEventArgs e)
        {
            m_IsSuccessful = e.IsSuccessful;
        }

        #endregion
    }
}
