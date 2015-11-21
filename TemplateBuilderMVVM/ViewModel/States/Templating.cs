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
    abstract public class Templating : Initialised
    {
        #region Constructor

        public Templating(TemplateBuilderViewModel outer, StateManager stateMgr) : base(outer, stateMgr)
        { }

        public override void OnEnteringState()
        {
            base.OnEnteringState();

            // Ensure UI controls active
            if (!m_Outer.IsSaveTemplatePermitted)
            {
                m_Outer.IsSaveTemplatePermitted = true;
            }
            if (!m_Outer.IsInputMinutiaTypePermitted)
            {
                m_Outer.IsInputMinutiaTypePermitted = true;
            }
        }

        #endregion
    }
}
