using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using TemplateBuilderMVVM.Helpers;
using TemplateBuilderMVVM.Model;

namespace TemplateBuilderMVVM.ViewModel.States
{
    abstract public class Templating : BaseState
    {
        #region Constructor

        public Templating(TemplateBuilderViewModel outer, StateManager stateMgr) : base(outer, stateMgr)
        { }

        public override void image_SizeChanged(Size newSize)
        {
            IntegrityCheck.IsNotNull(m_Outer.Image);
            // Get scaling in each dimension.
            double scaleX = newSize.Width / m_Outer.Image.Width;
            double scaleY = newSize.Height / m_Outer.Image.Height;
            // Check that scaling factor is equal for each dimension.
            IntegrityCheck.AreEqual(scaleX, scaleY);
            double scale = (scaleX + scaleY) / 2;
            // Update ViewModel scale
            m_Outer.Scale = scale;
        }

        #endregion
    }
}
