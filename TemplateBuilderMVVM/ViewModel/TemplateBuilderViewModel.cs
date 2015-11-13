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
        private BitmapImage m_Image;
        private ICommand m_LoadFileCommand;
        private ICommand m_SaveTemplateCommand;

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
        /// <value>
        /// The load file command.
        /// </value>
        public ICommand LoadFileCommand { get { return m_LoadFileCommand; } }
        public ICommand SaveTemplateCommand { get { return m_SaveTemplateCommand; } }

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
            m_SaveTemplateCommand = new RelayCommand(x => SaveTemplate()); // Ugly.
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
