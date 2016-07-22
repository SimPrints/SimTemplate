using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimTemplate.Helpers.GoogleApis
{
    public class GetClientCompleteEventArgs : EventArgs
    {
        private IAuthenticationClient m_Client;

        public IAuthenticationClient Client { get{ return m_Client; } }

        public GetClientCompleteEventArgs(IAuthenticationClient client)
        {
            m_Client = client;
        }
    }
}
