using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SimTemplate.Helpers;
using SimTemplate.Model.DataControllerEventArgs;
using RestSharp;

namespace SimTemplate.Model.DataControllers
{
    public class TempApiDataController : DataController
    {
        #region Constants

        private const string API_KEY_RESOURCE = "api/ApiKey";
        private const string HUMAN_TEMPLATE_RESOURCE = "api/HumanTemplate";

        private const string API_KEY = "c9c9c40b-8444-42e1-9c19-6fc48cd5309b";

        #endregion

        private RestClient m_RestClient;
        private string m_Token;

        public override void BeginInitialise(DataControllerConfig config)
        {
            base.BeginInitialise(config);

            // Create client
            m_RestClient = new RestClient(Config.UrlRoot);

            // Request a token
            RestRequest request = new RestRequest(API_KEY_RESOURCE, Method.GET);
            request.AddParameter("apiKey", API_KEY);

            m_RestClient.ExecuteAsync(request, response =>
            {
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    // TODO: Catch deserializing errors
                    m_Token = JsonConvert.DeserializeObject<ApiKeyResponse>(response.Content).token;
                    
                    OnInitialisationComplete(new InitialisationCompleteEventArgs(InitialisationResult.Initialised));
                }
                else
                {
                    OnInitialisationComplete(new InitialisationCompleteEventArgs(InitialisationResult.Error));
                }
            });
        }

        protected override void StartCaptureTask(ScannerType scannerType, bool isTemplated, Guid guid, CancellationToken token)
        {
            RestRequest request = new RestRequest(HUMAN_TEMPLATE_RESOURCE, Method.GET);
            request.AddParameter("token", m_Token, ParameterType.QueryString);

            m_RestClient.ExecuteAsync(request, response =>
            {
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    HumanTemplateResponse templateResponse =
                        JsonConvert.DeserializeObject<HumanTemplateResponse>(response.Content);

                    CaptureInfo capture = new CaptureInfo(
                        templateResponse.Id,
                        Convert.FromBase64String(templateResponse.Base64Image),
                        null);

                    // Raise GetCaptureComplete event.
                    OnGetCaptureComplete(new GetCaptureCompleteEventArgs(capture, guid, DataRequestResult.Success));
                }
                else
                {
                    OnGetCaptureComplete(new GetCaptureCompleteEventArgs(null, guid, DataRequestResult.Failed));
                }
            });
        }

        protected override void StartSaveTask(long dbId, byte[] template, Guid guid, CancellationToken token)
        {
            RestRequest request = new RestRequest(HUMAN_TEMPLATE_RESOURCE, Method.POST);
        }
    }
}
