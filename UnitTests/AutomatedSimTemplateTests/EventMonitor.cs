using SimTemplate.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SimTemplate
{
    public class EventMonitor
    {
        private readonly IDictionary<string, IList<EventArgs>> m_EventResponses;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventMonitor"/> class.
        /// </summary>
        public EventMonitor()
        {
            m_EventResponses = new Dictionary<string, IList<EventArgs>>();
        }

        /// <summary>
        /// Adds an object to be monitored for its events
        /// </summary>
        /// <param name="obj">The object.</param>
        public void AddMonitoredObject(object obj)
        {
            foreach (EventInfo eventInfo in obj.GetType().GetEvents())
            {
                // Create a new event response placeholder
                KeyValuePair<string, IList<EventArgs>> eventResponse =
                    new KeyValuePair<string, IList<EventArgs>>(eventInfo.Name, new List<EventArgs>());

                // Save record the new event response
                m_EventResponses.Add(eventResponse);

                // Add an event handler that saves the event args when fired
                AddEventHandler(
                    eventInfo,
                    obj,
                    (sender, args) => eventResponse.Value.Add(args));
            }
        }

        /// <summary>
        /// Gets the event responses for a given event.
        /// </summary>
        /// <typeparam name="T">The class to cast the event arguments to</typeparam>
        /// <param name="eventName">Name of the event.</param>
        /// <returns></returns>
        public IEnumerable<T> GetEventResponses<T>(string eventName) where T : EventArgs
        {
            IntegrityCheck.IsTrue(m_EventResponses.ContainsKey(eventName));
            return m_EventResponses[eventName].Select(x => (T)x);
        }

        public void Reset()
        {
            foreach (KeyValuePair<string, IList<EventArgs>> eventResponse in m_EventResponses)
            {
                // Clear the existing responses from the monitor
                eventResponse.Value.Clear();
            }
        }

        #region Helper Methods

        /// <summary>
        /// Adds the event handler.
        /// </summary>
        /// <param name="eventInfo">The event to listen for.</param>
        /// <param name="item">The sender?.</param>
        /// <param name="action">The action to execute upon detection of the event.</param>
        private static void AddEventHandler(EventInfo eventInfo, object item, Action<object, EventArgs> action)
        {
            var parameters = eventInfo.EventHandlerType
              .GetMethod("Invoke")
              .GetParameters()
              .Select(parameter => Expression.Parameter(parameter.ParameterType))
              .ToArray();

            var invoke = action.GetType().GetMethod("Invoke");

            var handler = Expression.Lambda(
                eventInfo.EventHandlerType,
                Expression.Call(Expression.Constant(action), invoke, parameters[0], parameters[1]),
                parameters
              )
              .Compile();

            eventInfo.AddEventHandler(item, handler);
        }

        #endregion

    }
}
