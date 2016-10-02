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
using FakeItEasy;
using log4net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using SimTemplate.Model.DataControllers;
using SimTemplate.DataTypes.Enums;
using SimTemplate.ViewModels;
using SimTemplate.DataTypes;
using SimTemplate.Utilities;

namespace AutomatedSimTemplateTests.ViewModel.MainWindow
{
    [TestClass]
    public class MainWindowViewModelTest
    {
        private static readonly ILog m_Log = LogManager.GetLogger(typeof(MainWindowViewModelTest));

        IDataController m_DataController;
        ITemplatingViewModel m_TemplatingViewModel;
        ISettingsViewModel m_SettingsViewModel;
        ISettingsManager m_SettingsValidator;
        IWindowService m_WindowService;

        MainWindowViewModel m_ViewModel;

        [TestInitialize]
        public void TestSetup()
        {
            m_DataController = A.Fake<IDataController>();
            m_TemplatingViewModel = A.Fake<ITemplatingViewModel>();
            m_SettingsViewModel = A.Fake<ISettingsViewModel>();
            m_SettingsValidator = A.Fake<ISettingsManager>();
            m_WindowService = A.Fake<IWindowService>();

            m_ViewModel = new MainWindowViewModel(
                m_DataController,
                m_TemplatingViewModel,
                m_SettingsViewModel,
                m_SettingsValidator,
                m_WindowService);
        }


        [TestMethod]
        public void TestConnect_Fail()
        {
            // TODO: fake the initialiseComplete event
            //A.CallTo(() => m_DataController.Initialise(A<DataControllerConfig>._))
            //    .Invokes(() => )
            m_Log.Debug("Starting test...");
            m_ViewModel.BeginInitialise();

            // Assert that ViewModel made only one request to IDataController
            A.CallTo(() => m_DataController.BeginInitialise(A<DataControllerConfig>._))
                .MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => m_DataController.BeginGetCapture(A<ScannerType>._))
                .MustNotHaveHappened();

            // Assert that ViewModel made no calls to ITemplatingViewModel
            A.CallTo(() => m_TemplatingViewModel.BeginTemplating(A<CaptureInfo>._))
                .MustNotHaveHappened();
            A.CallTo(() => m_TemplatingViewModel.EscapeAction())
                .MustNotHaveHappened();
            A.CallTo(() => m_TemplatingViewModel.QuitTemplating())
                .MustNotHaveHappened();
            A.CallTo(() => m_TemplatingViewModel.Capture)
                .MustNotHaveHappened();
            A.CallTo(() => m_TemplatingViewModel.FinaliseTemplate())
                .MustNotHaveHappened();
            // TODO: Add sets for InputMinutiaType and StatusImage
            A.CallTo(() => m_TemplatingViewModel.IsSaveTemplatePermitted)
                .MustNotHaveHappened();

            // Assert public state of ViewModel
            Assert.IsNull(m_ViewModel.Exception);
        }
    }
}
