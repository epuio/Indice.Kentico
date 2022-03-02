using CMS.Membership;
using IdentityModel;
using IdentityModel.Client;
using System.Web;
using System.Collections;
using System.Collections.Specialized;
using System.Collections.Generic;
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

            // Creating string dictionary for extra parameter
            //IDictionary extraParameters = new Dictionary<string,string>() {
            //    { "client_id", OAuthConfiguration.ClientId },
            //    { "logout_uri", OAuthConfiguration.Host.TrimEnd('/') + "/SignOut.ashx" }
            //};
            IDictionary extraParameters = new Dictionary<string, string>();
            //if (OAuthConfiguration.EndsessionExtraParameters != null) {
            //    extraParameters.Add("client_id", OAuthConfiguration.ClientId);
            //    extraParameters.Add("logout_uri", OAuthConfiguration.Host.TrimEnd('/'));
            //}
            var extraValues = JsonConvert.DeserializeObject<Dictionary<string, string>>(OAuthConfiguration.EndsessionExtraParameters);
            if (OAuthConfiguration.EndsessionExtraParameters != null) {
                foreach (KeyValuePair<string, string> entry in extraValues) {
                    // do something with entry.Value or entry.Key
                    extraParameters.Add(entry.Key, entry.Value);
                };
                //extraParameters.Add("client_id", OAuthConfiguration.ClientId);
                //extraParameters.Add("logout_uri", OAuthConfiguration.Host.TrimEnd('/'));
            }

            // Set the default CreateSessionUrl parameters if configuration indicates they should be
            string idTokenHint = (OAuthConfiguration.EndsessionExcludeDefaultParameters) ? null : HttpContext.Current.GetToken(OidcConstants.ResponseTypes.IdToken);
            string postLogoutRedirectUri = (OAuthConfiguration.EndsessionExcludeDefaultParameters) ? null : OAuthConfiguration.Host;

            var endsessionEndpoint = OAuthConfiguration.Authority.TrimEnd('/') + "/" + OAuthConfiguration.EndsessionEndpointPath;
            var requestUrl = new RequestUrl(endsessionEndpoint);
            var endSessionUrl = requestUrl.CreateEndSessionUrl(
                idTokenHint: idTokenHint,
                postLogoutRedirectUri: postLogoutRedirectUri,
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