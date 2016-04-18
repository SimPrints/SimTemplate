using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemplateBuilder.Helpers;
using TemplateBuilder.ViewModel.MainWindow;

namespace TemplateBuilder.ViewModel.MainWindow
{
    public partial class TemplateBuilderViewModel
    {
        public abstract class AbstractAsyncTransitionalState<T> : Initialised where T : EventArgs
        {
            private object m_Identifier;

            public AbstractAsyncTransitionalState(TemplateBuilderViewModel outer) : base(outer)
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
                if (m_Identifier == id)
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
