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
using log4net;
using System;
using System.Runtime.CompilerServices;
using SimTemplate.Utilities;
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
