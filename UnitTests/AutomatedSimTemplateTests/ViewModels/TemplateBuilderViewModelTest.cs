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
using SimTemplate.ViewModels.Interfaces;
using SimTemplate.Model.DataControllers.EventArguments;
using System;
using System.Collections.Generic;

namespace AutomatedSimTemplateTests.ViewModel.MainWindow
{
    /// <summary>
    /// Test class for testing the MainWindowViewModel. This consists primarily of testing the state machine.
    /// </summary>
    [TestClass]
    public class MainWindowViewModelTest
    {
        #region Static & Constants

        private const ScannerType DEFAULT_SCANNER_TYPE = ScannerType.None;
        private const MinutiaType DEFAULT_MINUTIA_TYPE = MinutiaType.Termination;
        private static readonly CaptureInfo EXAMPLE_CAPTURE = new CaptureInfo(1, new byte[] { 0 }, new byte[] { 0 });

        private const string INITIALISING_PROMPT_TEXT = "Initialising...";
        private const string IDLE_PROMPT_TEXT = "Ready";
        private const string LOADING_PROMPT_TEXT = "Loading...";
        private const string SAVING_PROMPT_TEXT = "Saving...";

        #endregion

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
        public void TestBeginInitialise_IgnoredConnection()
        {
            DoTestBeginInitialise();

            DoTestInitialisationIgnored();
        }

        [TestMethod]
        public void TestBeginInitialise_ConnectionFailed_Reinitialise_ConnectionSuccess()
        {
            // First test a failed connection
            DoTestFailConnection();

            // Now test a successful connection
            DoTestSucceedConnection();
        }

        [TestMethod]
        public void TestBeginInitialise_2xInvalidSettings_Updated_ConnectionSuccess()
        {
            // PREPARE:
            // Indicate that the settings are invalid, then valid
            A.CallTo(() => m_SettingsValidator.ValidateCurrentSettings())
                .ReturnsNextFromSequence(false, false, true);
            // Indicate that settings are first cancelled, then updated
            A.CallTo(() => m_WindowService.ShowDialog(m_SettingsViewModel)).Returns(true);
            A.CallTo(() => m_WindowService.ShowDialog(m_SettingsViewModel))
                .ReturnsNextFromSequence(false, true);
            // Return a valid identifier for asynchronous connection operation
            Guid identifier = Guid.NewGuid();
            A.CallTo(() => m_DataController.BeginInitialise(A<DataControllerConfig>._)).Returns(identifier);

            // EXECUTE:
            // Start initialisation
            ((IMainWindowViewModel)m_ViewModel).BeginInitialise();

            // ASSERT:
            // Assert public state of ViewModel
            Assert.AreEqual(Activity.Transitioning, m_ViewModel.CurrentActivity);
            Assert.IsFalse(m_ViewModel.IsTemplating);
            Assert.IsNull(m_ViewModel.Exception);
            Assert.AreEqual(INITIALISING_PROMPT_TEXT, m_ViewModel.PromptText);

            // Assert that settings dialog is requested 
            A.CallTo(() => m_WindowService.ShowDialog(m_SettingsViewModel))
                .MustHaveHappened(Repeated.Exactly.Times(3));
            A.CallTo(() => m_WindowService.Show(m_SettingsViewModel)).MustNotHaveHappened();

            // Assert SettingsViewModel interaction
            A.CallTo(() => m_SettingsViewModel.Refresh())
                .MustHaveHappened(Repeated.Exactly.Times(3));
            // First the dialog is cancelled
            A.CallTo(m_SettingsViewModel)
                .Where(x => x.Method.Name.Equals("set_Result"))
                .WhenArgumentsMatch(x => x.Get<ViewModelStatus>(0).Equals(ViewModelStatus.Running));
            A.CallTo(m_SettingsViewModel)
                .Where(x => x.Method.Name.Equals("set_Result"))
                .WhenArgumentsMatch(x => x.Get<ViewModelStatus>(0).Equals(ViewModelStatus.NoChange));
            // Next the dialog is submitted, with invalid settings
            A.CallTo(m_SettingsViewModel)
                .Where(x => x.Method.Name.Equals("set_Result"))
                .WhenArgumentsMatch(x => x.Get<ViewModelStatus>(0).Equals(ViewModelStatus.Running));
            A.CallTo(m_SettingsViewModel)
                .Where(x => x.Method.Name.Equals("set_Result"))
                .WhenArgumentsMatch(x => x.Get<ViewModelStatus>(0).Equals(ViewModelStatus.Complete));
            // Next the dialog is submitted with valid settings
            A.CallTo(m_SettingsViewModel)
                .Where(x => x.Method.Name.Equals("set_Result"))
                .WhenArgumentsMatch(x => x.Get<ViewModelStatus>(0).Equals(ViewModelStatus.Running));
            A.CallTo(m_SettingsViewModel)
                .Where(x => x.Method.Name.Equals("set_Result"))
                .WhenArgumentsMatch(x => x.Get<ViewModelStatus>(0).Equals(ViewModelStatus.Complete));

            // Assert that ViewModel made no calls to ITemplatingViewModel, IDataController or ISettingsViewModel
            AssertNoCallsToDataController();
            AssertNoCallsToTemplatingViewModel();

            // Now finish the connection successfully
            DoTestInitialisationSuccess(identifier);
        }

