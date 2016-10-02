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
using Google.Apis.Auth.OAuth2.Responses;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SimTemplate.Helpers.GoogleApis
{
    public interface IAuthenticationClient
    {
        TokenResponse Token { get; }

        //
        // Summary:
        //     Send a GET request to the specified Uri and return the response body as a string
        //     in an asynchronous operation.
        //
        // Parameters:
        //   requestUri:
        //     The Uri the request is sent to.
        //
        // Returns:
        //     Returns System.Threading.Tasks.Task`1.The task object representing the asynchronous
        //     operation.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     The requestUri was null.
        Task<string> GetStringAsync(Uri requestUri);
        //
        // Summary:
        //     Send a GET request to the specified Uri and return the response body as a string
        //     in an asynchronous operation.
        //
        // Parameters:
        //   requestUri:
        //     The Uri the request is sent to.
        //
        // Returns:
        //     Returns System.Threading.Tasks.Task`1.The task object representing the asynchronous
        //     operation.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     The requestUri was null.
        Task<string> GetStringAsync(string requestUri);
        //
        // Summary:
        //     Send a POST request to the specified Uri as an asynchronous operation.
        //
        // Parameters:
        //   requestUri:
        //     The Uri the request is sent to.
        //
        //   content:
        //     The HTTP request content sent to the server.
        //
        // Returns:
        //     Returns System.Threading.Tasks.Task`1.The task object representing the asynchronous
        //     operation.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     The requestUri was null.
        Task<HttpResponseMessage> PostAsync(Uri requestUri, HttpContent content);
        //
        // Summary:
        //     Send a POST request to the specified Uri as an asynchronous operation.
        //
        // Parameters:
        //   requestUri:
        //     The Uri the request is sent to.
        //
        //   content:
        //     The HTTP request content sent to the server.
        //
        // Returns:
        //     Returns System.Threading.Tasks.Task`1.The task object representing the asynchronous
        //     operation.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     The requestUri was null.
        Task<HttpResponseMessage> PostAsync(string requestUri, HttpContent content);
        //
        // Summary:
        //     Send a POST request with a cancellation token as an asynchronous operation.
        //
        // Parameters:
        //   requestUri:
        //     The Uri the request is sent to.
        //
        //   content:
        //     The HTTP request content sent to the server.
        //
        //   cancellationToken:
        //     A cancellation token that can be used by other objects or threads to receive
        //     notice of cancellation.
        //
        // Returns:
        //     Returns System.Threading.Tasks.Task`1.The task object representing the asynchronous
        //     operation.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     The requestUri was null.
        Task<HttpResponseMessage> PostAsync(Uri requestUri, HttpContent content, CancellationToken cancellationToken);
        //
        // Summary:
        //     Send a POST request with a cancellation token as an asynchronous operation.
        //
        // Parameters:
        //   requestUri:
        //     The Uri the request is sent to.
        //
        //   content:
        //     The HTTP request content sent to the server.
        //
        //   cancellationToken:
        //     A cancellation token that can be used by other objects or threads to receive
        //     notice of cancellation.
        //
        // Returns:
        //     Returns System.Threading.Tasks.Task`1.The task object representing the asynchronous
        //     operation.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     The requestUri was null.
        Task<HttpResponseMessage> PostAsync(string requestUri, HttpContent content, CancellationToken cancellationToken);
        //
        // Summary:
        //     Send a PUT request to the specified Uri as an asynchronous operation.
        //
        // Parameters:
        //   requestUri:
        //     The Uri the request is sent to.
        //
        //   content:
        //     The HTTP request content sent to the server.
        //
        // Returns:
        //     Returns System.Threading.Tasks.Task`1.The task object representing the asynchronous
        //     operation.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     The requestUri was null.
        Task<HttpResponseMessage> PutAsync(Uri requestUri, HttpContent content);
    }
}
