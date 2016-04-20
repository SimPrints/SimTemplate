using Google.Apis.Auth.OAuth2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimTemplate.Helpers.GoogleApis
{
    public interface IConfigurableHttpClientFactory
    {
        void BeginGetClient(ClientSecrets secrets, IEnumerable<string> scopes, string user);

        event EventHandler<GetClientCompleteEventArgs> GetClientComplete;
    }
}
