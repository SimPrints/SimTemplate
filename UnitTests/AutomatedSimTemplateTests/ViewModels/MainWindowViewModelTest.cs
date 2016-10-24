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
using SimTemplate.DataTypes;
using SimTemplate.Utilities;
using SimTemplate.ViewModels.Interfaces;
using SimTemplate.Model.DataControllers.EventArguments;
using System;
using System.Collections.Generic;

namespace SimTemplate.ViewModels.Test
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
        private const string LOADING_PROMPT_TEXT = "Loading capture...";
        private const string SAVING_PROMPT_TEXT = "Saving template...";
        private const string ABORTED_PROMPT_TEXT = "Aborted";

        #endregion

        private static readonly ILog m_Log = LogManager.GetLogger(typeof(MainWindowViewModelTest));

        IDataController m_DataController;
        ITemplatingViewModel m_TemplatingViewModel;
        ISettingsViewModel m_SettingsViewModel;
        ISettingsManager m_SettingsManager;
        IWindowService m_WindowService;
        IDispatcherHelper m_DispatcherHelper;

        MainWindowViewModel m_ViewModel;
        IMainWindowViewModel m_IViewModel;

        [TestInitialize]
        public void TestSetup()
        {
            m_DataController = A.Fake<IDataController>();
            m_TemplatingViewModel = A.Fake<ITemplatingViewModel>();
            m_SettingsViewModel = A.Fake<ISettingsViewModel>();
            m_SettingsManager = A.Fake<ISettingsManager>();
            m_WindowService = A.Fake<IWindowService>();
            m_DispatcherHelper = A.Fake<IDispatcherHelper>();

            m_ViewModel = new MainWindowViewModel(
                m_DataController,
                m_TemplatingViewModel,
                m_SettingsViewModel,
                m_SettingsManager,
                m_WindowService,
                m_DispatcherHelper);
            m_IViewModel = (IMainWindowViewModel)m_ViewModel;
        }

        // TODO: Test for integrity checks that matter

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

            // Next test a crashed connection
            DoTestCrashedConnection();

            // Now test a successful connection
            DoTestSucceedInitialise();
        }

        [TestMethod]
        public void TestBeginInitialise_InvalidSettings_Updated_ConnectionSuccess()
        {
            // Initialise with invalid settings
            DoTestInvalidSettings();

            // Update the settings to valid ones
            Guid identifier = DoTestUpdateSettings();

            // Now finish the connection successfully
            DoTestInitialisationSuccess(identifier);
        }

        [TestMethod]
        public void TestInitialise_Success_LoadFile_Success_SaveTemplate_Success()
        {
            Guid identifier;

            // First connect
            DoTestSucceedInitialise();

            // Then load a capture
            DoTestSucceedLoadCapture();

            // Next, save a template
            identifier = DoTestSaveTemplate();

            // Finally, indicate template was saved
            DoTestSaveTemplateSuccess(identifier);
        }

        // TODO: Test escape action

        // TODO: Test changing minutia type

        //[TestMethod]
        //public void TestSetMinutiaType()
        //{
        //    // First Connect
        //    DoTestSucceedInitialise();
        //}

        // TODO: Test failed load

        // TODO: Test failed save

        [TestMethod]
        public void TestInitialise_Success_LoadFile_Abort()
        {
            Guid identifier;

            // First connect
            DoTestSucceedInitialise();

            // Next, request a file
            identifier = DoTestLoadFile();

            // Next, indicate file is ready
            DoTestLoadFileAbort(identifier);
        }

        #region Do Test Routine Methods

        private void DoTestFailConnection()
        {
            // First test calling BeginInitialise (starting asynchronous operation)
            Guid identifier = DoTestBeginInitialise();

            // Then test InitialisationComplete
            DoTestInitialisationFailed(identifier, DataRequestResult.Failed);
        }

        private void DoTestCrashedConnection()
        {
            // First test calling BeginInitialise (starting asynchronous operation)
            Guid identifier = DoTestBeginInitialise();

            // Then test InitialisationComplete
            DoTestInitialisationFailed(identifier, DataRequestResult.TaskFailed);
        }

        private void DoTestSucceedInitialise()
        {
            // First test calling BeginInitialise (starting asynchronous operation)
            Guid identifier = DoTestBeginInitialise();

            // Then test InitialisationComplete
            DoTestInitialisationSuccess(identifier);
        }

        private void DoTestSucceedLoadCapture()
        {
            // Next, request a file
            Guid identifier = DoTestLoadFile();

            // Next, indicate file is ready
            DoTestLoadFileSuccess(identifier);
        }

        #region Initialisation

        private Guid DoTestBeginInitialise()
        {
            using (Fake.CreateScope())
            {
                // PREPARE:
                // Indicate that the settings are valid
                A.CallTo(() => m_SettingsManager.ValidateCurrentSettings()).Returns(true);
                A.CallTo(() => m_SettingsManager.GetCurrentSetting(Setting.ApiKey)).Returns("");
                A.CallTo(() => m_SettingsManager.GetCurrentSetting(Setting.RootUrl)).Returns("");
                // Return a valid identifier for asynchronous connection operation
                Guid identifier = Guid.NewGuid();
                A.CallTo(() => m_DataController.BeginInitialise(A<DataControllerConfig>._)).Returns(identifier);

                // EXECUTE:
                // First start the initialisation
                ((IMainWindowViewModel)m_ViewModel).BeginInitialise();

                // ASSERT:
                // Assert public state of ViewModel
                Assert.AreEqual(Activity.Transitioning, m_ViewModel.CurrentActivity);
                Assert.IsFalse(m_ViewModel.IsTemplating);
                Assert.IsNull(m_ViewModel.Exception);
                Assert.AreEqual(INITIALISING_PROMPT_TEXT, m_ViewModel.PromptText);

                // Assert IDataController interaction
                // TODO: Check DataControllerConfig value
                A.CallTo(() => m_DataController.BeginInitialise(A<DataControllerConfig>._))
                    .MustHaveHappened(Repeated.Exactly.Once);
                A.CallTo(() => m_DataController.BeginGetCapture(A<ScannerType>._))
                    .MustNotHaveHappened();
                A.CallTo(() => m_DataController.AbortRequest(A<Guid>._))
                    .MustNotHaveHappened();
                A.CallTo(() => m_DataController.BeginSaveTemplate(A<long>._, A<byte[]>._))
                    .MustNotHaveHappened();

                // Assert ITemplatingViewModel interaction
                A.CallTo(() => m_TemplatingViewModel.BeginInitialise()).MustHaveHappened(Repeated.Exactly.Once);
                A.CallTo(() => m_TemplatingViewModel.BeginTemplating(A<CaptureInfo>._)).MustNotHaveHappened();
                A.CallTo(() => m_TemplatingViewModel.EscapeAction()).MustNotHaveHappened();
                A.CallTo(() => m_TemplatingViewModel.FinaliseTemplate()).MustNotHaveHappened();
                A.CallTo(() => m_TemplatingViewModel.QuitTemplating()).MustNotHaveHappened();
                A.CallTo(m_TemplatingViewModel)
                    .Where(x => x.Method.Name.Equals("set_InputMinutiaType"))
                    .WhenArgumentsMatch(x => x.Get<MinutiaType>(0).Equals(DEFAULT_MINUTIA_TYPE))
                    .MustNotHaveHappened();

                // Assert ISettingsValidator interaction
                AssertSettingsChecked();

                // Assert that view model made no calls to other fake objects
                AssertNoCallsToSettingsViewModel();
                AssertNoCallsToWindowService();
                AssertNoCallsToDispatcherHelper();

                // Return the identifier so that the operation can be completed later
                return identifier;
            }
        }

        private void DoTestInitialisationSuccess(Guid identifier)
        {
            using (Fake.CreateScope())
            {
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
                AssertNoCallsToDispatcherHelper();
            }
        }

        private void DoTestInitialisationFailed(Guid identifier, DataRequestResult result)
        {
            using (Fake.CreateScope())
            {
                // EXECUTE:
                m_DataController.InitialisationComplete += Raise.With(
                    new InitialisationCompleteEventArgs(InitialisationResult.Error, identifier, result));

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
                AssertNoCallsToDispatcherHelper();
            }
        }

        private void DoTestInitialisationIgnored()
        {
            using (Fake.CreateScope())
            {
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
                AssertNoCallsToDispatcherHelper();
            }
        }

        #endregion

        #region Settings

        private Guid DoTestUpdateSettings()
        {
            using (Fake.CreateScope())
            {
                // PREPARE:
                // Indicate that settings are updated
                A.CallTo(() => m_WindowService.ShowDialog(m_SettingsViewModel)).Returns(true);
                A.CallTo(() => m_SettingsViewModel.Result).Returns(ViewModelStatus.Complete);
                // Indicate that the settings are valid
                A.CallTo(() => m_SettingsManager.ValidateCurrentSettings()).Returns(true);
                A.CallTo(() => m_SettingsManager.GetCurrentSetting(Setting.ApiKey)).Returns("");
                A.CallTo(() => m_SettingsManager.GetCurrentSetting(Setting.RootUrl)).Returns("");
                // Return a GUID for the subsequent initialisation operation
                Guid identifier = Guid.NewGuid();
                A.CallTo(() => m_DataController.BeginInitialise(A<DataControllerConfig>._)).Returns(identifier);

                // EXECUTE:
                // Open the settings dialog to update settings
                m_ViewModel.ToggleSettingsCommand.Execute(null);

                // Assert public state of view model
                Assert.AreEqual(Activity.Transitioning, m_ViewModel.CurrentActivity);
                Assert.IsFalse(m_ViewModel.IsTemplating);
                Assert.IsNull(m_ViewModel.Exception);
                Assert.AreEqual(INITIALISING_PROMPT_TEXT, m_ViewModel.PromptText);

                // Assert IWindowService interaction
                A.CallTo(() => m_WindowService.ShowDialog(m_SettingsViewModel))
                    .MustHaveHappened(Repeated.Exactly.Once);
                A.CallTo(() => m_WindowService.Show(m_SettingsViewModel))
                    .MustNotHaveHappened();

                // Assert ISettingsViewModel interaction
                A.CallTo(() => m_SettingsViewModel.Refresh()).MustHaveHappened(Repeated.Exactly.Once);
                A.CallTo(() => m_SettingsViewModel.Result).MustHaveHappened(Repeated.Exactly.Once);

                // Assert IDataController interaction
                A.CallTo(() => m_DataController.BeginInitialise(A<DataControllerConfig>._))
                    .MustHaveHappened(Repeated.Exactly.Once);
                A.CallTo(() => m_DataController.BeginGetCapture(A<ScannerType>._))
                    .MustNotHaveHappened();
                A.CallTo(() => m_DataController.AbortRequest(A<Guid>._))
                    .MustNotHaveHappened();
                A.CallTo(() => m_DataController.BeginSaveTemplate(A<long>._, A<byte[]>._))
                    .MustNotHaveHappened();

                // Assert that ViewModel made no calls to other fakes
                AssertNoCallsToTemplatingViewModel();
                AssertNoCallsToDispatcherHelper();

                return identifier;
            }
        }

        private void DoTestInvalidSettings()
        {
            // PREPARE:
            // Indicate that the settings are invalid
            A.CallTo(() => m_SettingsManager.ValidateCurrentSettings()).Returns(false);

            // EXECUTE:
            // Start initialisation
            ((IMainWindowViewModel)m_ViewModel).BeginInitialise();

            // ASSERT:
            // Assert public state of ViewModel
            Assert.AreEqual(Activity.Fault, m_ViewModel.CurrentActivity);
            Assert.IsFalse(m_ViewModel.IsTemplating);
            Assert.IsNotNull(m_ViewModel.Exception);
            Assert.AreEqual(m_ViewModel.Exception.Message, m_ViewModel.PromptText);

            // Assert calls to SettingsValidator
            A.CallTo(() => m_SettingsManager.ValidateCurrentSettings()).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => m_SettingsManager.GetCurrentSetting(Setting.ApiKey)).MustNotHaveHappened();
            A.CallTo(() => m_SettingsManager.GetCurrentSetting(Setting.RootUrl)).MustNotHaveHappened();
            A.CallTo(() => m_SettingsManager.SaveSettings()).MustNotHaveHappened();
            A.CallTo(() => m_SettingsManager.UpdateSetting(A<Setting>._, A<object>._)).MustNotHaveHappened();
            A.CallTo(() => m_SettingsManager.ValidateQuerySetting(A<Setting>._, A<object>._)).MustNotHaveHappened();
            A.CallTo(() => m_SettingsManager.ValidationHelpText(A<Setting>._)).MustNotHaveHappened();

            // Assert that ViewModel made no calls to other fakes
            AssertNoCallsToDataController();
            AssertNoCallsToSettingsViewModel();
            AssertNoCallsToTemplatingViewModel();
            AssertNoCallsToWindowService();
            AssertNoCallsToDispatcherHelper();
        }

        #endregion

        #region Load File

        private Guid DoTestLoadFile()
        {
            using (Fake.CreateScope())
            {

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
                Assert.AreEqual(Activity.Loading, m_ViewModel.CurrentActivity);
                Assert.IsFalse(m_ViewModel.IsTemplating);
                Assert.IsNull(m_ViewModel.Exception);
                Assert.AreEqual(LOADING_PROMPT_TEXT, m_ViewModel.PromptText);

                // Assert that ViewModel made no calls to ITemplatingViewModel or ISettingsViewModel
                AssertNoCallsToSettingsValidator();
                AssertNoCallsToSettingsViewModel();
                AssertNoCallsToTemplatingViewModel();
                AssertNoCallsToDispatcherHelper();

                // Return identifier so that test may continue
                return identifier;
            }
        }

        private void DoTestLoadFileSuccess(Guid identifier)
        {
            using (Fake.CreateScope())
            {
                // EXECUTE:
                m_DataController.GetCaptureComplete += Raise.With(
                    new GetCaptureCompleteEventArgs(EXAMPLE_CAPTURE, identifier, DataRequestResult.Success));

                // ASSERT:
                // Assert public state of ViewModel
                Assert.AreEqual(Activity.Templating, m_ViewModel.CurrentActivity);
                Assert.IsTrue(m_ViewModel.IsTemplating);
                Assert.IsNull(m_ViewModel.Exception);
                Assert.AreEqual(String.Empty, m_ViewModel.PromptText);

                // Assert TemplatingViewModel
                A.CallTo(() => m_TemplatingViewModel.BeginTemplating(EXAMPLE_CAPTURE)).MustHaveHappened(Repeated.Exactly.Once);
                A.CallTo(() => m_TemplatingViewModel.BeginTemplating(A<CaptureInfo>._)).MustHaveHappened(Repeated.Exactly.Once);
                A.CallTo(() => m_TemplatingViewModel.EscapeAction()).MustNotHaveHappened();
                A.CallTo(() => m_TemplatingViewModel.FinaliseTemplate()).MustNotHaveHappened();
                A.CallTo(() => m_TemplatingViewModel.QuitTemplating()).MustNotHaveHappened();
                A.CallTo(m_TemplatingViewModel)
                    .Where(x => x.Method.Name.Equals("set_InputMinutiaType"))
                    .WhenArgumentsMatch(x => x.Get<MinutiaType>(0).Equals(DEFAULT_MINUTIA_TYPE))
                    .MustNotHaveHappened();

                // Assert that there were no calls to other fakes
                AssertNoCallsToDataController();
                AssertNoCallsToSettingsValidator();
                AssertNoCallsToWindowService();
                AssertNoCallsToDispatcherHelper();
            }
        }

        private void DoTestLoadFileAbort(Guid identifier)
        {
            using (Fake.CreateScope())
            {
                // EXECUTE:
                m_ViewModel.LoadFileCommand.Execute(null);
                m_DataController.GetCaptureComplete += Raise.With(
                    new GetCaptureCompleteEventArgs(EXAMPLE_CAPTURE, identifier, DataRequestResult.Success));

                // ASSERT:
                // Assert public state of ViewModel
                Assert.AreEqual(Activity.Idle, m_ViewModel.CurrentActivity);
                Assert.IsFalse(m_ViewModel.IsTemplating);
                Assert.IsNull(m_ViewModel.Exception);
                Assert.AreEqual(IDLE_PROMPT_TEXT, m_ViewModel.PromptText);

                // Assert IDataController interaction
                A.CallTo(() => m_DataController.BeginInitialise(A<DataControllerConfig>._))
                    .MustNotHaveHappened();
                A.CallTo(() => m_DataController.BeginGetCapture(A<ScannerType>._))
                    .MustNotHaveHappened();
                A.CallTo(() => m_DataController.AbortRequest(A<Guid>._))
                    .MustHaveHappened(Repeated.Exactly.Once);
                A.CallTo(() => m_DataController.BeginSaveTemplate(A<long>._, A<byte[]>._))
                    .MustNotHaveHappened();

                // Assert that there were no calls to other fakes
                AssertNoCallsToSettingsValidator();
                AssertNoCallsToSettingsViewModel();
                AssertNoCallsToWindowService();
                AssertNoCallsToTemplatingViewModel();
                AssertNoCallsToDispatcherHelper();
            }
        }

        #endregion

        #region Save File

        private Guid DoTestSaveTemplate()
        {
            using (Fake.CreateScope())
            {

                // PREPARE:
                // Return a valid template for saving
                byte[] template = new byte[] { 0, 1, 2 };
                A.CallTo(() => m_TemplatingViewModel.FinaliseTemplate()).Returns(template);
                A.CallTo(() => m_TemplatingViewModel.IsSaveTemplatePermitted).Returns(true);
                // Return the capture that was being templated
                A.CallTo(() => m_TemplatingViewModel.Capture).Returns(EXAMPLE_CAPTURE);
                // Return a valid identifier for asynchronous load file operation
                Guid identifier = Guid.NewGuid();
                A.CallTo(() => m_DataController.BeginSaveTemplate(EXAMPLE_CAPTURE.DbId, template)).Returns(identifier);

                // EXECUTE:
                m_ViewModel.SaveTemplateCommand.Execute(null);

                // ASSERT:
                // Assert public state of ViewModel
                Assert.AreEqual(Activity.Transitioning, m_ViewModel.CurrentActivity);
                Assert.IsFalse(m_ViewModel.IsTemplating);
                Assert.IsNull(m_ViewModel.Exception);
                Assert.AreEqual(SAVING_PROMPT_TEXT, m_ViewModel.PromptText);

                // Assert interaction with IDataController
                A.CallTo(() => m_DataController.BeginGetCapture(A<ScannerType>._))
                    .MustNotHaveHappened();
                A.CallTo(() => m_DataController.BeginInitialise(A<DataControllerConfig>._))
                    .MustNotHaveHappened();
                A.CallTo(() => m_DataController.AbortRequest(A<Guid>._))
                    .MustNotHaveHappened();
                A.CallTo(() => m_DataController.BeginSaveTemplate(A<long>._, A<byte[]>._))
                    .MustHaveHappened(Repeated.Exactly.Once);
                A.CallTo(() => m_DataController.BeginSaveTemplate(EXAMPLE_CAPTURE.DbId, template))
                    .MustHaveHappened(Repeated.Exactly.Once);

                // Assert interaction with ITemplatingViewModel
                A.CallTo(() => m_TemplatingViewModel.IsSaveTemplatePermitted).MustHaveHappened(Repeated.Exactly.Once);
                A.CallTo(() => m_TemplatingViewModel.FinaliseTemplate()).MustHaveHappened(Repeated.Exactly.Once);
                A.CallTo(() => m_TemplatingViewModel.BeginTemplating(A<CaptureInfo>._)).MustNotHaveHappened();
                A.CallTo(() => m_TemplatingViewModel.EscapeAction()).MustNotHaveHappened();
                A.CallTo(() => m_TemplatingViewModel.QuitTemplating()).MustNotHaveHappened();
                A.CallTo(m_TemplatingViewModel)
                    .Where(x => x.Method.Name.Equals("set_InputMinutiaType"))
                    .MustNotHaveHappened();

                // Assert that ViewModel made no calls to ITemplatingViewModel or ISettingsViewModel
                AssertNoCallsToSettingsValidator();
                AssertNoCallsToSettingsViewModel();
                AssertNoCallsToDispatcherHelper();

                // Return identifier so that test may continue
                return identifier;
            }
        }

        private void DoTestSaveTemplateSuccess(Guid identifier)
        {
            using (Fake.CreateScope())
            {

                // EXECUTE:
                m_DataController.SaveTemplateComplete += Raise.With(
                    new SaveTemplateEventArgs(identifier, DataRequestResult.Success));

                // ASSERT:
                // Assert public state of ViewModel
                Assert.AreEqual(Activity.Idle, m_ViewModel.CurrentActivity);
                Assert.IsFalse(m_ViewModel.IsTemplating);
                Assert.IsNull(m_ViewModel.Exception);
                Assert.AreEqual(IDLE_PROMPT_TEXT, m_ViewModel.PromptText);

                // Assert TemplatingViewModel interaction
                A.CallTo(() => m_TemplatingViewModel.BeginTemplating(A<CaptureInfo>._)).MustNotHaveHappened();
                A.CallTo(() => m_TemplatingViewModel.EscapeAction()).MustNotHaveHappened();
                A.CallTo(() => m_TemplatingViewModel.Capture).MustNotHaveHappened();
                A.CallTo(() => m_TemplatingViewModel.QuitTemplating()).MustHaveHappened(Repeated.Exactly.Once);
                A.CallTo(() => m_TemplatingViewModel.FinaliseTemplate()).MustNotHaveHappened();
                A.CallTo(() => m_TemplatingViewModel.IsSaveTemplatePermitted).MustNotHaveHappened();
                A.CallTo(m_TemplatingViewModel)
                    .Where(x => x.Method.Name.Equals("set_InputMinutiaType"))
                    .WhenArgumentsMatch(x => x.Get<MinutiaType>(0).Equals(DEFAULT_MINUTIA_TYPE))
                    .MustNotHaveHappened();

                // Assert that there were no calls to other fakes
                AssertNoCallsToDataController();
                AssertNoCallsToSettingsValidator();
                AssertNoCallsToWindowService();
                AssertNoCallsToDispatcherHelper();
                AssertNoCallsToSettingsViewModel();
            }
        }

        #endregion

        #endregion

        #region Assertion Methods

        private void AssertSettingsChecked()
        {
            A.CallTo(() => m_SettingsManager.ValidateCurrentSettings()).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => m_SettingsManager.GetCurrentSetting(Setting.ApiKey)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => m_SettingsManager.GetCurrentSetting(Setting.RootUrl)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => m_SettingsManager.SaveSettings()).MustNotHaveHappened();
            A.CallTo(() => m_SettingsManager.UpdateSetting(A<Setting>._, A<object>._)).MustNotHaveHappened();
            A.CallTo(() => m_SettingsManager.ValidateQuerySetting(A<Setting>._, A<object>._)).MustNotHaveHappened();
            A.CallTo(() => m_SettingsManager.ValidationHelpText(A<Setting>._)).MustNotHaveHappened();
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
            A.CallTo(() => m_TemplatingViewModel.IsSaveTemplatePermitted).MustNotHaveHappened();
            A.CallTo(m_TemplatingViewModel)
                .Where(x => x.Method.Name.Equals("set_InputMinutiaType"))
                .WhenArgumentsMatch(x => x.Get<MinutiaType>(0).Equals(DEFAULT_MINUTIA_TYPE))
                .MustNotHaveHappened();
        }

        private void AssertNoCallsToWindowService()
        {
            A.CallTo(() => m_WindowService.Show(A<object>._)).MustNotHaveHappened();
            A.CallTo(() => m_WindowService.ShowDialog(A<object>._)).MustNotHaveHappened();
        }

        private void AssertNoCallsToSettingsValidator()
        {
            A.CallTo(() => m_SettingsManager.GetCurrentSetting(A<Setting>._)).MustNotHaveHappened();
            A.CallTo(() => m_SettingsManager.SaveSettings()).MustNotHaveHappened();
            A.CallTo(() => m_SettingsManager.UpdateSetting(A<Setting>._, A<object>._)).MustNotHaveHappened();
            A.CallTo(() => m_SettingsManager.ValidateCurrentSettings()).MustNotHaveHappened();
            A.CallTo(() => m_SettingsManager.ValidateQuerySetting(A<Setting>._, A<object>._)).MustNotHaveHappened();
            A.CallTo(() => m_SettingsManager.ValidationHelpText(A<Setting>._)).MustNotHaveHappened();
        }

        private void AssertNoCallsToDispatcherHelper()
        {
            A.CallTo(() => m_DispatcherHelper.Invoke(A<Action>._)).MustNotHaveHappened();
        }

        #endregion
    }
}
