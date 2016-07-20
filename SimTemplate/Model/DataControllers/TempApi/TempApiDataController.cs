using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SimTemplate.Model.DataControllers.EventArguments;
using RestSharp;
using log4net;
using SimTemplate.DataTypes.Enums;
using SimTemplate.DataTypes;

namespace SimTemplate.Model.DataControllers.TempApi
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
            ResourceRequest(
                request,
                (ApiKey response) =>
                {
                    m_Token = response.Token;
                    OnInitialisationComplete(new InitialisationCompleteEventArgs(InitialisationResult.Initialised));
                },
                () => OnInitialisationComplete(new InitialisationCompleteEventArgs(InitialisationResult.Error)));
        }

        protected override void StartCaptureTask(ScannerType scannerType, Guid guid, CancellationToken token)
        {
            // Construct a request for a capture
            RestRequest request = new RestRequest(HUMAN_TEMPLATE_RESOURCE, Method.GET);
            request.AddParameter("token", m_Token, ParameterType.QueryString);

            // Begin the request
            ResourceRequest(
                request,
                (HumanTemplate response) => 
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
            // Create human template object for serialising
            HumanTemplate templateData = new HumanTemplate(dbId, Convert.ToBase64String(template));

            // Construct a request for a capture
            RestRequest request = new RestRequest(HUMAN_TEMPLATE_RESOURCE, Method.POST);
            request.AddParameter("token", m_Token, ParameterType.QueryString);
            request.AddParameter("application/json", JsonConvert.SerializeObject(templateData), ParameterType.RequestBody);

            // Begin the request
            ResourceRequest(
                request,
                () => OnSaveTemplateComplete(new SaveTemplateEventArgs(guid, DataRequestResult.Success)),
                () => OnSaveTemplateComplete(new SaveTemplateEventArgs(guid, DataRequestResult.Failed)));
        }

        #region Private Methods

        private void ResourceRequest(RestRequest request, Action successCallback, Action failureCallback)
        {
            m_Log.DebugFormat("Executing resource request: {0}", request.Resource);
            m_RestClient.ExecuteAsync(request, response =>
            {
                if (response.StatusCode == HttpStatusCode.OK ||
                    response.StatusCode == HttpStatusCode.ResetContent)
                {
                    successCallback.Invoke();
                }
                else
                {
                    m_Log.WarnFormat("Request returned status: {0}", response.StatusCode);
                    failureCallback.Invoke();
                }
            });
        }

        private void ResourceRequest<T>(RestRequest request, Action<T> successCallback, Action failureCallback)
        {
            m_Log.DebugFormat("Executing resource request: {0}", request.Resource);
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
