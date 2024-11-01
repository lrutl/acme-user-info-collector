﻿using Acme.UserInfoCollector.Middleware.Enumerations;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;
using System;
using System.ComponentModel.DataAnnotations;

namespace Acme.UserInfoCollector.Middleware
{
    /// <summary>
    /// Person view model
    /// </summary>
    public class PersonVM : ObservableValidator
    {
        private readonly ILogger<PersonVM> _logger;
        private readonly UserExporterService _userExporterService;

        private string _FirstName = string.Empty;
        private string _Surname = string.Empty;
        private DateTime _DateOfBirth = DateTime.MinValue;
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
        [CustomValidation(typeof(PersonVM), nameof(ValidateStringDoesNotContainPipes))]
        public string FirstName { get => _FirstName; set => SetProperty(ref _FirstName, value); }

        /// <summary>
        /// Sirname/last name of the user whose information is being collected
        /// </summary>
        [CustomValidation(typeof(PersonVM), nameof(ValidateStringDoesNotContainPipes))]
        public string Surname { get => _Surname; set => SetProperty(ref _Surname, value); }

        /// <summary>
        /// User's date of birth
        /// </summary>
        ///        
        [CustomValidation(typeof(PersonVM), nameof(ValidateUserAge))]
        public DateTime DateOfBirth { get => _DateOfBirth; set => SetProperty(ref _DateOfBirth, value); }

        /// <summary>
        /// Marital status of the user
        /// </summary>
        [Required]
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
        [CustomValidation(typeof(PersonVM), nameof(ValidateUserParentalConsent))]
        public ParentalConsent ParentalConsent { get => _ParentalConsent; set => SetProperty(ref _ParentalConsent, value); }

        /// <summary>
        /// Partner information if user is married
        /// </summary>
        public PersonVM PartnerInfo
        { 
            get => _PartnerInfo;
            set
            {
                SetProperty(ref _PartnerInfo, value);
                PartnerInfo.MaritalStatus = MaritalStatus;
            }
        }

        /// <summary>
        /// Path to partner info
        /// </summary>
        public string PartnerInfoPath { get => _PartnerInfoPath; set => SetProperty(ref _PartnerInfoPath, value); }

        /// <summary>
        /// Validate user parental consent
        /// </summary>
        public static ValidationResult ValidateUserParentalConsent(ParentalConsent parentalConsent, ValidationContext context)
        {
            if (parentalConsent == ParentalConsent.No)
            {
                return new ValidationResult("A parental figure has explicity asked for the user not to use this applicaton.");
            }

            return ValidationResult.Success;
        }

        /// <summary>
        /// Validate a user's age
        /// </summary>
        /// <param name="toValidate">Age to validate</param>
        /// <param name="context">Validation context</param>
        /// <returns></returns>
        public static ValidationResult ValidateUserAge(DateTime toValidate, ValidationContext context)
        {
            if (toValidate.AddYears(16) <= DateTime.Now)
            {
                return ValidationResult.Success;
            }

            return new ValidationResult("User must be 16 years old to use this application.");
        }

        /// <summary>
        /// Custom validation to ensure user does not break the data store
        /// </summary>
        /// <param name="toValidate">String to validate</param>
        /// <returns>Validaiton result</returns>
        public static ValidationResult ValidateStringDoesNotContainPipes(string toValidate, ValidationContext context)
        {
            if (!toValidate.Contains("|"))
            {
                return ValidationResult.Success;
            }

            return new ValidationResult("The provided value contains unsupported special characters.");
        }
    }
}
