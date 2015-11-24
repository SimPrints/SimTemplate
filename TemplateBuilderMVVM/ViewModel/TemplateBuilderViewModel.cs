using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using TemplateBuilder.Helpers;
using TemplateBuilder.Model;
using TemplateBuilder.ViewModel.Commands;
using TemplateBuilder.ViewModel.States;

namespace TemplateBuilder.ViewModel
{
    public class TemplateBuilderViewModel : ViewModel
    {
        #region Constants

        private const int LINE_LENGTH = 20;

        #endregion

        private readonly TemplateBuilderViewModelParameters m_Parameters;
        private StateManager m_StateMgr;
        // ViewModel-driven properties
        private BitmapImage m_Image;
        private bool m_IsInputMinutiaTypePermitted;
        // View and ViewModel-driven properties
        private MinutiaType m_InputMinutiaType;
        // View-driven properties
        private Vector m_Scale;
        // Commands
        private ICommand m_LoadFileCommand;
        private ICommand m_LoadFolderCommand;
        private ICommand m_SaveTemplateCommand;

        private ICommand m_TerminationButtonPressCommand;
        private ICommand m_BifuricationButtonPressCommand;
        private ICommand m_EscapePressCommand;

        #region Constructor

        public TemplateBuilderViewModel(TemplateBuilderViewModelParameters parameters)
        {
            IntegrityCheck.IsNotNull(parameters);
            IntegrityCheck.IsNotNullOrEmpty(parameters.SqliteDatabase);
            IntegrityCheck.IsNotNullOrEmpty(parameters.IdCol);
            IntegrityCheck.IsNotNullOrEmpty(parameters.ScannerNameCol);
            IntegrityCheck.IsNotNullOrEmpty(parameters.FingerNumberCol);
            IntegrityCheck.IsNotNullOrEmpty(parameters.CaptureNumberCol);

            m_Parameters = parameters;
            m_StateMgr = new StateManager(this, typeof(Uninitialised));
            InitialiseCommands();

            // Start the state machine.
            m_StateMgr.Start();
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
        public ICommand LoadFileCommand { get { return m_LoadFileCommand; } }

        /// <summary>
        /// Gets the 'load folder' command for binding to the 'openFolder' button.
        /// </summary>
        public ICommand LoadFolderCommand { get { return m_LoadFolderCommand; } }

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

        public string ImageFileName { get; set; }

        public IEnumerator<string> ImageFileNames { get; set; }

        public TemplateBuilderViewModelParameters Parameters { get { return m_Parameters; } }

        public SQLiteConnection SQLiteConnection { get; set; }

        public TemplateBuilderException Exception { get; set; }

        #region Command Callbacks

        private void LoadFile()
        {
            m_StateMgr.State.OpenFile();
        }

        private void LoadFolder()
        {
            m_StateMgr.State.OpenFolder();
        }

        private void SaveTemplate()
        {
            m_StateMgr.State.SaveTemplate();
        }

        private void EscapeAction()
        {
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

        public void itemsControl_MouseUp(Point p)
        {
            m_StateMgr.State.PositionInput(p);
        }

        public void itemsControl_MouseMove(Point p)
        {
            m_StateMgr.State.PositionMove(p);
        }

        public void Ellipse_MouseRightButtonUp(int index)
        {
            m_StateMgr.State.RemoveItem(index);
        }

        public void image_SizeChanged(Size newSize)
            {
            m_StateMgr.State.image_SizeChanged(newSize);
            }

        #endregion

        #region Private Methods

        private void InitialiseCommands()
        {
            m_LoadFileCommand = new RelayCommand(x => LoadFile());
            m_LoadFolderCommand = new RelayCommand(x => LoadFolder());
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
