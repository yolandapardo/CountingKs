using CountingKs.Data;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using WebMatrix.WebData;

namespace CountingKs.Filters
{
    public class CountingKsAuthorizeAttribute: AuthorizationFilterAttribute
    {
        private bool _perUser;

        public CountingKsAuthorizeAttribute(bool perUser=true)
        {
            _perUser = perUser;
        }

        [Inject]
        public CountingKsRepository TheRepo { get; set; }
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            const string APIKEYNAME = "apikey";
            const string TOKENNAME = "token";
            var query = HttpUtility.ParseQueryString(actionContext.Request.RequestUri.Query);
            if (!string.IsNullOrWhiteSpace(query[APIKEYNAME]) && !string.IsNullOrWhiteSpace(query[TOKENNAME]))
            {
                var apiKey = query[APIKEYNAME];
                var token = query[TOKENNAME];

                var authToken = TheRepo.GetAuthToken(token);
                if (authToken != null && authToken.ApiUser.AppId == apiKey && authToken.Expiration > DateTime.UtcNow)
                {
                    if (_perUser)
                    {
                        if (Thread.CurrentPrincipal.Identity.IsAuthenticated)
                            return;

                        var authHeader = actionContext.Request.Headers.Authorization;

                        if (authHeader != null)
                        {
                            if (authHeader.Scheme.Equals("basic", StringComparison.OrdinalIgnoreCase) && !string.IsNullOrWhiteSpace(authHeader.Parameter))
                            {
                                var rawCredentials = authHeader.Parameter;
                                var encoding = Encoding.GetEncoding("iso-8859-1");
                                var credentials = encoding.GetString(Convert.FromBase64String(rawCredentials));
                                var split = credentials.Split(':');
                                var username = split[0];
                                var password = split[1];

                                if (!WebSecurity.Initialized)
                                {
                                    //la conexion a la tabla UserProfile de la DB
                                    WebSecurity.InitializeDatabaseConnection("DefaultConnection", "UserProfile", "UserId", "UserName", autoCreateTables: true);
                                }

                                if (WebSecurity.Login(username, password))
                                {
                                    var principal = new GenericPrincipal(new GenericIdentity(username), null);
                                    Thread.CurrentPrincipal = principal;
                                    return;
                                }
                            }
                        }
                    }
                    else
                    {
                        return;
                    }
                }
            }
            HandleUnauthorize(actionContext);
        }

        public void HandleUnauthorize(HttpActionContext actionContext)
        {
            actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized);
            actionContext.Response.Headers.Add("WWW-Authenticate","Basic Scheme='CountingKs' location='http://localhost:8901/account' "); 
        }
    }
}