using log4net;
using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using TemplateBuilder.Helpers;
using TemplateBuilder.Model;
using TemplateBuilder.Model.Database;
using TemplateBuilder.ViewModel.Commands;

namespace TemplateBuilder.ViewModel.MainWindow
{
    public partial class TemplateBuilderViewModel : ViewModel
    {
        #region Constants

        private const int LINE_LENGTH = 20;

        #endregion

        private static readonly ILog m_Log = LogManager.GetLogger(typeof(TemplateBuilderViewModel));

        private StateManager m_StateMgr;
        // ViewModel-driven properties
        private BitmapImage m_Image;
        private bool m_IsInputMinutiaTypePermitted;
        private IDataController m_DataController;
        private TemplateBuilderException m_Exception;
        // View and ViewModel-driven properties
        private MinutiaType m_InputMinutiaType;
        private int? m_SelectedMinutia;
        private object m_SelectedMinutiaLock = new object();
        // View-driven properties
        private Vector m_Scale;
        // Commands
        private ICommand m_SkipFileCommand;
        private ICommand m_SaveTemplateCommand;
        private ICommand m_TerminationButtonPressCommand;
        private ICommand m_BifuricationButtonPressCommand;
        private ICommand m_EscapePressCommand;

        #region Constructor

        public TemplateBuilderViewModel(IDataController dataController)
        {
            m_StateMgr = new StateManager(this, typeof(Initialising));
            m_DataController = dataController;
            InitialiseCommands();

            m_DataController.InitialisationComplete += DataController_InitialisationComplete;
        }

        #endregion

        #region Directly Bound Properties

        /// <summary>
        /// Gets the minutae. Bound to the canvas.
        /// </summary>
        public TrulyObservableCollection<MinutiaRecord> Minutae { get; set; }

        /// <summary>
        /// Gets the 'load file' command for binding to the 'openFile' button.
        /// </summary>
        public ICommand SkipFileCommand { get { return m_SkipFileCommand; } }

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

        /// <summary>
        /// Gets or sets the scaling applied to the image.
        /// </summary>
        public Vector Scale
        {
            get { return m_Scale; }
            set
            {
                if (m_Scale != value)
                {
                    m_Scale = value;
                    NotifyPropertyChanged();
                }
            }
        }

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
                    m_InputMinutiaType = value;
                    m_StateMgr.State.SetMinutiaType(m_InputMinutiaType);
                    NotifyPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the input minutia type radio buttons
        /// should be enabled (bound to IsEnabled on buttons).
        /// </summary>
        public bool IsInputMinutiaTypePermitted
        {
            get { return m_IsInputMinutiaTypePermitted; }
            set
            {
                if (value != m_IsInputMinutiaTypePermitted)
                {
                    m_IsInputMinutiaTypePermitted = value;
                    NotifyPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the image file. Bound to the canvas background.
        /// </summary>
        public BitmapImage Image
        {
            get { return m_Image; }
            set
            {
                if (value != m_Image)
                {
                    m_Image = value;
                    NotifyPropertyChanged();
                }
            }
        }

        #endregion

        /// <summary>
        /// Starts the TemplateBuilderViewModel running.
        /// </summary>
        public void Start()
        {
            m_StateMgr.Start();
        }

        public TemplateBuilderException Exception { get { return m_Exception; } }

        #region Command Callbacks

        private void SkipFile()
        {
            m_Log.Debug("LoadFile() called.");
            m_StateMgr.State.SkipFile();
        }

        private void SaveTemplate()
        {
            m_Log.Debug("SaveTemplate() called.");
            m_StateMgr.State.SaveTemplate();
        }

        private void EscapeAction()
        {
            m_Log.Debug("EscapeAction called.");
            m_StateMgr.State.EscapeAction();
        }

        #endregion

        #region Command CanExecute

        /// <summary>
        /// Gets or sets a value indicating whether the 'save tempalte' button is active.
        /// </summary>
        public bool IsSaveTemplatePermitted { get; set; }

        #endregion

        #region Event Callbacks

        public void itemsControl_MouseUp(Point p, MouseButton changedButton)
        {
            m_Log.DebugFormat(
                "itemsControl_MouseUp(p.X={0}, p.Y={1}) called.",
                p.X,
                p.Y);
            m_StateMgr.State.PositionInput(p, changedButton);
        }

        public void MouseMove(Point p)
        {
            // Do not log as this occurs many times per second.
            m_StateMgr.State.PositionMove(p);
        }

        public void MoveMinutia(Point p)
        {
            m_Log.DebugFormat(
                "MoveMinutia(p={1}) called.",
                p);
            m_StateMgr.State.MoveMinutia(p);
        }

        public void Minutia_MouseUp(int index)
        {
            m_Log.DebugFormat(
                "Ellipse_MouseRightButtonUp(index={0}) called.",
                index);
            m_StateMgr.State.RemoveMinutia(index);
        }

        public void image_SizeChanged(Size newSize)
        {
            m_Log.DebugFormat(
                "image_SizeChanged(newSize.Width={0}, newSize.Height={1}) called.",
                newSize.Width,
                newSize.Height);
            m_StateMgr.State.image_SizeChanged(newSize);
        }

        public void StartMove(int index)
        {
            m_Log.DebugFormat("StartMove(index={0}) called.", index);
            m_StateMgr.State.StartMove(index);
        }

        #endregion

        #region Event Handlers

        private void DataController_InitialisationComplete(object sender, InitialisationCompleteEventArgs e)
        {
            m_Log.DebugFormat(
                "DataController_InitialisationComplete(e.IsSuccessful={0}) called.",
                e.IsSuccessful);

            m_StateMgr.State.DataController_InitialisationComplete(e);
        }

        #endregion

        #region Private Methods

        private void InitialiseCommands()
        {
            m_SkipFileCommand = new RelayCommand(x => SkipFile());
            m_SaveTemplateCommand = new RelayCommand(
                x => SaveTemplate(),
                x => IsSaveTemplatePermitted);
            m_TerminationButtonPressCommand = new RelayCommand(
                x => InputMinutiaType = MinutiaType.Termination);
            m_BifuricationButtonPressCommand = new RelayCommand(
                x => InputMinutiaType = MinutiaType.Bifurication);
            m_EscapePressCommand = new RelayCommand(x => EscapeAction());
        }

        #endregion
    }
}
