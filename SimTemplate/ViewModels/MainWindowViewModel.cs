using log4net;
using SimTemplate.DataTypes.Enums;
using SimTemplate.Helpers;
using SimTemplate.Model.DataControllers;
using SimTemplate.Model.DataControllers.EventArguments;
using SimTemplate.StateMachine;
using SimTemplate.ViewModels.Commands;
using SimTemplate.ViewModels.CustomEventArgs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SimTemplate.ViewModels
{
    // TODO: Could make generic method that takes:
    // where T1 : ViewModel, ITemplatingViewModel
    // where T2 : ViewModel, ISettingsViewModel
    public partial class MainWindowViewModel : ViewModel
    {
        // State machine members
        private StateManager<MainWindowState> m_StateMgr;
        private object m_StateLock;

        // Child ViewModels
        private readonly ITemplatingViewModel m_TemplatingViewModel;
        private readonly ISettingsViewModel m_SettingsViewModel;

        // ViewModel-driven members
        private ViewModel m_CurrentContentViewModel;
        private readonly IDataController m_DataController;
        private string m_LoadIcon;
        private SimTemplateException m_Exception;

        // View-driven members
        private ScannerType m_FilteredScannerType;

        // Commands
        private ICommand m_LoadFileCommand;
        private ICommand m_SaveTemplateCommand;
        private ICommand m_TerminationButtonPressCommand;
        private ICommand m_BifuricationButtonPressCommand;
        private ICommand m_EscapePressCommand;
        private ICommand m_ToggleSettingsCommand;

        #region Constructor

        public MainWindowViewModel(
            IDataController dataController,
            ITemplatingViewModel templatingViewModel,
            ISettingsViewModel settingsViewModel)
        {
            IntegrityCheck.IsNotNull(dataController);
            IntegrityCheck.IsNotNull(templatingViewModel);

            // Save arguments
            m_DataController = dataController;
            m_TemplatingViewModel = templatingViewModel;
            m_SettingsViewModel = settingsViewModel;

            // Set current content view model
            // TODO: Is this wrong?
            m_CurrentContentViewModel = (ViewModel)m_TemplatingViewModel;

            // Initialise the state machine
            m_StateMgr = new StateManager<MainWindowState>(this, typeof(Uninitialised));
            m_StateLock = new object();

            // Configure commands/event handlers
            InitialiseCommands();
            m_DataController.InitialisationComplete += DataController_InitialisationComplete;
            m_DataController.GetCaptureComplete += DataController_GetCaptureComplete;
            m_DataController.SaveTemplateComplete += DataController_SaveTemplateComplete;
            m_SettingsViewModel.SettingsUpdated += SettingsViewModel_SettingsUpdated;
        }

        #endregion

        public SimTemplateException Exception { get { return m_Exception; } }

        #region Command Callbacks

        private void LoadFile()
        {
            Log.Debug("LoadFile() called.");
            lock (m_StateLock)
            {
                m_StateMgr.State.LoadFile();
            }
        }

        private void SaveTemplate()
        {
            Log.Debug("SaveTemplate() called.");
            lock (m_StateLock)
            {
                m_StateMgr.State.SaveTemplate();
            }
        }

        private void EscapeAction()
        {
            Log.Debug("EscapeAction() called.");
            lock (m_StateLock)
            {
                m_StateMgr.State.EscapeAction();
            }
        }

        private void ToggleSettings()
        {
            Log.Debug("ToggleSettings() called.");
            if (m_CurrentContentViewModel.Equals(m_TemplatingViewModel))
            {
                // Update settings before showing the view
                m_SettingsViewModel.Refresh();
                CurrentContentViewModel = (ViewModel)m_SettingsViewModel;
            }
            else
            {
                CurrentContentViewModel = (ViewModel)m_TemplatingViewModel;
            }
            
        }

        #endregion

        #region Directly Bound Properties

        public ITemplatingViewModel TemplatingViewModel
        {
            get { return m_TemplatingViewModel; }
        }

        /// <summary>
        /// Gets a list of scanner type optitons.
        /// </summary>
        /// <value>
        /// The scanner types.
        /// </value>
        public IEnumerable<ScannerType> ScannerTypes
        {
            get
            {
                return Enum.GetValues(typeof(ScannerType))
                    .Cast<ScannerType>();
            }
        }

        /// <summary>
        /// Gets or sets the type of scanner type to filter GET requests for.
        /// </summary>
        /// <value>
        /// The type of scanner to filter.
        /// </value>
        public ScannerType FilteredScannerType
        {
            get { return m_FilteredScannerType; }
            set
            {
                if (value != m_FilteredScannerType)
                {
                    lock (m_StateLock)
                    {
                        m_FilteredScannerType = value;
                        m_StateMgr.State.SetScannerType(m_FilteredScannerType);
                    }
                    NotifyPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the ViewModel is currently templating.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is templating; otherwise, <c>false</c>.
        /// </value>
        public bool IsTemplating
        {
            get { return (m_StateMgr.State.GetType() == typeof(Templating)); }
        }

        public string LoadIconOverride
        {
            get { return m_LoadIcon; }
            set
            {
                if (value != m_LoadIcon)
                {
                    m_LoadIcon = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public ViewModel CurrentContentViewModel
        {
            get { return m_CurrentContentViewModel; }
            set
            {
                if (value != m_CurrentContentViewModel)
                {
                    m_CurrentContentViewModel = value;
                    NotifyPropertyChanged();
                }
            }
        }

        #endregion

        #region Commands

        /// <summary>
        /// Gets the 'load file' command for binding to the 'openFile' button.
        /// </summary>
        public ICommand LoadFileCommand { get { return m_LoadFileCommand; } }

        /// <summary>
        /// Gets the 'save template' command for binding to the 'Save' button.
        /// </summary>
        public ICommand SaveTemplateCommand { get { return m_SaveTemplateCommand; } }

        /// <summary>
        /// Gets the termination button press command for binding to the 't' key.
        /// </summary>
        public ICommand TerminationButtonPressCommand { get { return m_TerminationButtonPressCommand; } }

        /// <summary>
        /// Gets the bifurication button press command for binding to the 'b' key.
        /// </summary>
        public ICommand BifuricationButtonPressCommand { get { return m_BifuricationButtonPressCommand; } }

        /// <summary>
        /// Gets the escape press command for binding to the Esc key.
        /// </summary>
        public ICommand EscapePressCommand { get { return m_EscapePressCommand; } }

        public ICommand ToggleSettingsCommand { get { return m_ToggleSettingsCommand; } }

        #endregion

        #region Private Methods

        private bool IsSettingsOpen { get { return (m_CurrentContentViewModel == m_SettingsViewModel); } }

        private void InitialiseCommands()
        {
            m_LoadFileCommand = new RelayCommand(
                x => LoadFile(),
                x => !IsSettingsOpen);
            m_SaveTemplateCommand = new RelayCommand(
                x => SaveTemplate(),
                x => (!IsSettingsOpen && m_TemplatingViewModel.IsSaveTemplatePermitted));
            m_TerminationButtonPressCommand = new RelayCommand(
                x => m_TemplatingViewModel.InputMinutiaType = MinutiaType.Termination,
                x => (!IsSettingsOpen && IsTemplating));
            m_BifuricationButtonPressCommand = new RelayCommand(
                x => m_TemplatingViewModel.InputMinutiaType = MinutiaType.Bifurication,
                x => (!IsSettingsOpen && IsTemplating));
            m_EscapePressCommand = new RelayCommand(
                x => EscapeAction(),
                x => !IsSettingsOpen);
            m_ToggleSettingsCommand = new RelayCommand(x => ToggleSettings());
        }

        #endregion

        #region Event Handlers

        private void DataController_InitialisationComplete(object sender, InitialisationCompleteEventArgs e)
        {
            Log.DebugFormat(
                "DataController_InitialisationComplete(Result={0}) called.",
                e.Result);
            lock (m_StateLock)
            {
                m_StateMgr.State.DataController_InitialisationComplete(e);
            }
        }

        private void DataController_GetCaptureComplete(object sender, GetCaptureCompleteEventArgs e)
        {
            Log.DebugFormat(
                "DataController_GetCaptureComplete(Capture={0}) called.",
                e.Capture);
            lock (m_StateLock)
            {
                m_StateMgr.State.DataController_GetCaptureComplete(e);
            }
        }


        private void DataController_SaveTemplateComplete(object sender, SaveTemplateEventArgs e)
        {
            Log.DebugFormat(
                "DataController_SaveTemplateComplete(Result={0}) called.",
                e.Result);
            lock (m_StateLock)
            {
                m_StateMgr.State.DataController_SaveTemplateComplete(e);
            }
        }

        private void SettingsViewModel_SettingsUpdated(object sender, SettingsUpdatedEventArgs e)
        {
            Log.DebugFormat(
                "SettingsViewModel_SettingsUpdated(ApiKey={0}",
                e.ApiKey);
            lock (m_StateLock)
            {
                m_StateMgr.State.SettingsViewModel_SettingsUpdated(e.ApiKey);
            }
        }

        #endregion

        #region Public Methods

        public void BeginInitialise()
        {
            Log.Debug("BeginInitialise() called.");
            lock (m_StateLock)
            {

                m_StateMgr.State.BeginInitialise();
            }
        }

        #endregion
    }
}
