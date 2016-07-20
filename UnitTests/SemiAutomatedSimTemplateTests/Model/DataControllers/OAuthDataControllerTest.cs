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
using SimTemplate.Model.DataControllers.EventArguments;
using SimTemplate.Model.DataControllers;
using SimTemplate.Model.DataControllers.OAuth;

namespace AutomatedSimTemplateTests.Model
{
    [TestClass]
    public class OAuthDataControllerTest
    {
        private static readonly ILog m_Log = LogManager.GetLogger(typeof(OAuthDataControllerTest));

        private IDataController m_Controller;

        private IList<InitialisationCompleteEventArgs> m_InitialisationComplete;
        private IList<GetCaptureCompleteEventArgs> m_GetCaptureCompleteEvents;

        [TestInitialize]
        public void Setup()
        {
            m_InitialisationComplete = new List<InitialisationCompleteEventArgs>();
            m_GetCaptureCompleteEvents = new List<GetCaptureCompleteEventArgs>();

            // Instantiate our controller for testing
            m_Controller = new OAuthDataController(new ConfigurableHttpClientFactory());
            m_Controller.InitialisationComplete +=
                (object o, InitialisationCompleteEventArgs e) => m_InitialisationComplete.Add(e);
            m_Controller.GetCaptureComplete +=
                (object o, GetCaptureCompleteEventArgs e) => m_GetCaptureCompleteEvents.Add(e);
        }

        [TestMethod]
        public void TestBeginInitialise_Success()
        {
            m_Controller.BeginInitialise(new DataControllerConfig("as", "sd"));
        }
    }
}
