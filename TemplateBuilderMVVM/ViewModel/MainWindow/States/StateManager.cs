using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemplateBuilder.Helpers;

namespace TemplateBuilder.ViewModel.MainWindow.States
{
    public class StateManager
    {
        private static readonly ILog m_Log = LogManager.GetLogger(typeof(StateManager));

        private TemplateBuilderViewModel m_ViewModel;
        private State m_State;
        private bool m_IsStarted;

        public StateManager(TemplateBuilderViewModel viewModel, Type initialState)
        {

            m_ViewModel = viewModel;
            m_State = ToState(initialState);
        }

        /// <summary>
        /// Gets the current state.
        /// </summary>
        /// <value>
        /// The state.
        /// </value>
        public State State { get { return m_State; } }

        public void Start()
        {
            IntegrityCheck.IsFalse(m_IsStarted);

            m_State.OnEnteringState();
            m_IsStarted = true;
        }

        /// <summary>
        /// Transitions to a new state, executing transition actions.
        /// </summary>
        /// <param name="stateType">Type of the state.</param>
        public void TransitionTo(Type stateType) 
        {
            State newState = ToState(stateType);

            m_Log.InfoFormat("State transition: {0}->{1}", m_State.Name, newState.Name);
            m_State.OnLeavingState();
            m_State = newState;
            newState.OnEnteringState();
        }

        private State ToState(Type stateType)
        {
            IntegrityCheck.IsTrue(typeof(State).IsAssignableFrom(stateType),
                "Supplied type not a recognised state");
            return (State)Activator.CreateInstance(stateType, m_ViewModel, this);
        }
    }
}
