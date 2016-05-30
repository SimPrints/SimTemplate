using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SimTemplate.Helpers.GoogleApis;
using Google.Apis.Auth.OAuth2;

namespace SemiAutomatedSimTemplateTests.Helpers.GoogleApis
{
    [TestClass]
    public class GoogleAuthenticatorTest
    {
        private IList<GetClientCompleteEventArgs> m_GetClientCompleteEvents;
        private IList<AutoResetEvent> m_GetClientCompleteCounter;

        private ConfigurableHttpClientFactory m_Factory;

        #region Constants

        // TODO: Secret in environment variable or json file (FileStream)
        private static readonly ClientSecrets CLIENT_SECRETS = new ClientSecrets
        {
            ClientId = "916016530-chmjlljhh1a94fti71h2d9nd1denc7s5.apps.googleusercontent.com",
            ClientSecret = "Ytg35Ulbfn8WtItj2xUWB5zW",
        };
        private static readonly IEnumerable<string> SCOPES = new string[]
        {
            "openid",
            "profile",
            "email",
        };

        #endregion

        [TestInitialize]
        public void Setup()
        {
            m_GetClientCompleteEvents = new List<GetClientCompleteEventArgs>();
            m_GetClientCompleteCounter = new List<AutoResetEvent>();

            m_Factory = new ConfigurableHttpClientFactory();
        }

        [TestMethod]
        public void TestBeginGetClient_Success()
        {
            // PREPARE:
            // Prepare to wait till we complete getting the client 
            m_GetClientCompleteCounter.Add(new AutoResetEvent(false));
            m_Factory.GetClientComplete += (object sender, GetClientCompleteEventArgs e) =>
            {
                m_GetClientCompleteEvents.Add(e);
                int index = m_GetClientCompleteEvents.Count() - 1;
                if (index < m_GetClientCompleteCounter.Count())
                {
                    m_GetClientCompleteCounter[index].Set();
                }
            };

            // RUN:
            m_Factory.BeginGetClient(
                CLIENT_SECRETS,
                SCOPES,
                "sjbriggs14@g.com");
            WaitHandle.WaitAll(m_GetClientCompleteCounter.ToArray(), TimeSpan.FromMinutes(2), false);

            // ASSERT:
            Assert.AreEqual(1, m_GetClientCompleteEvents.Count());
            Assert.IsNotNull(m_GetClientCompleteEvents[0].Client);

            // PREPARE:
            IAuthenticationClient client = m_GetClientCompleteEvents[0].Client;

            // RUN:
            // Make request to validate the token
            Task<string> validateTask = client.GetStringAsync(
                String.Format("https://www.googleapis.com/oauth2/v3/tokeninfo?access_token={0}&id_token={1}",
                    client.Token.AccessToken, "samIam"));
            string response = validateTask.Result;

            // ASSERT:
            Assert.IsFalse(String.IsNullOrEmpty(response));
        }
    }
}
