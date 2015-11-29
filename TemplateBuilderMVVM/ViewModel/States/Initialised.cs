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
using TemplateBuilder.Helpers;

namespace TemplateBuilder.ViewModel.States
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

        #region Overriden Methods

        public override void OpenFile()
        {
            m_StateMgr.TransitionTo(typeof(Idle));
        }

        public override void image_SizeChanged(Size newSize)
        {
            //    if (Outer.Image != null)
            //    {
            //        // Image has been resized.
            //        // Get scaling in each dimension.
            //        double scaleX = newSize.Width / Outer.Image.Width;
            //        double scaleY = newSize.Height / Outer.Image.Height;
            //        // Check that scaling factor is equal for each dimension.
            //        Vector scale = new Vector(scaleX, scaleY);
            //        // Update ViewModel scale
            //        Outer.Scale = scale;
            //    }
            //    else
            //    {
            //        // Image has been removed.
            //        IntegrityCheck.AreEqual(0, newSize.Height);
            //        IntegrityCheck.AreEqual(0, newSize.Width);
            //     }
         }

        #endregion
    }
}
