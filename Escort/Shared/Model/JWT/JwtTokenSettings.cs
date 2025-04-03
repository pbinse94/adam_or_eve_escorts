namespace Shared.Model.JWT
{
    public class JwtTokenSettings
    {
#nullable disable
        /// <summary>
        /// Expiry
        /// </summary>
        public int Expiry { get; set; }

        /// <summary>
        /// Audience
        /// </summary>
        public string Audience { get; set; }

        /// <summary>
        /// Subject
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// Secret
        /// </summary>
        public string Secret { get; set; }

        /// <summary>
        /// IsUser
        /// </summary>
        public string IsUser { get; set; }
    }
}
