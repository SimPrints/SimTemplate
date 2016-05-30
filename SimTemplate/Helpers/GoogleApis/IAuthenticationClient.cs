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
