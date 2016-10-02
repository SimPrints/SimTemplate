// Copyright 2016 Sam Briggs
//
// This file is part of SimTemplate.
//
// SimTemplate is free software: you can redistribute it and/or modify it under the
// terms of the GNU General Public License as published by the Free Software 
// Foundation, either version 3 of the License, or (at your option) any later
// version.
//
// SimTemplate is distributed in the hope that it will be useful, but WITHOUT ANY 
// WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR 
// A PARTICULAR PURPOSE. See the GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License along with
// SimTemplate. If not, see http://www.gnu.org/licenses/.
//
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
using SimTemplate.Utilities;

namespace SimTemplate.Model.DataControllers.TempApi
{
    // TODO: Make requests abortable by using RestRequestAsyncHandle
    // Will require a new abstract DataController class that implements the CancellationToken
    // aproach for other controllers, but leaves this one free to use it's own handles.
    public class TempApiDataController : DataController
    {
        #region Constants

        // TODO: Move these API constants into config, created from application settings
        // Resources
        private const string API_KEY_RESOURCE = @"api/ApiKey";
        private const string HUMAN_TEMPLATE_RESOURCE = @"api/HumanTemplate";
        // Parameters
        private const string API_KEY_PARAMETER = @"apiKey";
        private const string TOKEN_PARAMETER = @"token";
        // Other
        private const string RESPONSE_FORMAT = @"application/json";

        #endregion

        private RestClient m_RestClient;

        // Token obtained from the server that grants access to resources.
        private string m_Token;

        protected override void StartInitialiseTask(DataControllerConfig config, Guid guid, CancellationToken token)
        {
            // Create client
            m_RestClient = new RestClient(Config.UrlRoot);

            // Construct a request for a token
            RestRequest request = new RestRequest(API_KEY_RESOURCE, Method.GET);
            request.AddParameter(API_KEY_PARAMETER, config.ApiKey);

            // Begin the request
            ResourceRequest(
                request,
                (ApiKey response) =>
                {
                    m_Token = response.Token;
                    OnInitialisationComplete(new InitialisationCompleteEventArgs(InitialisationResult.Initialised, guid, DataRequestResult.Success));
                },
                () => OnInitialisationComplete(new InitialisationCompleteEventArgs(InitialisationResult.Error, guid, DataRequestResult.Failed)));
        }

        protected override void StartCaptureTask(ScannerType scannerType, Guid guid, CancellationToken token)
        {
            // Construct a request for a capture
            RestRequest request = new RestRequest(HUMAN_TEMPLATE_RESOURCE, Method.GET);
            request.AddParameter(TOKEN_PARAMETER, m_Token, ParameterType.QueryString);

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
            request.AddParameter(TOKEN_PARAMETER, m_Token, ParameterType.QueryString);
            request.AddParameter(RESPONSE_FORMAT, JsonConvert.SerializeObject(templateData), ParameterType.RequestBody);

            // Begin the request
            ResourceRequest(
                request,
                () => OnSaveTemplateComplete(new SaveTemplateEventArgs(guid, DataRequestResult.Success)),
                () => OnSaveTemplateComplete(new SaveTemplateEventArgs(guid, DataRequestResult.Failed)));
        }

        #region Private Methods

        private void ResourceRequest(RestRequest request, Action successCallback, Action failureCallback)
        {
            Log.DebugFormat("Executing resource request: {0}", request.Resource);
            m_RestClient.ExecuteAsync(request, response =>
            {
                if (response.StatusCode == HttpStatusCode.OK ||
                    response.StatusCode == HttpStatusCode.ResetContent)
                {
                    successCallback.Invoke();
                }
                else
                {
                    Log.WarnFormat("Request returned status: {0}", response.StatusCode);
                    failureCallback.Invoke();
                }
            });
        }

        private void ResourceRequest<T>(RestRequest request, Action<T> successCallback, Action failureCallback)
        {
            Log.DebugFormat("Executing resource request: {0}", request.Resource);
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
                        Log.Warn("Failed to serialize response to JSON", ex);
                        failureCallback.Invoke();
                    }

                    if (!resource.Equals(default(T)))
                    {
                        successCallback.Invoke(resource);
                    }
                }
                else
                {
                    Log.WarnFormat("Request returned status: {0}", response.StatusCode);
                    failureCallback.Invoke();
                }
            });
        }

        #endregion
    }
}
