using Google.Apis.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SimTemplate.Helpers.GoogleApis
{
    /// <summary>
    /// Wrapper for the ConfigurableHttpClient
    /// </summary>
    public class GoogleAuthenticator : IAuthenticationClient
    {
        private ConfigurableHttpClient m_Client;

        public GoogleAuthenticator(ConfigurableHttpClient client)
        {
            m_Client = client;
        }

        public Task<string> GetStringAsync(string requestUri)
        {
            return m_Client.GetStringAsync(requestUri);
        }

        public Task<string> GetStringAsync(Uri requestUri)
        {
            return m_Client.GetStringAsync(requestUri);
        }

        public Task<HttpResponseMessage> PostAsync(string requestUri, HttpContent content)
        {
            return m_Client.PostAsync(requestUri, content);
        }

        public Task<HttpResponseMessage> PostAsync(Uri requestUri, HttpContent content)
        {
            return m_Client.PostAsync(requestUri, content);
        }

        public Task<HttpResponseMessage> PostAsync(string requestUri, HttpContent content, CancellationToken cancellationToken)
        {
            return m_Client.PostAsync(requestUri, content, cancellationToken);
        }

        public Task<HttpResponseMessage> PostAsync(Uri requestUri, HttpContent content, CancellationToken cancellationToken)
        {
            return m_Client.PostAsync(requestUri, content, cancellationToken);
        }

        public Task<HttpResponseMessage> PutAsync(Uri requestUri, HttpContent content)
        {
            return m_Client.PostAsync(requestUri, content);
        }
    }
}
