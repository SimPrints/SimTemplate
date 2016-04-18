using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using System.Threading;
using log4net;
using Google.Apis.Http;
using System.Net.Http;

namespace TemplateBuilder.Helpers.GoogleApis
{
    public class ConfigurableHttpClientFactory : IConfigurableHttpClientFactory
    {
        private static ILog m_Log = LogManager.GetLogger(typeof(ConfigurableHttpClientFactory));

        private event EventHandler<GetClientCompleteEventArgs> m_GetClientComplete;

        public void BeginGetClient(ClientSecrets secrets, IEnumerable<string> scopes, string user)
        {
            // Perform authorization
            Task<UserCredential> authorizeTask = GoogleWebAuthorizationBroker.AuthorizeAsync(
                secrets,
                scopes,
                user,
                CancellationToken.None);

            ConfigurableMessageHandler handler = new ConfigurableMessageHandler(new HttpClientHandler());
            ConfigurableHttpClient client = new ConfigurableHttpClient(handler);

            // Raise InitializeComplete event when task complete
            authorizeTask.ContinueWith((Task<UserCredential> aT) =>
            {
                IntegrityCheck.IsFalse(aT.IsCanceled, "Initialization was not passed a cancellation token.");
                if (!aT.IsFaulted)
                {
                    // Initialize the client handler
                    aT.Result.Initialize(client);
                    OnGetClientComplete(new GetClientCompleteEventArgs((IConfigurableHttpClient)client));
                }
                else
                {
                    // Async exception
                    m_Log.Error("Async error during Initialization", aT.Exception);
                    OnGetClientComplete(new GetClientCompleteEventArgs(null));
                }
            });
        }

        public event EventHandler<GetClientCompleteEventArgs> GetClientComplete
        {
            add { m_GetClientComplete += value; }
            remove { m_GetClientComplete += value; }
        }

        private void OnGetClientComplete(GetClientCompleteEventArgs e)
        {
            EventHandler<GetClientCompleteEventArgs> temp = m_GetClientComplete;
            if (temp != null)
            {
                temp.Invoke(this, e);
            }
        }
    }
}
