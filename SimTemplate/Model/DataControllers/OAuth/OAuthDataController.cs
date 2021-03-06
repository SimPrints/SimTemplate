﻿// Copyright 2016 Sam Briggs
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
using Google.Apis.Auth.OAuth2;
using Google.Apis.Http;
using log4net;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Linq;
using SimTemplate.Helpers;
using SimTemplate.Helpers.GoogleApis;
using SimTemplate.Model.DataControllers.EventArguments;
using SimTemplate.DataTypes.Enums;
using SimTemplate.DataTypes;

namespace SimTemplate.Model.DataControllers.OAuth
{
    public class OAuthDataController : DataController
    {
        #region Constants

        // TODO: Secret in environment variable or json file (FileStream)
        private static readonly ClientSecrets APP_SECRETS = new ClientSecrets
        {
            ClientId = "916016530-chmjlljhh1a94fti71h2d9nd1denc7s5.apps.googleusercontent.com",
            ClientSecret = "Ytg35Ulbfn8WtItj2xUWB5zW",
        };
        private static readonly IEnumerable<string> API_CONTEXTS = new string[]
        {
            "profile",
        };

        #endregion

        IConfigurableHttpClientFactory m_ClientFactory;
        IAuthenticationClient m_Client;

        public OAuthDataController(IConfigurableHttpClientFactory clientFactory)
        {
            m_ClientFactory = clientFactory;

            m_ClientFactory.GetClientComplete += ClientFactory_GetClientComplete;
        }

        #region Event Handlers

        private void ClientFactory_GetClientComplete(object sender, GetClientCompleteEventArgs e)
        {
            if (e.Client == null)
            {
                OnInitialisationComplete(new InitialisationCompleteEventArgs(InitialisationResult.Error));
            }
            else
            {
                m_Client = e.Client;
                OnInitialisationComplete(new InitialisationCompleteEventArgs(InitialisationResult.Initialised));
            }
        }

        #endregion

        #region Private Methods

        protected override void StartInitialiseTask(DataControllerConfig config, Guid guid, CancellationToken token)
        {
            // TODO: determine if it is possible to leave the user field empty
            m_ClientFactory.BeginGetClient(APP_SECRETS, API_CONTEXTS, "");
        }

        protected override void StartCaptureTask(ScannerType scannerType, Guid guid, CancellationToken token)
        {
            Log.DebugFormat("Starting task with Guid={0}", guid);

            // Make HTTP request
            Task<string> responseText = m_Client.GetStringAsync(
                SimTemplateServerHelper.GetCaptureRequestUri(scannerType));

            // When task is complete, raise GetCaptureComplete event
            // Pass the task the cancellation token so that this action may be skipped
            responseText.ContinueWith((Task<string> gCT) =>
            {
                // Check for cancellation (race)
                if (!token.IsCancellationRequested)
                {
                    // HTTP requests are not cancellable
                    IntegrityCheck.IsFalse(gCT.IsCanceled);
                    if (gCT.IsCompleted)
                    {
                        CaptureInfo capture;
                        try
                        {
                            capture = ProcessResponse(gCT.Result);
                            OnGetCaptureComplete(
                                new GetCaptureCompleteEventArgs(capture, guid, DataRequestResult.Success));
                        }
                        catch (SimTemplateException ex)
                        {
                            Log.Error("Failed to process xml response:", ex);
                            OnGetCaptureComplete(
                                new GetCaptureCompleteEventArgs(null, guid, DataRequestResult.Failed));
                        }
                    }
                    else if (gCT.IsFaulted)
                    {
                        // An exception was thrown during the request.
                        Log.Error("GetCapture task failed: " + gCT.Exception.Message, gCT.Exception);
                        OnGetCaptureComplete(
                            new GetCaptureCompleteEventArgs(null, guid, DataRequestResult.TaskFailed));
                    }
                }
            }, token);
        }

