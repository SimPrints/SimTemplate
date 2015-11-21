using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemplateBuilderMVVM.Helpers;

namespace TemplateBuilderMVVM.ViewModel.States
{
    public class StateManager
    {
        private TemplateBuilderViewModel m_ViewModel;
        private State m_State;

        public StateManager(TemplateBuilderViewModel viewModel)
        {
            m_ViewModel = viewModel;

            // Manually transition to the first state.
            m_State = new Uninitialised(m_ViewModel, this);
            m_State.OnEnteringState();
        }

        /// <summary>
        /// Gets the current state.
        /// </summary>
        /// <value>
        /// The state.
        /// </value>
        public State State { get { return m_State; } }

        /// <summary>
        /// Transitions to a new state, executing transition actions.
        /// </summary>
        /// <param name="stateType">Type of the state.</param>
        public void TransitionTo(Type stateType) 
        {
            IntegrityCheck.IsTrue(typeof(State).IsAssignableFrom(stateType),
                "Supplied type not a recognised state");

            State newState = (State)Activator.CreateInstance(stateType, m_ViewModel, this);
            Console.WriteLine("State transition: {0}->{1}", m_State.Name, newState.Name);
            m_State.OnLeavingState();
            m_State = newState;
            newState.OnEnteringState();
        }
    }
}
