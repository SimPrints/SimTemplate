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
        // Commands
        private ICommand m_LoadFileCommand;
        private ICommand m_SaveTemplateCommand;
        private ICommand m_TerminationCommand;
        private ICommand m_BifuricationCommand;

        /// <summary>
        /// Gets the minutae. Bound to the canvas.
        /// </summary>
        /// <value>
        /// The minutae.
        /// </value>
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
        /// Gets the 'termination' command for binding to the 'termination' radio button.
        /// </summary>
        public ICommand TerminationCommand { get { return m_TerminationCommand; } }
        /// <summary>
        /// Gets the 'bifurication' command for binding to the 'bifurication' radio button.
        /// </summary>
        public ICommand BifuricationCommand { get { return m_BifuricationCommand; } }
        /// <summary>
        /// Gets or sets the scaling applied to the image.
        /// </summary>
        public double Scale { get; set; }

        /// <summary>
        /// Gets or sets the image file. Bound to the canvas background.
        /// </summary>
        /// <value>
        /// The image file.
        /// </value>
        public BitmapImage Image
        {
            get { return m_Image; }
            set
            {
                if (value != this.m_Image)
                {
                    this.m_Image = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public string ImageFileName { get; set; }

        public TemplateBuilderViewModel()
        {
            Minutae = new TrulyObservableCollection<MinutiaRecord>();
            m_StateMgr = new StateManager(this);
            m_LoadFileCommand = new RelayCommand(x => LoadFile());
            m_SaveTemplateCommand = new RelayCommand(x => SaveTemplate());
            m_TerminationCommand = new RelayCommand(
                x => SetMinutiaType(MinutiaType.Termination),
                x => m_StateMgr.State.IsMinutiaTypeButtonsEnabled);
            m_BifuricationCommand = new RelayCommand(
                x => SetMinutiaType(MinutiaType.Bifurication),
                x => m_StateMgr.State.IsMinutiaTypeButtonsEnabled);
        }

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

        public void PositionInput(Point e)
        {
            m_StateMgr.State.PositionInput(e);
        }

        public void PositionMove(Point e)
        {
            m_StateMgr.State.PositionMove(e);
        }

        public void RemoveItem(int index)
        {
            m_StateMgr.State.RemoveItem(index);
        }

        #endregion
    }
}
