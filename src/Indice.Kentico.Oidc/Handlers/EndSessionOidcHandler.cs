using CMS.Membership;
using CMS.EventLog;
using IdentityModel;
using IdentityModel.Client;
using System;
using System.Web;
using System.Collections;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Configuration;
using Newtonsoft.Json;

namespace Indice.Kentico.Oidc
{
    public class EndSessionOidcHandler : IHttpHandler
    {
        public bool IsReusable => false;

        public void ProcessRequest(HttpContext context)
        {
            if (context.User.Identity.IsAuthenticated)
            {
                AuthenticationHelper.SignOut();
            }
            EndSession();
        }

        public static void EndSession()
        {
            // At this point we have already sign out by using FormsAuthentication and we also have to sign out from Identity Server.
            // Create the url to Identity Server's end session endpoint.

            // Create dictionary for extra parameters if they've been defined
            IDictionary extraParameters = new Dictionary<string, string>();
            if (ConfigurationManager.AppSettings["Oidc:EndsessionExtraParameters"] != null) {
                try {
                    var extraValues = JsonConvert.DeserializeObject<Dictionary<string, string>>(OAuthConfiguration.EndsessionExtraParameters);
                    if (OAuthConfiguration.EndsessionExtraParameters != null) {
                        foreach (KeyValuePair<string, string> entry in extraValues) {
                            extraParameters.Add(entry.Key, entry.Value);
                        };
                    }
                }
                catch (Exception e) {
                    EventLogProvider.LogInformation("Indice.Kentico.Oidc", "Parameter Error", "Improperly formatted EndsessionExtraParameters");
                    throw(e);
                }
            }

            var endsessionEndpoint = OAuthConfiguration.Authority.TrimEnd('/') + "/" + OAuthConfiguration.EndsessionEndpointPath;
            var requestUrl = new RequestUrl(endsessionEndpoint);
            var endSessionUrl = requestUrl.CreateEndSessionUrl(
                idTokenHint: HttpContext.Current.GetToken(OidcConstants.ResponseTypes.IdToken),
                postLogoutRedirectUri: OAuthConfiguration.Host,
                state: null,
                extra: extraParameters
            );
            if (!HttpContext.Current.Response.IsRequestBeingRedirected)
            {
                HttpContext.Current.Response.Redirect(endSessionUrl);
            }
            HttpContext.Current.ApplicationInstance.CompleteRequest();
        }
    }
}