using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimTemplate.Helpers;

namespace SimTemplate.ViewModels
{
    public partial class MainWindowViewModel
    {
        public abstract class TransitioningAsync<T> : Initialised where T : EventArgs
        {
            private object m_Identifier;

            public TransitioningAsync(MainWindowViewModel outer) : base(outer)
            { }

            public override void OnEnteringState()
            {
                base.OnEnteringState();

                m_Identifier = StartAsyncOperation();
                IntegrityCheck.IsNotNull(m_Identifier);
            }

            protected object Identifier { get { return m_Identifier; } }

            protected void CheckCompleteAndContinue(object id, T e)
            {
                if (m_Identifier.Equals(id))
                {
                    OnOperationComplete(e);
                }
                else
                {
                    Log.WarnFormat("Completed operation did not match identifier. Ignoring");
                }
            }

            protected abstract object StartAsyncOperation();

            protected abstract void OnOperationComplete(T e);
        }
    }
}
