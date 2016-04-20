using log4net;
using System;
using System.Runtime.CompilerServices;
using SimTemplate.Helpers;
using SimTemplate.ViewModel;

namespace SimTemplate.StateMachine
{
    public abstract class BaseState
    {
        private ILog m_Log;
        private BaseViewModel m_Outer;

        #region Constructor

        public BaseState(BaseViewModel outer)
        {
            IntegrityCheck.IsNotNull(outer);
            m_Outer = outer;
        }

        #endregion

        #region Virtual Methods

        public virtual void OnEnteringState() { }

        public virtual void OnLeavingState() { }

        #endregion

        public string Name { get { return GetType().Name; } }

        /// <summary>
        /// Gets the outer class that this state is behaviour for.
        /// </summary>
        protected BaseViewModel BaseOuter { get { return m_Outer; } }

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

        protected void MethodNotImplemented([CallerMemberName] string caller = null)
        {
            throw new NotImplementedException(
                String.Format("Method {0} not implemented in state {1}",
                caller,
                Name));
        }
    }
}
