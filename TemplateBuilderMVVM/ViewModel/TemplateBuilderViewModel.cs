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

        // ViewModel-driven properties
        private StateManager m_StateMgr;
        private BitmapImage m_Image;
        // View and ViewModel-driven properties
        private MinutiaType m_InputMinutiaType;
        // View-driven properties
        private Vector m_Scale;
        // Commands
        private ICommand m_LoadFileCommand;
        private ICommand m_SaveTemplateCommand;
        private ICommand m_TerminationButtonPressCommand;
        private ICommand m_BifuricationButtonPressCommand;

        #region Constructor

        public TemplateBuilderViewModel()
        {
            Minutae = new TrulyObservableCollection<MinutiaRecord>();
            m_StateMgr = new StateManager(this);

            // Commands
            m_LoadFileCommand = new RelayCommand(x => LoadFile());
            m_SaveTemplateCommand = new RelayCommand(x => SaveTemplate());
            m_TerminationButtonPressCommand = new RelayCommand(
                x => InputMinutiaType = MinutiaType.Termination);
            m_BifuricationButtonPressCommand = new RelayCommand(
                x => InputMinutiaType = MinutiaType.Bifurication);
        }

        #endregion

        #region Bound Properties
        /// <summary>
        /// Gets the minutae. Bound to the canvas.
        /// </summary>
        public TrulyObservableCollection<MinutiaRecord> Minutae { get; set; }
        /// <summary>
        /// Gets the 'load file' command for binding to the 'Open' button.
        /// </summary>
        public ICommand LoadFileCommand { get { return m_LoadFileCommand; } }
        /// <summary>
        /// Gets the 'save template' command for binding to the 'Save' button.
        /// </summary>
        public ICommand SaveTemplateCommand { get { return m_SaveTemplateCommand; } }
        /// <summary>
        /// Gets the termination button press command.
        /// </summary>
        public ICommand TerminationButtonPressCommand { get { return m_TerminationButtonPressCommand; } }
        /// <summary>
        /// Gets the bifurication button press command.
        /// </summary>
        public ICommand BifuricationButtonPressCommand { get { return m_BifuricationButtonPressCommand; } }
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
                    Console.WriteLine("InputMinutiaType st to {0}", m_InputMinutiaType);
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

        #region Command Callbacks

        private void LoadFile()
        {
            m_StateMgr.State.LoadFile();
        }

        private void SaveTemplate()
        {
            m_StateMgr.State.SaveTemplate();
        }

        private void SetMinutiaType(MinutiaType type)
        {
            m_StateMgr.State.SetMinutiaType(type);
        }

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
    }
}
