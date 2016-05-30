using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SimTemplate.Helpers;
using SimTemplate.Model.DataControllers.EventArguments;
using RestSharp;
using log4net;

namespace SimTemplate.Model.DataControllers
{
    public class TempApiDataController : DataController
    {
        #region Constants

        private const string API_KEY_RESOURCE = "api/ApiKey";
        private const string HUMAN_TEMPLATE_RESOURCE = "api/HumanTemplate";

        #endregion

        private static readonly ILog m_Log = LogManager.GetLogger(typeof(TempApiDataController));

        private RestClient m_RestClient;
        private string m_Token;

        public override void BeginInitialise(DataControllerConfig config)
        {
            base.BeginInitialise(config);

            // Create client
            m_RestClient = new RestClient(Config.UrlRoot);

            // Construct a request for a token
            RestRequest request = new RestRequest(API_KEY_RESOURCE, Method.GET);
            request.AddParameter("apiKey", config.ApiKey);

            // Begin the request
            RequestResource(
                request,
                (ApiKeyResponse response) =>
                {
                    m_Token = response.Token;
                    OnInitialisationComplete(new InitialisationCompleteEventArgs(InitialisationResult.Initialised));
                },
                () => OnInitialisationComplete(new InitialisationCompleteEventArgs(InitialisationResult.Error)));
        }

        protected override void StartCaptureTask(ScannerType scannerType, bool isTemplated, Guid guid, CancellationToken token)
        {
            // Construct a request for a capture
            RestRequest request = new RestRequest(HUMAN_TEMPLATE_RESOURCE, Method.GET);
            request.AddParameter("token", m_Token, ParameterType.QueryString);

            // Begin the request
            RequestResource(
                request,
                (HumanTemplateResponse response) => 
                {
                    CaptureInfo capture = new CaptureInfo(
                        response.Id,
                        Convert.FromBase64String(response.Base64Image),
                        null);
                    // Raise GetCaptureComplete event.
                    OnGetCaptureComplete(new GetCaptureCompleteEventArgs(capture, guid, DataRequestResult.Success));
                },
                () => OnGetCaptureComplete(new GetCaptureCompleteEventArgs(null, guid, DataRequestResult.Failed)));
        }

        protected override void StartSaveTask(long dbId, byte[] template, Guid guid, CancellationToken token)
        {
            RestRequest request = new RestRequest(HUMAN_TEMPLATE_RESOURCE, Method.POST);
        }

        #region Private Methods

        private void RequestResource<T>(RestRequest request, Action<T> successCallback, Action failureCallback)
        {
            m_Log.DebugFormat("Executing request for resource: {0}", request.Resource);
            m_RestClient.ExecuteAsync(request, response =>
            {
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    T resource = default(T);
                    try
                    {
                        resource = JsonConvert.DeserializeObject<T>(response.Content); 
                    }
                    catch (JsonSerializationException ex)
                    {
                        m_Log.Warn("Failed to serialize response to JSON", ex);
                        failureCallback.Invoke();
                    }

                    if (!resource.Equals(default(T)))
                    {
                        successCallback.Invoke(resource);
                    }
                }
                else
                {
                    m_Log.WarnFormat("Request returned status: {0}", response.StatusCode);
                    failureCallback.Invoke();
                }
            });
        }

        #endregion
    }
}
