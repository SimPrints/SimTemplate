using Microsoft.Win32;
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
    abstract public class BaseState : State
    {
        public BaseState(TemplateBuilderViewModel outer, StateManager stateMgr) : base(outer, stateMgr)
        { }

        public override void LoadFile()
        {
            OpenFileDialog od = new OpenFileDialog();
            //od.Filter = "XLS files|*.xls";
            //od.Multiselect = true;
            if (od.ShowDialog() == true)
            {
                // TODO: only pass valid file types.

                // Record image and filename
                m_Outer.ImageFileName = od.FileName;
                m_Outer.Image = new BitmapImage(new Uri(od.FileName));

                // Transition state to 'Templating'
                m_StateMgr.TransitionTo(typeof(WaitLocation));
            }
        }
        public override void image_SizeChanged(Size newSize)
        {
            IntegrityCheck.IsNotNull(m_Outer.Image);
            // Get scaling in each dimension.
            double scaleX = newSize.Width / m_Outer.Image.Width;
            double scaleY = newSize.Height / m_Outer.Image.Height;
            // Check that scaling factor is equal for each dimension.
            Vector scale = new Vector(scaleX, scaleY);
            // Update ViewModel scale
            m_Outer.Scale = scale;
        }
    }
}
