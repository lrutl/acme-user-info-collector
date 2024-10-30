using Acme.UserInfoCollector.Middleware.Enumerations;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;
using System;
using System.Runtime.Serialization;

namespace Acme.UserInfoCollector.Middleware
{
    [DataContract]
    public class PersonVM : ObservableObject
    {
        private readonly ILogger<PersonVM> _logger;
        private readonly UserExporterService _userExporterService;

        private string _FirstName = string.Empty;
        private string _Surname = string.Empty;
        private DateTime _DateOfBirth = DateTime.Now;
        private MaritalStatus _MaritalStatus;
        private ParentalConsent _ParentalConsent;
        private PersonVM _PartnerInfo;
        private string _PartnerInfoPath = string.Empty;

        /// <summary>
        /// This is the parameterized constructor for the person view model.
        /// </summary>
        /// <param name="logger">DI provided logger</param>
        /// <param name="userExporterService">User exporter service</param>
        public PersonVM(ILogger<PersonVM> logger, UserExporterService userExporterService)
        {
            _logger = logger;
            _userExporterService = userExporterService;
        }

        /// <summary>
        /// First name of the user whose information is being collected
        /// </summary>
        [DataMember]
        public string FirstName { get => _FirstName; set => SetProperty(ref _FirstName, value); }

        /// <summary>
        /// Sirname/last name of the user whose information is being collected
        /// </summary>
        [DataMember]
        public string Surname { get => _Surname; set => SetProperty(ref _Surname, value); }

        /// <summary>
        /// User's date of birth
        /// </summary>
        [DataMember]
        public DateTime DateOfBirth { get => _DateOfBirth; set => SetProperty(ref _DateOfBirth, value); }

        /// <summary>
        /// Marital status of the user
        /// </summary>
        [DataMember]
        public MaritalStatus MaritalStatus { get => _MaritalStatus; set => SetProperty(ref _MaritalStatus, value); }

        /// <summary>
        /// Returns true if user is legally married
        /// </summary>
        public bool IsLegallyMarried
        {
            get
            {
                if (MaritalStatus == Enumerations.MaritalStatus.Married)
                {
                    return true;
                }

                if (MaritalStatus == Enumerations.MaritalStatus.Separated)
                {
                    return true;
                }

                return false;
            }
        }

        /// <summary>
        /// Whether or not the user's parent has given consent for info to be exported
        /// </summary>
        [DataMember]
        public ParentalConsent ParentalConsent { get => _ParentalConsent; set => SetProperty(ref _ParentalConsent, value); }

        /// <summary>
        /// Partner information if user is married
        /// </summary>
        public PersonVM PartnerInfo {  get => _PartnerInfo; set => SetProperty(ref _PartnerInfo, value); }

        /// <summary>
        /// Path to partner info
        /// </summary>
        [DataMember]
        public string PartnerInfoPath { get => _PartnerInfoPath; set => SetProperty(ref _PartnerInfoPath, value); }
    }
}
