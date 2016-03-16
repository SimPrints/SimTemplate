using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TemplateBuilder.Helpers;
using TemplateBuilder.ViewModel;

namespace TemplateBuilder.StateMachine
{
    public class StateManager<T> where T : BaseState
    {
        private static readonly ILog m_Log = LogManager.GetLogger(typeof(StateManager<T>));

        private BaseViewModel m_ViewModel;
        private T m_CurrentState;
        private bool m_IsStarted;
        private object m_StateLock = new object();
        private readonly IDictionary<Type, T> m_States;

        public StateManager(BaseViewModel viewModel)
        {
            IntegrityCheck.IsNotNull(viewModel);

            m_ViewModel = viewModel;

            // Instantiate all concrete states into a list for transitioning to.
            m_States = new Dictionary<Type, T>();
            foreach (Type type in
                Assembly.GetAssembly(typeof(T)).GetTypes()
                .Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(T))))
            {
                m_States.Add(type, (T)Activator.CreateInstance(type, m_ViewModel));
            }
        }

        /// <summary>
        /// Gets the current state.
        /// </summary>
        /// <value>
        /// The state.
        /// </value>
        public T State { get { return m_CurrentState; } }

        public void Start(Type initialState)
        {
            IntegrityCheck.IsFalse(m_IsStarted);
            IntegrityCheck.IsNotNull(initialState);

            TransitionTo(initialState);
            m_IsStarted = true;
        }

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

            lock (m_StateLock)
            {
                m_CurrentState = newState;
            }
            newState.OnEnteringState();
        }

        private T ToState(Type stateType)
        {
            IntegrityCheck.IsTrue(
                typeof(T).IsAssignableFrom(stateType),
                "Supplied type not a recognised state");

            T state;
            bool isFound = m_States.TryGetValue(stateType, out state);

            IntegrityCheck.IsTrue(isFound, "State {0} not found in m_States", stateType);
            IntegrityCheck.IsNotNull(
                state,
                "State {0} found in m_States, but value was null",
                stateType);

            return state;
        }
    }
}
