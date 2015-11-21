using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using TemplateBuilderMVVM.Helpers;

namespace TemplateBuilderMVVM.ViewModel.States
{
    public class LoadingFile : Initialised
    {
        public LoadingFile(TemplateBuilderViewModel outer, StateManager stateMgr)
            : base(outer, stateMgr)
        { }

        public override void OnEnteringState()
        {
            base.OnEnteringState();

            // We should only enter this state if there is definitely a file to load.
            IntegrityCheck.IsNotNull(m_Outer.ImageFileNames);
            IntegrityCheck.IsNotNull(m_Outer.ImageFileNames.Current);

            LoadFile(m_Outer.ImageFileNames.Current);

            // Now transition to templating
            m_StateMgr.TransitionTo(typeof(WaitLocation));
        }

        public override void OnLeavingState()
        {
            base.OnLeavingState();

            // Ensure that the state has set the correct conditions.
            IntegrityCheck.AreEqual(0, m_Outer.Minutae.Count());
        }

        public override void PositionInput(Point point)
        {
            // Do nothing.
        }

        public override void PositionMove(Point point)
        {
            // Do nothing.
        }

        public override void RemoveItem(int index)
        {
            // Do nothing.
        }

        public override void SaveTemplate()
        {
            // Do nothing.
        }

        public override void OpenFile()
        {
            // Do not allow opening a file while loading a file!
        }

        public override void OpenFolder()
        {
            // Do not allow opening a folder while loading a file!
        }

        #region Private Methods

        private void LoadFile(string filename)
        {
            bool isLoaded = false;
            BitmapImage image = null;
            try
            {
                image = new BitmapImage(new Uri(filename));
                isLoaded = true;
            }
            catch (NotSupportedException ex)
            {
                // Unable to load file.
                Console.WriteLine("Unable to load file:" + ex, ex.Message);
                // Transition to Idle, as we cannot load the file.
                m_StateMgr.TransitionTo(typeof(Idle));
            }

            if (isLoaded)
            {
                m_Outer.ImageFileName = filename;
                m_Outer.Image = image;
            }
        }

        #endregion
    }
}
