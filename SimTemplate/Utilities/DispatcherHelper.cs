using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimTemplate.Utilities
{
    /// <summary>
    /// A helper class that wraps App.Current.Dispatcher to execute actions
    /// that affect UI elements on the application thread.
    /// </summary>
    /// <seealso cref="SimTemplate.Utilities.IDispatcherHelper" />
    public class DispatcherHelper : IDispatcherHelper
    {
        /// <summary>
        /// Invokes the specified action.
        /// </summary>
        /// <param name="action">The action.</param>
        void IDispatcherHelper.Invoke(Action action)
        {
            App.Current.Dispatcher.Invoke(action);
        }
    }
}
