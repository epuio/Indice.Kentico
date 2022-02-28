using CMS.Membership;
using IdentityModel;
using IdentityModel.Client;
using System.Web;
using System.Collections;
using System.Collections.Specialized;

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
            StringDictionary cognitoParameters = new StringDictionary();
            cognitoParameters.Add("client_id", OAuthConfiguration.ClientId);
            cognitoParameters.Add("logout_uri", OAuthConfiguration.EndsessionEndpointPath);

            var endsessionEndpoint = OAuthConfiguration.Authority.TrimEnd('/') + "/" + OAuthConfiguration.EndsessionEndpointPath;
            var requestUrl = new RequestUrl(endsessionEndpoint);
            var endSessionUrl = requestUrl.CreateEndSessionUrl(
                idTokenHint: HttpContext.Current.GetToken(OidcConstants.ResponseTypes.IdToken),
                postLogoutRedirectUri: OAuthConfiguration.Host,
                state: null,
                extra: cognitoParameters
            );
            if (!HttpContext.Current.Response.IsRequestBeingRedirected)
            {
                HttpContext.Current.Response.Redirect(endSessionUrl);
            }
            HttpContext.Current.ApplicationInstance.CompleteRequest();
        }
    }
}