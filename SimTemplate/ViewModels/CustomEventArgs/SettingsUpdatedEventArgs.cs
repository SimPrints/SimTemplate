using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimTemplate.ViewModels.CustomEventArgs
{
    public class SettingsUpdatedEventArgs : EventArgs
    {
        private readonly string m_ApiKey;

        public string ApiKey { get { return m_ApiKey; } }

        public SettingsUpdatedEventArgs(string apiKey)
        {
            m_ApiKey = apiKey;
        }
    }
}
