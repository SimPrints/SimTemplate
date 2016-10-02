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
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SimTemplate.Helpers.GoogleApis
{
    /// <summary>
    /// Wrapper for the ConfigurableHttpClient
    /// </summary>
    public class GoogleAuthenticator : IAuthenticationClient
    {
        private const string HEADER_TOKEN_NAME = "X-SimTemplate-Token";
        private const string HEADER_USER_ID_NAME = "X-SimTemplate-UserId";

        private readonly ConfigurableHttpClient m_Client;
        private readonly UserCredential m_Credential;

        public GoogleAuthenticator(ConfigurableHttpClient client, UserCredential credential)
        {
            m_Client = client;
            m_Credential = credential;

            // Configure the client to send the token and UserId as headers
            // We do this so that the SimPrints server may validate our authentication with google
            // and obtain our user details
            HttpRequestHeaders headers = m_Client.DefaultRequestHeaders;
            headers.Add(HEADER_TOKEN_NAME, m_Credential.Token.AccessToken);
            headers.Add(HEADER_USER_ID_NAME, m_Credential.UserId);
        }

        public TokenResponse Token { get {return m_Credential.Token; } }

        public Task<string> GetStringAsync(string requestUri)
        {
            return m_Client.GetStringAsync(requestUri);
        }

        public Task<string> GetStringAsync(Uri requestUri)
        {
            return m_Client.GetStringAsync(requestUri);
        }

        public Task<HttpResponseMessage> PostAsync(string requestUri, HttpContent content)
        {
            return m_Client.PostAsync(requestUri, content);
        }

        public Task<HttpResponseMessage> PostAsync(Uri requestUri, HttpContent content)
        {
            return m_Client.PostAsync(requestUri, content);
        }

        public Task<HttpResponseMessage> PostAsync(string requestUri, HttpContent content, CancellationToken cancellationToken)
        {
            return m_Client.PostAsync(requestUri, content, cancellationToken);
        }

        public Task<HttpResponseMessage> PostAsync(Uri requestUri, HttpContent content, CancellationToken cancellationToken)
        {
            return m_Client.PostAsync(requestUri, content, cancellationToken);
        }

        public Task<HttpResponseMessage> PutAsync(Uri requestUri, HttpContent content)
        {
            return m_Client.PostAsync(requestUri, content);
        }
    }
}
