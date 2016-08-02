using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimTemplate.Utilities;
using SimTemplate.DataTypes.Enums;

namespace SimTemplate.ViewModels
{
    public partial class MainWindowViewModel
    {
        public abstract class TransitioningAsync<T> : MainWindowState where T : EventArgs
        {
            private object m_Identifier;

            public TransitioningAsync(MainWindowViewModel outer, Activity stateActivity) :
                base(outer, stateActivity)
            { }

            #region Overriden Public Methods

            public override void OnEnteringState()
            {
                base.OnEnteringState();

                m_Identifier = StartAsyncOperation();
                IntegrityCheck.IsNotNull(m_Identifier);
            }

            #endregion

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

            #region Abstract Methods

            protected abstract object StartAsyncOperation();

            protected abstract void AbortAsyncOperation(object identifier);

            protected abstract void OnOperationComplete(T e);

            #endregion
        }
    }
}
