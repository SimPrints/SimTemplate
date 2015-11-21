using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using TemplateBuilderMVVM.Helpers;
using TemplateBuilderMVVM.Model;
using TemplateBuilderMVVM.ViewModel.Commands;
using TemplateBuilderMVVM.ViewModel.States;

namespace TemplateBuilderMVVM.ViewModel
{
    public class TemplateBuilderViewModel : ViewModel
    {
        #region Constants

        private const int LINE_LENGTH = 20;

        #endregion

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

        #region Constructor

        public TemplateBuilderViewModel()
        {
            m_StateMgr = new StateManager(this);
            InitialiseCommands();
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
        /// Gets the termination button press command.
        /// </summary>
        public ICommand SetTerminationInputMinutiaType { get { return m_TerminationButtonPressCommand; } }

        /// <summary>
        /// Gets the bifurication button press command.
        /// </summary>
        public ICommand SetBifuricationInputMinutiaType { get { return m_BifuricationButtonPressCommand; } }

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
        }

        #endregion
    }
}