        [TestMethod]
        public void TestInitialise_Success_LoadFile_Success()
        {
            // First connect
            DoTestSucceedConnection();

            // Next, request a file
            Guid identifier = DoTestLoadFile();

            // Next, indicate file is ready
            DoTestLoadFileSuccess(identifier);
        }

        #region Do Test Routine Methods

        private void DoTestFailConnection()
        {
            // First test calling BeginInitialise (starting asynchronous operation)
            Guid identifier = DoTestBeginInitialise();

            // Then test InitialisationComplete
            DoTestInitialisationFailed(identifier);
        }

        private void DoTestSucceedConnection()
        {
            // First test calling BeginInitialise (starting asynchronous operation)
            Guid identifier = DoTestBeginInitialise();

            // Then test InitialisationComplete
            DoTestInitialisationSuccess(identifier);
        }

        #region Initialisation

        private Guid DoTestBeginInitialise()
        {
            Fake.CreateScope();
            // PREPARE:
            // Indicate that the settings are valid
            A.CallTo(() => m_SettingsValidator.ValidateCurrentSettings()).Returns(true);
            A.CallTo(() => m_SettingsValidator.GetCurrentSetting(Setting.ApiKey)).Returns("");
            A.CallTo(() => m_SettingsValidator.GetCurrentSetting(Setting.RootUrl)).Returns("");
            // Return a valid identifier for asynchronous connection operation
            Guid identifier = Guid.NewGuid();
            A.CallTo(() => m_DataController.BeginInitialise(A<DataControllerConfig>._)).Returns(identifier);

            // EXECUTE:
            // First start the initialisation
            ((IMainWindowViewModel)m_ViewModel).BeginInitialise();

            // ASSERT:
            // Assert that ViewModel made only one request to IDataController
            // TODO: Check DataControllerConfig value
            A.CallTo(() => m_DataController.BeginInitialise(A<DataControllerConfig>._))
                .MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => m_DataController.BeginGetCapture(A<ScannerType>._))
                .MustNotHaveHappened();
            A.CallTo(() => m_DataController.AbortRequest(A<Guid>._))
                .MustNotHaveHappened();
            A.CallTo(() => m_DataController.BeginSaveTemplate(A<long>._, A<byte[]>._))
                .MustNotHaveHappened();

            // Assert SettingsValidator calls
            AssertSettingsChecked();

            // Assert public state of ViewModel
            Assert.AreEqual(Activity.Transitioning, m_ViewModel.CurrentActivity);
            Assert.IsFalse(m_ViewModel.IsTemplating);
            Assert.IsNull(m_ViewModel.Exception);
            Assert.AreEqual(INITIALISING_PROMPT_TEXT, m_ViewModel.PromptText);

            // Assert that ViewModel made no calls to other fake objects
            AssertNoCallsToSettingsViewModel();
            AssertNoCallsToTemplatingViewModel();
            AssertNoCallsToWindowService();

            // Return the identifier so that the operation can be completed later
            return identifier;
        }

        private void DoTestInitialisationSuccess(Guid identifier)
        {
            Fake.CreateScope();
            // EXECUTE:
            m_DataController.InitialisationComplete += Raise.With(
                new InitialisationCompleteEventArgs(InitialisationResult.Initialised, identifier, DataRequestResult.Success));

            // ASSERT:
            // Assert public state of ViewModel
            Assert.AreEqual(Activity.Idle, m_ViewModel.CurrentActivity);
            Assert.IsFalse(m_ViewModel.IsTemplating);
            Assert.IsNull(m_ViewModel.Exception);
            Assert.AreEqual("Ready", m_ViewModel.PromptText);

            // Assert that ViewModel made no calls to other fake objects
            AssertNoCallsToDataController();
            AssertNoCallsToSettingsValidator();
            AssertNoCallsToSettingsViewModel();
            AssertNoCallsToTemplatingViewModel();
            AssertNoCallsToWindowService();
        }

