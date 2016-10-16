// Copyright 2016 Sam Briggs
//
// This file is part of SimTemplate.
//
// SimTemplate is free software: you can redistribute it and/or modify it under the
// terms of the GNU General Public License as published by the Free Software 
// Foundation, either version 3 of the License, or (at your option) any later
// version.
//
// SimTemplate is distributed in the hope that it will be useful, but WITHOUT ANY 
// WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR 
// A PARTICULAR PURPOSE. See the GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License along with
// SimTemplate. If not, see http://www.gnu.org/licenses/.
//
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
            private readonly string m_PromptText;

            public TransitioningAsync(
                MainWindowViewModel outer,
                Activity stateActivity,
                string promptText) :
                base(outer, stateActivity)
            {
                m_PromptText = promptText;
            }

            #region Overriden Public Methods

            public override void OnEnteringState()
            {
                base.OnEnteringState();

                Outer.PromptText = m_PromptText;

                m_Identifier = StartAsyncOperation();
                IntegrityCheck.IsNotNull(m_Identifier);
            }

            public override void OnLeavingState()
            {
                base.OnLeavingState();

                Outer.PromptText = String.Empty;
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
