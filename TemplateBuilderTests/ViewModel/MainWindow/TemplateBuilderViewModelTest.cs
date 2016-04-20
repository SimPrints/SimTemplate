using FakeItEasy;
using log4net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using SimTemplate.Helpers;
using SimTemplate.Model;
using SimTemplate.Model.Database;
using SimTemplate.ViewModel.MainWindow;
using SimTemplate.Model.DataControllers;

namespace AutomatedSimTemplateTests.ViewModel.MainWindow
{
    [TestClass]
    public class TemplateBuilderViewModelTest
    {
        private static readonly ILog m_Log = LogManager.GetLogger(typeof(TemplateBuilderViewModelTest));
        IDataController m_DataController;
        TemplateBuilderViewModel m_ViewModel;

        [TestInitialize]
        public void TestSetup()
        {
            m_DataController = A.Fake<IDataController>();
            m_ViewModel = new TemplateBuilderViewModel(m_DataController);
        }


        [TestMethod]
        public void TestConnect_Fail()
        {
            // TODO: fake the initialiseComplete event
            //A.CallTo(() => m_DataController.Initialise(A<DataControllerConfig>._))
            //    .Invokes(() => )
            m_Log.Debug("Starting test...");
            m_ViewModel.BeginInitialise();

            // Assert that ViewModel made no further requests
            A.CallTo(() => m_DataController.BeginGetCapture(A<ScannerType>._, A<bool>._))
                .MustNotHaveHappened();
            A.CallTo(() => m_DataController.BeginInitialise(A<DataControllerConfig>._))
                .MustHaveHappened(Repeated.Exactly.Once);

            // Assert public state of ViewModel
            Assert.IsNull(m_ViewModel.Capture);
            Assert.AreEqual(0, m_ViewModel.Minutae.Count());
            Assert.IsNull(m_ViewModel.Exception);
        }

        [TestMethod]
        public void TestTemplating_Save()
        {

        }
    }
}