        protected override void StartSaveTask(long dbId, byte[] template, Guid guid, CancellationToken token)
        {
            // Make HTTP request
            HttpContent content = new ByteArrayContent(template);
            Task<HttpResponseMessage> saveResponse = m_Client.PostAsync(SimTemplateServerHelper.SaveTemplateUri(dbId), content);

            // When task is complete, raise GetCaptureComplete event
            // Pass the task the cancellation token so that this action may be skipped
            saveResponse.ContinueWith((Task<HttpResponseMessage> sTT) =>
            {
                // Check for cancellation (race)
                if (!token.IsCancellationRequested)
                {
                    // HTTP requests are not cancellable
                    IntegrityCheck.IsFalse(sTT.IsCanceled);
                    if (sTT.IsCompleted)
                    {
                        HttpResponseMessage response = sTT.Result;
                        // Do some dealing with the response to check it was successful
                        OnSaveTemplateComplete(
                            new SaveTemplateEventArgs(guid, DataRequestResult.Success));
                    }
                    else if (sTT.IsFaulted)
                    {
                        // An exception was thrown during the request.
                        Log.Error("Save Template task failed: " + sTT.Exception.Message, sTT.Exception);
                        OnSaveTemplateComplete(
                            new SaveTemplateEventArgs(guid, DataRequestResult.TaskFailed));
                    }
                }
            }, token);
        }

        private CaptureInfo ProcessResponse(string response)
        {
            // Parse the response XML
            XDocument xml;
            try
            {
                xml = XDocument.Parse(response);
            }
            catch (System.Xml.XmlException ex)
            {
                throw new SimTemplateException("Failed to parse response XML", ex);
            }

            CaptureInfo capture;
            // Expect one capture element (or none)
            XElement captureEl = xml.Elements("capture").SingleOrDefault();

            if (captureEl != null)
            {
                capture = SimTemplateServerHelper.ToCaptureInfo(captureEl);
            }
            else
            {
                throw new SimTemplateException("XML response contained no capture results");
            }
            return capture;
        }

        #endregion

        private static class SimTemplateServerHelper
        {
            #region Constants
            private const string ROOT_URL = @"https://simtemplateapi.azurewebsites.net";
            private const string ELEMENT_IMAGE_LOCATION = "imageUrl";
            private const string ELEMENT_DB_ID = "dbId";
            private const string ELEMENT_GUID = "guid";
            private const string ELEMENT_TEMPLATE = "template";
            #endregion

            #region Uris

            public static Uri GetCaptureRequestUri(ScannerType type)
            {
                return new Uri(String.Format("{0}/Capture?scanner={1}",
                    ROOT_URL, type));
            }

            public static Uri SaveTemplateUri(long dbId)
            {
                return new Uri(String.Format("{0}/Capture/{1}",
                    ROOT_URL, dbId));
            }

            #endregion

            #region Xml

            public static CaptureInfo ToCaptureInfo(XElement captureEl)
            {
                XElement imageLocationEl = captureEl.Element(ELEMENT_IMAGE_LOCATION);
                XElement dbIdEl = captureEl.Element(ELEMENT_DB_ID);
                XElement templateEl = captureEl.Element(ELEMENT_TEMPLATE);

                // Assert xml structure is as expected
                CheckElementNotNull(imageLocationEl, ELEMENT_IMAGE_LOCATION);
                CheckElementNotNull(dbIdEl, ELEMENT_DB_ID);
                CheckElementNotNull(templateEl, ELEMENT_TEMPLATE);

                // Get info from elements
                Uri imageLocation;
                bool isUriParsed = Uri.TryCreate(imageLocationEl.Value, UriKind.Absolute, out imageLocation);
                if (!isUriParsed)
                {
                    throw new SimTemplateException(
                        String.Format("Failed to parse image URL string ({0}) to Uri", imageLocationEl.Value));
                }
                long dbId;
                bool isIdParsed = long.TryParse(dbIdEl.Value, out dbId);
                if (!isIdParsed)
                {
                    throw new SimTemplateException(
                        String.Format("Failed to parse capture ID string ({0}) to long", dbIdEl.Value));
                }
                byte[] templateData = null;
                if (!String.IsNullOrEmpty(templateEl.Value))
                {
                    templateData = IsoTemplateHelper.ToByteArray(templateEl.Value);
                }
                byte[] imageData;

                // Get image file from url
                using (WebClient webClient = new WebClient())
                {
                    imageData = webClient.DownloadData(imageLocation);
                }

                return new CaptureInfo(dbId, imageData, templateData);
            }

            private static void CheckElementNotNull(XElement el, string tag)
            {
                IntegrityCheck.IsNotNull(el,
                    "Capture was missing '{0}' element.", tag);
            }

            #endregion
        }
    }
}