        private void DoTestInitialisationFailed(Guid identifier)
        {
            Fake.CreateScope();
            // EXECUTE:
            m_DataController.InitialisationComplete += Raise.With(
                new InitialisationCompleteEventArgs(InitialisationResult.Error, identifier, DataRequestResult.Failed));

            // ASSERT:
            // Assert public state of ViewModel
            Assert.AreEqual(Activity.Fault, m_ViewModel.CurrentActivity);
            Assert.IsFalse(m_ViewModel.IsTemplating);
            Assert.IsNotNull(m_ViewModel.Exception);
            Assert.AreEqual(m_ViewModel.Exception.Message, m_ViewModel.PromptText);

            // Assert that ViewModel made no calls to other fake objects
            AssertNoCallsToDataController();
            AssertNoCallsToSettingsValidator();
            AssertNoCallsToSettingsViewModel();
            AssertNoCallsToTemplatingViewModel();
            AssertNoCallsToWindowService();
        }

        private void DoTestInitialisationIgnored()
        {
            Fake.CreateScope();
            // EXECUTE
            m_DataController.InitialisationComplete += Raise.With(
                new InitialisationCompleteEventArgs(InitialisationResult.Error, Guid.NewGuid(), DataRequestResult.Failed));

            // ASSERT:
            // Assert public state of ViewModel
            Assert.AreEqual(Activity.Transitioning, m_ViewModel.CurrentActivity);
            Assert.IsFalse(m_ViewModel.IsTemplating);
            Assert.IsNull(m_ViewModel.Exception);
            Assert.AreEqual(INITIALISING_PROMPT_TEXT, m_ViewModel.PromptText);

            // Assert that ViewModel made no calls to other fake objects
            AssertNoCallsToDataController();
            AssertNoCallsToSettingsValidator();
            AssertNoCallsToSettingsViewModel();
            AssertNoCallsToTemplatingViewModel();
            AssertNoCallsToWindowService();
        }

        #endregion

        #region Load File

        private Guid DoTestLoadFile()
        {
            Fake.CreateScope();

            // PREPARE:
            // Return a valid identifier for asynchronous load file operation
            Guid identifier = Guid.NewGuid();
            A.CallTo(() => m_DataController.BeginGetCapture(DEFAULT_SCANNER_TYPE)).Returns(identifier);

            // EXECUTE:
            m_ViewModel.LoadFileCommand.Execute(null);

            // ASSERT:
            // Assert that ViewModel made only one request to IDataController
            A.CallTo(() => m_DataController.BeginGetCapture(A<ScannerType>._))
                .MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => m_DataController.BeginInitialise(A<DataControllerConfig>._))
                .MustNotHaveHappened();
            A.CallTo(() => m_DataController.AbortRequest(A<Guid>._))
                .MustNotHaveHappened();
            A.CallTo(() => m_DataController.BeginSaveTemplate(A<long>._, A<byte[]>._))
                .MustNotHaveHappened();

            // Assert public state of ViewModel
            Assert.AreEqual(Activity.Transitioning, m_ViewModel.CurrentActivity);
            Assert.IsFalse(m_ViewModel.IsTemplating);
            Assert.IsNull(m_ViewModel.Exception);
            Assert.AreEqual(LOADING_PROMPT_TEXT, m_ViewModel.PromptText);

            // Assert that ViewModel made no calls to ITemplatingViewModel or ISettingsViewModel
            AssertNoCallsToSettingsValidator();
            AssertNoCallsToSettingsViewModel();
            AssertNoCallsToTemplatingViewModel();

