using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimTemplate.Helpers.GoogleApis
{
    public class GetClientCompleteEventArgs : EventArgs
    {
        private IConfigurableHttpClient m_Client;

        public IConfigurableHttpClient Client { get{ return m_Client; } }

        public GetClientCompleteEventArgs(IConfigurableHttpClient client)
        {
            m_Client = client;
        }
    }
}
