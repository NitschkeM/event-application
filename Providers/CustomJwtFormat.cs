using Microsoft.Owin.Security;
using Microsoft.Owin.Security.DataHandler.Encoder;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Thinktecture.IdentityModel.Tokens;

namespace $safeprojectname$.Providers
{
    // For this I installed two NuGet packages: 
    // System.IdentityModel.Tokens.Jwt and Thinktecture.IdentityModel.Core - ****NOTE: HE SAID USE SAME NUGET VERSION i'M USING - I AM NOT DOING THAT *****
    // This is the manual implementation for issuing JWT:
    // The implementation of the interface: "ISecureDataFormat" and the method: "Protect" are the key components. 
    public class CustomJwtFormat : ISecureDataFormat<AuthenticationTicket>
    {
        private readonly string _issuer = string.Empty;

        // This API acts as both Authorization and Resource Server.
        // Constructor: accepts the "Issuer" of this JWT - which will be our API.
        // issuer can be a string or a URI, in the tutorial we will fix it to URI.
        public CustomJwtFormat(string issuer)
        {
            _issuer = issuer;
        }

        // JWT generation takes place inside the Protect method.
        public string Protect(AuthenticationTicket data)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }

            // This API serves a both Resource and Authorization server
            // Therefore we fix the Audience Id and the Audience Secret (Resource Server) in the web.config file
            // This Audience Id and Secret will be used for HMAC265 and hash the JWT token.
            // Then he writes: "I've used this implementation(clickable) to generate the Audience Id and Secret.
            string audienceId = ConfigurationManager.AppSettings["as:AudienceId"];

            string symmetrickKeyAsBase64 = ConfigurationManager.AppSettings["as:AudienceSecret"];


            // Prepare raw data for the token
            var keyByteArray = TextEncodings.Base64Url.Decode(symmetrickKeyAsBase64);

            var signingKey = new HmacSigningCredentials(keyByteArray);

            var issued = data.Properties.IssuedUtc;

            var expires = data.Properties.ExpiresUtc;

            // Here we provide: The issuer, Audience, user claims, issue date, expiery date
            // + the signing key which will sign(hash) the JWT payload.
            var token = new JwtSecurityToken(_issuer, audienceId, data.Identity.Claims, issued.Value.UtcDateTime, expires.Value.UtcDateTime, signingKey);

            var handler = new JwtSecurityTokenHandler();

            // Serialize the JWT to a string
            var jwt = handler.WriteToken(token);

            // Return it to the requester
            return jwt;
        }

        public AuthenticationTicket Unprotect(string protectedText)
        {
            throw new NotImplementedException();
        }
    }
}
