using Google.Apis.Auth.OAuth2;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using TemplateBuilder.Helpers;

namespace TemplateBuilder.Model.Database
{
    public class OAuthDataController
    {
        private const string OAUTH_LOGIN = "https://accounts.google.com/o/oauth2/v2/auth";
        private const string CLIENT_ID = "916016530-chmjlljhh1a94fti71h2d9nd1denc7s5.apps.googleusercontent.com";
        private const string CLIENT_SECRET = "Ytg35Ulbfn8WtItj2xUWB5zW";
        private const string SCOPE = @"openid+profile+email";
        private const string REDIRECT_URI = "http://localhost:{0}";
        private const string RESPONSE_TYPE = "code";

        private const int LOCAL_PORT = 9004;

        public OAuthDataController()
        {
            Authenticate();
        }

        public void Authenticate()
        {
            ShowUserLogin();
            ListenForLogin();
        }

        private void ShowUserLogin()
        {
            UserCredential credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                new ClientSecrets
                {
                    ClientId = CLIENT_ID,
                    ClientSecret = CLIENT_SECRET,
                },
                new[] { "profile", "email", "openid" },
                "sjbriggs14@gmail.com",
                CancellationToken.None).Result;
        }

        private void ListenForLogin()
        {
            // create the socket
            Socket listenSocket = new Socket(AddressFamily.InterNetwork,
                                             SocketType.Stream,
                                             ProtocolType.Tcp);

            // bind the listening socket to the port
            IPEndPoint ep = new IPEndPoint(IPAddress.Loopback, LOCAL_PORT);
            listenSocket.Bind(ep);

            // start listening
            listenSocket.Listen(1);
        }

        /// <summary>
        /// Constructs a QueryString (string).
        /// Consider this method to be the opposite of "System.Web.HttpUtility.ParseQueryString"
        /// </summary>
        /// <param name="nvc">NameValueCollection</param>
        /// <returns>String</returns>
        public static String ConstructQueryString(NameValueCollection parameters)
        {
            List<String> items = new List<String>();

            foreach (String name in parameters)
                items.Add(String.Concat(name, "=", parameters[name]));

            return String.Join("&", items.ToArray());
        }
    }
}