            // Return identifier so that test may continue
            return identifier;
        }

        private void DoTestLoadFileSuccess(Guid identifier)
        {
            // EXECUTE:
            m_DataController.GetCaptureComplete += Raise.With(
                new GetCaptureCompleteEventArgs(EXAMPLE_CAPTURE, identifier, DataRequestResult.Success));

            // ASSERT:
            // Assert public state of ViewModel
            Assert.AreEqual(Activity.Templating, m_ViewModel.CurrentActivity);
            Assert.IsTrue(m_ViewModel.IsTemplating);
            Assert.IsNull(m_ViewModel.Exception);
            Assert.IsNull(m_ViewModel.PromptText);

            // Assert TemplatingViewModel
            A.CallTo(() => m_TemplatingViewModel.BeginTemplating(EXAMPLE_CAPTURE)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => m_TemplatingViewModel.BeginTemplating(A<CaptureInfo>._)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => m_TemplatingViewModel.EscapeAction()).MustNotHaveHappened();
            A.CallTo(() => m_TemplatingViewModel.FinaliseTemplate()).MustNotHaveHappened();
            A.CallTo(() => m_TemplatingViewModel.QuitTemplating()).MustNotHaveHappened();
            A.CallTo(m_TemplatingViewModel)
                .Where(x => x.Method.Name.Equals("set_InputMinutiaType"))
                .WhenArgumentsMatch(x => x.Get<MinutiaType>(0).Equals(DEFAULT_MINUTIA_TYPE));


            // Assert that there were no calls to other fakes
            AssertNoCallsToDataController();
            AssertNoCallsToSettingsValidator();
        }

        #endregion

        #endregion

        #region Assertion Methods

        private void AssertSettingsChecked()
        {
            A.CallTo(() => m_SettingsValidator.ValidateCurrentSettings()).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => m_SettingsValidator.GetCurrentSetting(Setting.ApiKey)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => m_SettingsValidator.GetCurrentSetting(Setting.RootUrl)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => m_SettingsValidator.SaveSettings()).MustNotHaveHappened();
            A.CallTo(() => m_SettingsValidator.UpdateSetting(A<Setting>._, A<object>._)).MustNotHaveHappened();
            A.CallTo(() => m_SettingsValidator.ValidateQuerySetting(A<Setting>._, A<object>._)).MustNotHaveHappened();
            A.CallTo(() => m_SettingsValidator.ValidationHelpText(A<Setting>._)).MustNotHaveHappened();
        }

        private void AssertNoCallsToDataController()
        {
            A.CallTo(() => m_DataController.BeginInitialise(A<DataControllerConfig>._))
                .MustNotHaveHappened();
            A.CallTo(() => m_DataController.BeginGetCapture(A<ScannerType>._))
                .MustNotHaveHappened();
            A.CallTo(() => m_DataController.AbortRequest(A<Guid>._))
                .MustNotHaveHappened();
            A.CallTo(() => m_DataController.BeginSaveTemplate(A<long>._, A<byte[]>._))
                .MustNotHaveHappened();
        }

        private void AssertNoCallsToSettingsViewModel()
        {
            A.CallTo(() => m_SettingsViewModel.Result).MustNotHaveHappened();
            A.CallTo(() => m_SettingsViewModel.Refresh()).MustNotHaveHappened();
            A.CallTo(m_SettingsViewModel)
                .Where(x => x.Method.Name.Equals("set_Result"))
                .MustNotHaveHappened();
        }

        private void AssertNoCallsToTemplatingViewModel()
        {
            A.CallTo(() => m_TemplatingViewModel.BeginTemplating(A<CaptureInfo>._)).MustNotHaveHappened();
            A.CallTo(() => m_TemplatingViewModel.EscapeAction()).MustNotHaveHappened();
            A.CallTo(() => m_TemplatingViewModel.QuitTemplating()).MustNotHaveHappened();
            A.CallTo(() => m_TemplatingViewModel.Capture).MustNotHaveHappened();
            A.CallTo(() => m_TemplatingViewModel.FinaliseTemplate()).MustNotHaveHappened();
            // TODO: Add sets for InputMinutiaType and StatusImage
            A.CallTo(() => m_TemplatingViewModel.IsSaveTemplatePermitted).MustNotHaveHappened();
        }

        private void AssertNoCallsToWindowService()
        {
            A.CallTo(() => m_WindowService.Show(A<object>._)).MustNotHaveHappened();
            A.CallTo(() => m_WindowService.ShowDialog(A<object>._)).MustNotHaveHappened();
        }

        private void AssertNoCallsToSettingsValidator()
        {
            A.CallTo(() => m_SettingsValidator.GetCurrentSetting(A<Setting>._)).MustNotHaveHappened();
            A.CallTo(() => m_SettingsValidator.SaveSettings()).MustNotHaveHappened();
            A.CallTo(() => m_SettingsValidator.UpdateSetting(A<Setting>._, A<object>._)).MustNotHaveHappened();
            A.CallTo(() => m_SettingsValidator.ValidateCurrentSettings()).MustNotHaveHappened();
            A.CallTo(() => m_SettingsValidator.ValidateQuerySetting(A<Setting>._, A<object>._)).MustNotHaveHappened();
            A.CallTo(() => m_SettingsValidator.ValidationHelpText(A<Setting>._)).MustNotHaveHappened();
        }

        #endregion
    }
}
