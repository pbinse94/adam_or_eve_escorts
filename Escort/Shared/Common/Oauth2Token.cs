using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Common
{
    public class Oauth2Token
    {
        public string Type { get; set; } = string.Empty;

        /// <summary>
        /// Email associated to a token, it is important for code that monitors
        /// email IMAP folder or send data with SMTP
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Oid claim.
        /// </summary>
        public string Sub { get; set; } = string.Empty;

        public string Scope { get; set; } = string.Empty;

        public DateTime ExpireAtUtc { get; set; }

        public string AccessToken { get; set; } = string.Empty;

        public string RefreshToken { get; set; } = string.Empty;

        public string IdToken { get; set; } = string.Empty;

        public static Oauth2Token DeserializeFromTokenResponse(string json)
        {
            var obj = JsonConvert.DeserializeAnonymousType(json, new
            {
                Type = "",
                Scope = "",
                ExpireAtUtc = 0,
                AccessToken = "",
                RefreshToken = "",
                IdToken = "",
            });

            return new Oauth2Token()
            {
                AccessToken = obj?.AccessToken ?? "",
                ExpireAtUtc = DateTime.UtcNow.AddSeconds(obj?.ExpireAtUtc ?? 0),
                RefreshToken = obj?.RefreshToken ?? "",
                IdToken = obj?.IdToken ?? "",
                Scope = obj?.Scope ?? "",
                Type = obj?.Type ?? "",
            };
        }

        internal void Refresh(Oauth2Token refreshedToken)
        {
            AccessToken = refreshedToken.AccessToken;
            ExpireAtUtc = refreshedToken.ExpireAtUtc;
            if (!string.IsNullOrEmpty(refreshedToken.RefreshToken) && RefreshToken != refreshedToken.RefreshToken)
            {
                RefreshToken = refreshedToken.RefreshToken;
            }
        }

    }

}
