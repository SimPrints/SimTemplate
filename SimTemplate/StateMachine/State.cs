using log4net;
using System;
using System.Runtime.CompilerServices;
using SimTemplate.Helpers;
using SimTemplate.ViewModels;

namespace SimTemplate.StateMachine
{
    public abstract class State : LoggingClass
    {
        private ViewModel m_Outer;

        #region Constructor

        public State(ViewModel outer)
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
        protected ViewModel BaseOuter { get { return m_Outer; } }

        protected void MethodNotImplemented([CallerMemberName] string caller = null)
        {
            throw new NotImplementedException(
                String.Format("Method {0} not implemented in state {1}",
                caller,
                Name));
        }
    }
}
