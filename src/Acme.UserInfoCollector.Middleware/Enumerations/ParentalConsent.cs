namespace Acme.UserInfoCollector.Middleware.Enumerations
{
    /// <summary>
    /// Parental consent
    /// </summary>
    public enum ParentalConsent
    {
        /// <summary>
        /// Parental consent is not needed
        /// </summary>
        NotNeeded,

        /// <summary>
        /// Parent has given consent for user info to be collected
        /// </summary>
        Yes,

        /// <summary>
        /// Parent has not given consent for user info to be collected
        /// </summary>
        No
    }
}
