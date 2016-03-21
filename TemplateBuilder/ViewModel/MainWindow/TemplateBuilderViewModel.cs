using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using TemplateBuilder.Helpers;
using TemplateBuilder.Model;
using TemplateBuilder.Model.Database;
using TemplateBuilder.StateMachine;
using TemplateBuilder.ViewModel.Commands;

namespace TemplateBuilder.ViewModel.MainWindow
{
    public partial class TemplateBuilderViewModel : BaseViewModel
    {
        #region Constants

        private const int LINE_LENGTH = 20;

        #endregion

        private static readonly ILog m_Log = LogManager.GetLogger(typeof(TemplateBuilderViewModel));

        private StateManager<TemplateBuilderBaseState> m_StateMgr;
        private object m_StateLock;
        // ViewModel-driven properties
        private CaptureInfo m_Capture;
        private Uri m_StatusImage;
        private bool m_IsTemplating;
        private IDataController m_DataController;
        private TemplateBuilderException m_Exception;
        private string m_PromptText;
        // View and ViewModel-driven properties
        private MinutiaType m_InputMinutiaType; // TODO: Justify why enum not boolean?
        private ScannerType m_FilteredScannerType;
        private int? m_SelectedMinutia;
        // View-driven properties
        private bool m_IsGetTemplatedCapture;
        // Commands
        private ICommand m_LoadFileCommand;
        private ICommand m_SaveTemplateCommand;
        private ICommand m_TerminationButtonPressCommand;
        private ICommand m_BifuricationButtonPressCommand;
        private ICommand m_EscapePressCommand;

        #region Constructor

        public TemplateBuilderViewModel(IDataController dataController)
        {
            m_StateMgr = new StateManager<TemplateBuilderBaseState>(this);
            m_StateLock = new object();
            m_DataController = dataController;
            InitialiseCommands();

            m_DataController.InitialisationComplete += DataController_InitialisationComplete;
            m_DataController.GetCaptureComplete += m_DataController_GetCaptureComplete;
        }

        #endregion

        #region Directly Bound Properties

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

        /// <summary>
        /// Gets the minutae. Bound to the canvas.
        /// </summary>
        public TrulyObservableCollection<MinutiaRecord> Minutae { get; set; }

        /// <summary>
        /// Gets the type of minutia being input.
        /// </summary>
        public MinutiaType InputMinutiaType
        {
            get { return m_InputMinutiaType; }
            set
            {
                if (value != m_InputMinutiaType)
                {
                    lock (m_StateLock)
                    {
                        m_InputMinutiaType = value;
                        m_StateMgr.State.SetMinutiaType(m_InputMinutiaType);
                    }
                    NotifyPropertyChanged();
                }
            }
        }

        public bool IsGetTemplatedCapture
        {
            get { return m_IsGetTemplatedCapture; }
            set
            {
                if (value != m_IsGetTemplatedCapture)
                {
                    lock (m_StateLock)
                    {
                        m_IsGetTemplatedCapture = value;
                    }
                    NotifyPropertyChanged();
                }
            }
        }

        public IEnumerable<ScannerType> ScannerTypes
        {
            get
            {
                return Enum.GetValues(typeof(ScannerType))
                    .Cast<ScannerType>();
            }
        }

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
        public bool IsTemplating
        {
            get { return m_IsTemplating; }
            set
            {
                if (value != m_IsTemplating)
                {
                    m_IsTemplating = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public CaptureInfo Capture
        {
            get { return m_Capture; }
            set
            {
                if (value != m_Capture)
                {
                    m_Capture = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public Uri StatusImage
        {
            get { return m_StatusImage; }
            set
            {
                if (value != m_StatusImage)
                {
                    m_StatusImage = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public string PromptText
        {
            get { return m_PromptText; }
            set
            {
                if (value != m_PromptText)
                {
                    m_PromptText = value;
                    NotifyPropertyChanged();
                }
            }
        }

        #endregion

        public TemplateBuilderException Exception { get { return m_Exception; } }

        #region Command Handlers

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

        #region Command CanExecute

        /// <summary>
        /// Gets or sets a value indicating whether the 'save tempalte' command is active.
        /// </summary>
        public bool IsSaveTemplatePermitted { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Starts the TemplateBuilderViewModel running.
        /// </summary>
        public void Start()
        {
            m_StateMgr.Start(typeof(Initialising));
        }

        /// <summary>
        /// Takes a position on the image as a user input for templating.
        /// </summary>
        /// <param name="position">The position, in pixels, on the full scale image.</param>
        public void PositionInput(Point position)
        {
            m_Log.DebugFormat(
                "PositionInput(position.X={0}, position.Y={1}) called.",
                position.X,
                position.Y);
            lock (m_StateLock)
            {
                m_StateMgr.State.PositionInput(position);
            }
        }

        /// <summary>
        /// Updates a position on the image.
        /// </summary>
        /// <param name="position">The position, in pixels, on the full scale image.</param>
        public void PositionUpdate(Point position)
        {
            // Do not log as this occurs many times per second.
            lock (m_StateLock)
            {
                m_StateMgr.State.PositionUpdate(position);
            }
        }

        /// <summary>
        /// Updates the position of a recorded minutia.
        /// </summary>
        /// <param name="position">The position, in pixels, on the full scale image.</param>
        public void MoveMinutia(Point position)
        {
            m_Log.DebugFormat(
                "MoveMinutia(position={1}) called.",
                position);
            lock (m_StateLock)
            {
                m_StateMgr.State.MoveMinutia(position);
            }
        }

        public void RemoveMinutia(int index)
        {
            m_Log.DebugFormat(
                "Ellipse_MouseRightButtonUp(index={0}) called.",
                index);
            lock (m_StateLock)
            {
                m_StateMgr.State.RemoveMinutia(index);
            }
        }

        /// <summary>
        /// Indicates a minutia is to be moved
        /// </summary>
        /// <param name="index">The index of the minutia.</param>
        public void StartMove(int index)
        {
            m_Log.DebugFormat("StartMove(index={0}) called.", index);
            lock (m_StateLock)
            {
                m_StateMgr.State.StartMove(index);
            }
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

        private void m_DataController_GetCaptureComplete(object sender, GetCaptureCompleteEventArgs e)
        {
            m_Log.DebugFormat(
                "m_DataController_GetCaptureComplete(e.Capture={0}) called.",
                e.Capture);
            lock (m_StateLock)
            {
                m_StateMgr.State.DataController_GetCaptureComplete(e);
            }
        }

        #endregion

        #region Private Methods

        private void InitialiseCommands()
        {
            m_LoadFileCommand = new RelayCommand(x => LoadFile());
            m_SaveTemplateCommand = new RelayCommand(
                x => SaveTemplate(),
                x => IsSaveTemplatePermitted);
            m_TerminationButtonPressCommand = new RelayCommand(
                x => InputMinutiaType = MinutiaType.Termination,
                x => IsTemplating);
            m_BifuricationButtonPressCommand = new RelayCommand(
                x => InputMinutiaType = MinutiaType.Bifurication,
                x => IsTemplating);
            m_EscapePressCommand = new RelayCommand(x => EscapeAction());
        }

        #endregion
    }
}
