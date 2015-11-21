using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using TemplateBuilderMVVM.Helpers;

namespace TemplateBuilderMVVM.ViewModel.States
{
    abstract public class Initialised : State
    {
        public Initialised(TemplateBuilderViewModel outer, StateManager stateMgr) : base(outer, stateMgr)
        { }

        //IEnumerable<string> m_ValidFileExtensions = new List<string>()
        //{
        //    "jpg",
        //    "jpeg",
        //    "bmp",
        //    "png",
        //};

        public override void OpenFile()
        {
            Microsoft.Win32.OpenFileDialog od = new Microsoft.Win32.OpenFileDialog();
            //od.Filter = "XLS files|*.xls";
            //od.Multiselect = true;
            if (od.ShowDialog() == true)
            {
                // TODO: only pass valid file types.

                // Record image and filename
                string[] files = { od.FileName };
                m_Outer.ImageFileNames = files.ToList().GetEnumerator();

                // Transition to LoadFile if file is legit.
                ChoiceLoadFile();
            }
        }

        public override void OpenFolder()
        {
            // Open a folder browsing dialog for the user to select a folder.
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            DialogResult result = fbd.ShowDialog();

            if (result == DialogResult.OK)
            {
                // Obtain the contained files.
                string[] files = Directory.GetFiles(fbd.SelectedPath);

                // Filter for only valid file types
                string[] validFiles = files;
                //IEnumerable<string> validFiles = from file in files
                //                                 where IsValidFile(file)
                //                                 select file;

                m_Outer.ImageFileNames = files.ToList().GetEnumerator();

                // Transition to LoadFile if file is legit.
                ChoiceLoadFile();
            }
        }

        public override void image_SizeChanged(Size newSize)
        {
            if (m_Outer.Image != null)
            {
                // Image has been resized.
                // Get scaling in each dimension.
                double scaleX = newSize.Width / m_Outer.Image.Width;
                double scaleY = newSize.Height / m_Outer.Image.Height;
                // Check that scaling factor is equal for each dimension.
                Vector scale = new Vector(scaleX, scaleY);
                // Update ViewModel scale
                m_Outer.Scale = scale;
            }
            else
            {
                // Image has been removed.
                IntegrityCheck.AreEqual(0, newSize.Height);
                IntegrityCheck.AreEqual(0, newSize.Width);
            }
        }

        protected void ChoiceLoadFile()
        {
            if (m_Outer.ImageFileNames != null)
            {
                if (m_Outer.ImageFileNames.MoveNext())
                {
                    // If there is a file to load, we shouldn't hang around in Idle
                    // Transition to LoadingFile state.
                    m_StateMgr.TransitionTo(typeof(LoadingFile));
                }
                else
                {
                    // No file to load, but ImageFileNames exists, so remove it.
                    // (we have probably iterated all the elements)
                    m_Outer.ImageFileNames = null;
                }
            }
        }

        //private bool IsValidFile(string filename)
        //{
        //    string ext = Path.GetExtension(filename);
        //    return m_ValidFileExtensions.Any(x => x == ext);
        //}
    }
}
