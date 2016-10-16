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
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using SimTemplate.Utilities;
using SimTemplate.ViewModels;

namespace SimTemplate.StateMachine
{
    public class StateManager<T> where T : State
    {
        private static readonly ILog m_Log = LogManager.GetLogger(typeof(StateManager<T>));

        private ViewModel m_ViewModel;
        private T m_CurrentState;
        private object m_TransitionLock = new object();
        private readonly IDictionary<Type, T> m_States;

        public StateManager(ViewModel viewModel, Type initialStateType)
        {
            IntegrityCheck.IsNotNull(viewModel);

            m_ViewModel = viewModel;

            // Instantiate all concrete states into a list for transitioning to.
            m_States = new Dictionary<Type, T>();
            foreach (Type type in Assembly.GetAssembly(typeof(T)).GetTypes()
                .Where(myType =>
                myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(T))))
            {
                m_States.Add(type, (T)Activator.CreateInstance(type, m_ViewModel));
            }

            // Transition to the initial state.
            TransitionTo(initialStateType);
        }

        /// <summary>
        /// Gets the current state.
        /// </summary>
        /// <value>
        /// The state.
        /// </value>
        public T State { get { return m_CurrentState; } }

        /// <summary>
        /// Transitions to a new state, executing transition actions.
        /// </summary>
        /// <param name="stateType">Type of the state.</param>
        public void TransitionTo(Type stateType)
        {
            T newState = ToState(stateType);

            if (m_CurrentState != null)
            {
                m_Log.InfoFormat("State transition: {0}->{1}", m_CurrentState.Name, newState.Name);
                m_CurrentState.OnLeavingState();
            }
            else
            {
                // This is our first transition.
                m_Log.InfoFormat("State transition: [null]->{0}", newState.Name);
            }

            lock (m_TransitionLock)
            {
                m_CurrentState = newState;
            }
            newState.OnEnteringState();
        }

        private T ToState(Type stateType)
        {
            IntegrityCheck.IsTrue(
                typeof(T).IsAssignableFrom(stateType),
                "Supplied type doesn't derive from {0}", typeof(T).Name);
            IntegrityCheck.IsFalse(
                stateType.IsAbstract,
                "Cannot transition to abstract state {0}", stateType.Name);

            T state;
            bool isFound = m_States.TryGetValue(stateType, out state);

            IntegrityCheck.IsTrue(isFound, "State {0} not found in m_States", stateType.Name);
            IntegrityCheck.IsNotNull(
                state,
                "State {0} found in m_States, but value was null",
                stateType.Name);

            return state;
        }
    }
}
