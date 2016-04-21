using FakeItEasy;
using Google.Apis.Auth.OAuth2;
using log4net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimTemplate.Helpers.GoogleApis;
using SimTemplate.Model;
using SimTemplate.Model.Database;
using SimTemplate.Model.DataControllerEventArgs;
using SimTemplate.Model.DataControllers;

namespace AutomatedSimTemplateTests.Model
{
    [TestClass]
    public class OAuthDataControllerTest
    {
        private static readonly ILog m_Log = LogManager.GetLogger(typeof(OAuthDataControllerTest));

        private IAuthenticationClient m_Client;
        private IDataController m_Controller;

        private IList<InitialisationCompleteEventArgs> m_InitialisationComplete;
        private IList<GetCaptureCompleteEventArgs> m_GetCaptureCompleteEvents;

        [TestInitialize]
        public void Setup()
        {
            m_InitialisationComplete = new List<InitialisationCompleteEventArgs>();
            m_GetCaptureCompleteEvents = new List<GetCaptureCompleteEventArgs>();
            // Instantiate and configure a fake client to facilitate the tests
            m_Client = A.Fake<IAuthenticationClient>();
            // Create a fake client factory that will return our fake client
            IConfigurableHttpClientFactory clientFactory = A.Fake<IConfigurableHttpClientFactory>();
            A.CallTo(() => clientFactory.BeginGetClient(A<ClientSecrets>._, A<IEnumerable<string>>._, A<string>._))
                .Invokes(() =>
                {
                    clientFactory.GetClientComplete += Raise.With(new GetClientCompleteEventArgs(m_Client));
                });
            // Instantiate our controller for testing
            m_Controller = new OAuthDataController(clientFactory);
            m_Controller.InitialisationComplete +=
                (object o, InitialisationCompleteEventArgs e) => m_InitialisationComplete.Add(e);
            m_Controller.GetCaptureComplete +=
                (object o, GetCaptureCompleteEventArgs e) => m_GetCaptureCompleteEvents.Add(e);
        }

        [TestMethod]
        public void TestBeginInitialise_Success()
        {
            // Configure client to return good data
            string text;
            using (var streamReader = new StreamReader(@"Resources\good_capture.xml", Encoding.UTF8))
            {
                text = streamReader.ReadToEnd();
            }
            A.CallTo(() => m_Client.GetStringAsync(A<Uri>._))
                .Returns(Task.FromResult(text));

            // Call BeginInitialise to run the test
            m_Controller.BeginInitialise(new DataControllerConfig("asd", "asdf"));

            // Wait for the InitialisationComplete event
            Assert.AreEqual(1, m_InitialisationComplete.Count());
            Assert.AreEqual(InitialisationResult.Initialised, m_InitialisationComplete[0].Result);

            Guid requestId = m_Controller.BeginGetCapture(ScannerType.None, false);

            DateTime start = DateTime.Now;
            while (m_GetCaptureCompleteEvents.Count < 1)
            {
                if (DateTime.Now - start > TimeSpan.FromSeconds(1))
                {
                    break;
                }
            }

            Assert.AreEqual(1, m_GetCaptureCompleteEvents.Count());
            Assert.AreEqual(DataRequestResult.Success, m_GetCaptureCompleteEvents[0].Result);
            Assert.AreEqual(requestId, m_GetCaptureCompleteEvents[0].RequestId);
            Assert.IsNotNull(m_GetCaptureCompleteEvents[0].Capture);

            Assert.AreEqual(1, m_GetCaptureCompleteEvents[0].Capture.DbId);
            Assert.AreEqual("Ale-LES-0-1", m_GetCaptureCompleteEvents[0].Capture.HumanId);
            Assert.IsNotNull(m_GetCaptureCompleteEvents[0].Capture.ImageData);
            Assert.IsNull(m_GetCaptureCompleteEvents[0].Capture.TemplateData);
        }
    }
}
