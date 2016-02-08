using FakeItEasy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TemplateBuilder.Model.Database;
using TemplateBuilder.ViewModel.MainWindow;

namespace TemplateBuilderTests
{
    [TestClass]
    public class TemplateBuilderViewModelTest
    {
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

            // Start the ViewModel.
            m_ViewModel.Start();

            // Assert that ViewModel made no further requests
            A.CallTo(() => m_DataController.GetImageFile()).MustNotHaveHappened();
            A.CallTo(() => m_DataController.Initialise(A<DataControllerConfig>._))
                .MustHaveHappened(Repeated.Exactly.Once);

            // Assert public state of ViewModel
            Assert.IsNull(m_ViewModel.Image);
            Assert.AreEqual(new Vector(0, 0), m_ViewModel.Scale);
            Assert.AreEqual(0, m_ViewModel.Minutae.Count());
            Assert.IsNull(m_ViewModel.Exception);
        }

        [TestMethod]
        public void TestTemplating_Save()
        {

        }
    }
}
