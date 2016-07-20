using log4net;
using SimTemplate.DataTypes.Enums;
using SimTemplate.Helpers;
using SimTemplate.Model.DataControllers;
using SimTemplate.Model.DataControllers.EventArguments;
using SimTemplate.StateMachine;
using SimTemplate.ViewModels.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SimTemplate.ViewModels
{
    public partial class MainWindowViewModel : ViewModel
    {
        private static readonly ILog m_Log = LogManager.GetLogger(typeof(MainWindowViewModel));

        // State machine members
        private StateManager<MainWindowState> m_StateMgr;
        private object m_StateLock;

        // Child ViewModels
        private readonly ITemplatingViewModel m_TemplatingViewModel;

        // ViewModel-driven members
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

        #region Constructor

        public MainWindowViewModel(
            IDataController dataController,
            ITemplatingViewModel templatingViewModel)
        {
            IntegrityCheck.IsNotNull(dataController);
            IntegrityCheck.IsNotNull(templatingViewModel);

            m_DataController = dataController;
            m_TemplatingViewModel = templatingViewModel;

            // Initialise the state machine
            m_StateMgr = new StateManager<MainWindowState>(this, typeof(Uninitialised));
            m_StateLock = new object();

            // Configure commands/event handlers
            InitialiseCommands();
            m_DataController.InitialisationComplete += DataController_InitialisationComplete;
            m_DataController.GetCaptureComplete += DataController_GetCaptureComplete;
            m_DataController.SaveTemplateComplete += DataController_SaveTemplateComplete;
        }

        #endregion

        public SimTemplateException Exception { get { return m_Exception; } }

        #region Command Callbacks

        private void LoadFile()
        {
            m_Log.Debug("LoadFile() called.");
            lock (m_StateLock)
            {
                m_StateMgr.State.LoadFile();
            }
        }

        private void SaveTemplate()
        {
            m_Log.Debug("SaveTemplate() called.");
            lock (m_StateLock)
            {
                m_StateMgr.State.SaveTemplate();
            }
        }

        private void EscapeAction()
        {
            m_Log.Debug("EscapeAction called.");
            lock (m_StateLock)
            {
                m_StateMgr.State.EscapeAction();
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

        #endregion

        #region Private Methods

        private void InitialiseCommands()
        {
            m_LoadFileCommand = new RelayCommand(x => LoadFile());
            m_SaveTemplateCommand = new RelayCommand(
                x => SaveTemplate(),
                x => m_TemplatingViewModel.IsSaveTemplatePermitted);
            m_TerminationButtonPressCommand = new RelayCommand(
                x => m_TemplatingViewModel.InputMinutiaType = MinutiaType.Termination,
                x => IsTemplating);
            m_BifuricationButtonPressCommand = new RelayCommand(
                x => m_TemplatingViewModel.InputMinutiaType = MinutiaType.Bifurication,
                x => IsTemplating);
            m_EscapePressCommand = new RelayCommand(x => EscapeAction());
        }

        #endregion

        #region Event Handlers

        private void DataController_InitialisationComplete(object sender, InitialisationCompleteEventArgs e)
        {
            m_Log.DebugFormat(
                "DataController_InitialisationComplete(e.Result={0}) called.",
                e.Result);
            lock (m_StateLock)
            {
                m_StateMgr.State.DataController_InitialisationComplete(e);
            }
        }

        private void DataController_GetCaptureComplete(object sender, GetCaptureCompleteEventArgs e)
        {
            m_Log.DebugFormat(
                "DataController_GetCaptureComplete(e.Capture={0}) called.",
                e.Capture);
            lock (m_StateLock)
            {
                m_StateMgr.State.DataController_GetCaptureComplete(e);
            }
        }


        private void DataController_SaveTemplateComplete(object sender, SaveTemplateEventArgs e)
        {
            m_Log.DebugFormat(
                "DataController_SaveTemplateComplete(e.Result={0}) called.",
                e.Result);
            lock (m_StateLock)
            {
                m_StateMgr.State.DataController_SaveTemplateComplete(e);
            }
        }

        #endregion

        #region Public Methods

        public void BeginInitialise()
        {
            m_Log.Debug("BeginInitialise() called.");
            lock (m_StateLock)
            {

                m_StateMgr.State.BeginInitialise();
            }
        }

        #endregion
    }
}
