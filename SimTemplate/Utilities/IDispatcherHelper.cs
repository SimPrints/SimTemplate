using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimTemplate.Utilities
{
    /// <summary>
    /// Interface for invoking actions on the dispatcher thread.
    /// This is introduced to break the dependency of the application on the 
    /// </summary>
    public interface IDispatcherHelper
    {
        void Invoke(Action action);
    }
}
