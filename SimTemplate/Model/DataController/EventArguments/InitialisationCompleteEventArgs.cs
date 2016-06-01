using SimTemplate.ViewModel.DataControllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimTemplate.ViewModel.DataControllers.EventArguments
{
    public class InitialisationCompleteEventArgs : EventArgs
    {
        private readonly InitialisationResult m_Result;

        public InitialisationResult Result { get { return m_Result; } }

        public InitialisationCompleteEventArgs(InitialisationResult result)
        {
            m_Result = result;
        }
    }
}
