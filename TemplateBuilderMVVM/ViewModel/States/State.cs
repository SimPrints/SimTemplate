using log4net;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using TemplateBuilder.Helpers;
using TemplateBuilder.Model;

namespace TemplateBuilder.ViewModel.States
{
    public abstract class State
    {
        private ILog m_Log;

        protected TemplateBuilderViewModel Outer;
        protected StateManager m_StateMgr;

        #region Constructor

        public State(TemplateBuilderViewModel outer, StateManager stateMgr)
        {
            Outer = outer;
            m_StateMgr = stateMgr;
        }

        #endregion

        #region Virtual Methods

        public virtual void OnEnteringState() { }

        public virtual void OnLeavingState() { }

        public virtual bool IsMinutiaTypeButtonsEnabled { get { return false; } }

        #endregion

        #region Abstract Methods

        public abstract void OpenFile();

        public abstract void PositionInput(Point point);

        public abstract void PositionMove(Point point);

        public abstract void RemoveItem(int index);

        public abstract void SaveTemplate();

        public abstract void image_SizeChanged(Size newSize);

        public abstract void SetMinutiaType(MinutiaType type);

        public abstract void EscapeAction();

        #endregion

        public string Name { get { return GetType().Name; } }

        protected void OnErrorOccurred(TemplateBuilderException ex)
        {
            Outer.Exception = ex;
            Logger.ErrorFormat("Error occurred: " + ex.Message, ex);
            m_StateMgr.TransitionTo(typeof(Error));
        }

        protected ILog Logger
        {
            get
            {
                if (m_Log == null)
                {
                    m_Log = LogManager.GetLogger(this.GetType());
                }
                return m_Log;
            }
        }
    }
}
