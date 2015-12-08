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
        private State m_CurrentState;
        private bool m_IsStarted;

        private readonly IDictionary<Type, State> m_States;

        public StateManager(TemplateBuilderViewModel viewModel, Type initialState)
        {
            IntegrityCheck.IsNotNull(viewModel);
            IntegrityCheck.IsNotNull(initialState);

            m_ViewModel = viewModel;
            // Initialise one of each state.
            m_States = new Dictionary<Type, State>()
            {
                {typeof(Initialising), new Initialising(m_ViewModel, this)},
                {typeof(Idle), new Idle(m_ViewModel, this)},
                {typeof(WaitDirection), new WaitDirection(m_ViewModel, this)},
                {typeof(WaitLocation), new WaitLocation(m_ViewModel, this)},
                {typeof(MovingMinutia), new MovingMinutia(m_ViewModel, this)},
                {typeof(Error), new Error(m_ViewModel, this)},
            };
            m_CurrentState = ToState(initialState);
        }

        /// <summary>
        /// Gets the current state.
        /// </summary>
        /// <value>
        /// The state.
        /// </value>
        public State State { get { return m_CurrentState; } }

        public void Start()
        {
            IntegrityCheck.IsFalse(m_IsStarted);

            m_CurrentState.OnEnteringState();
            m_IsStarted = true;
        }

        /// <summary>
        /// Transitions to a new state, executing transition actions.
        /// </summary>
        /// <param name="stateType">Type of the state.</param>
        public void TransitionTo(Type stateType) 
        {
            State newState = ToState(stateType);

            m_Log.InfoFormat("State transition: {0}->{1}", m_CurrentState.Name, newState.Name);
            m_CurrentState.OnLeavingState();
            m_CurrentState = newState;
            newState.OnEnteringState();
        }

        private State ToState(Type stateType)
        {
            IntegrityCheck.IsTrue(typeof(State).IsAssignableFrom(stateType),
                "Supplied type not a recognised state");
            State state;
            bool isFound = m_States.TryGetValue(stateType, out state);
            if (!isFound)
            {
                throw new TemplateBuilderException(
                    String.Format("State {0} not found in m_States", stateType));
            }
            return state;
        }
    }
}
