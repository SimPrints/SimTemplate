using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimTemplate.ViewModel.DataControllers
{
    public class DataControllerConfig
    {
        private readonly string m_ApiKey;
        private readonly string m_UrlRoot;

        public string ApiKey { get { return m_ApiKey; } }
        public string UrlRoot { get { return m_UrlRoot; } }

        public DataControllerConfig(string apiKey, string urlRoot)
        {
            m_ApiKey = apiKey;
            m_UrlRoot = urlRoot;
        }
    }
}
