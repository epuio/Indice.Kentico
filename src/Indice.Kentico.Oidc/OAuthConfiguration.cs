using System;
using System.Configuration;
using IdentityModel;

namespace Indice.Kentico.Oidc
{
    public static class OAuthConfiguration {
        public static bool AutoRedirect => bool.TryParse(ConfigurationManager.AppSettings["Oidc:AutoRedirect"], out var autoRedirect) ? autoRedirect : false;
        public static string Authority => ConfigurationManager.AppSettings["Oidc:Authority"];
        public static string Host => ConfigurationManager.AppSettings["Oidc:Host"];
        public static string ClientId => ConfigurationManager.AppSettings["Oidc:ClientId"];
        public static string ClientSecret => ConfigurationManager.AppSettings["Oidc:ClientSecret"];
        public static string[] Scopes => ConfigurationManager.AppSettings["Oidc:Scopes"]?.Split(' ') ?? Array.Empty<string>();
        public static string ResponseType => ConfigurationManager.AppSettings["Oidc:ResponseType"];
        public static string UserNameClaim => ConfigurationManager.AppSettings["Oidc:UserNameClaim"] ?? "username";
        public static string AuthorizeEndpointPath => ConfigurationManager.AppSettings["Oidc:AuthorizeEndpointPath"]?.TrimStart('/') ?? "connect/authorize";
        public static string TokenEndpointPath => ConfigurationManager.AppSettings["Oidc:TokenEndpointPath"]?.TrimStart('/') ?? "connect/authorize";
        public static string UserInfoEndpointPath => ConfigurationManager.AppSettings["Oidc:UserInfoEndpointPath"]?.TrimStart('/') ?? "connect/authorize";
        
        // EndsessionEndpointPath points to the IDP endpoint used to log the user out of the IDP
        public static string EndsessionEndpointPath => ConfigurationManager.AppSettings["Oidc:EndsessionEndpointPath"]?.TrimStart('/') ?? "connect/endsession";
        
        // EndsessionExtraParameters is used if you must provide additional parameters with endsesson (logout) request
        // For example, for Cognito, it would be: <add key="Oidc:EndsessionExtraParameters" value='{ "client_id": "********", "logout_url": "https://example.com/logout" }' />
        public static string EndsessionExtraParameters => ConfigurationManager.AppSettings["Oidc:EndsessionExtraParameters"] ?? "{}";
    }
}
