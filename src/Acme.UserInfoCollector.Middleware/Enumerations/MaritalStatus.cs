namespace Acme.UserInfoCollector.Middleware.Enumerations
{
    /// <summary>
    /// Enumeration detailing marital status as defined in <href>https://www.cdc.gov/nchs/hus/sources-definitions/marital-status.htm</href>.
    /// </summary>
    public enum MaritalStatus
    {
        /// <summary>
        /// Married
        /// </summary>
        Married = 1,

        /// <summary>
        /// Widowed
        /// </summary>
        Widowed,

        /// <summary>
        /// Divorced
        /// </summary>
        Divorced,

        /// <summary>
        /// Separated (Legally married but living apart)
        /// </summary>
        Separated,

        /// <summary>
        /// Never married/single
        /// </summary>
        NeverMarried,

        /// <summary>
        /// Living with a partner but not legally married
        /// </summary>
        LivingWithPartner
    }
}
