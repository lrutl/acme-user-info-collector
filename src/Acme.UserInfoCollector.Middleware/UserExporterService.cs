using Acme.UserInfoCollector.Middleware.Enumerations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;

namespace Acme.UserInfoCollector.Middleware
{
    /// <summary>
    /// This service is meant to facilitate exporting data for a user.
    /// </summary>
    public class UserExporterService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<UserExporterService> _logger;
        private readonly IConfiguration _configuration;

        /// <summary>
        /// This is the parameterized constructor for the user exporter service.
        /// </summary>
        /// <param name="logger">DI provided logger</param>
        public UserExporterService(IServiceProvider serviceProvider, ILogger<UserExporterService> logger, IConfiguration configuration)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            _configuration = configuration;
        }

        /// <summary>
        /// Get user info line
        /// </summary>
        /// <param name="user">User</param>
        /// <returns>User exported in pipe delineated format</returns>

        public static string GetUserInfoLine(PersonVM user)
        {
            string consent = user.ParentalConsent == ParentalConsent.NotNeeded ? "null" : user.ParentalConsent.ToString();
            return $"{user.FirstName}|{user.Surname}|{user.DateOfBirth:dd-MM-yyyy}|{user.MaritalStatus}|{consent}|";
        }

        /// <summary>
        /// Validate a given user
        /// </summary>
        /// <param name="user">User to validate</param>
        /// <returns>True if given a valid user</returns>
        public bool ValidateUser(PersonVM user)
        {
            var ctx = new ValidationContext(user, _serviceProvider, new Dictionary<object, object>());
            try
            {
                Validator.ValidateObject(user, ctx, true);
            }
            catch (ValidationException vex)
            {
                _logger.LogError("Service was provided with an invalid user to export; see following messages for more details");
                _logger.LogError(vex.Message);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Persist a given user to a pre-configured *.csv location
        /// </summary>
        /// <param name="user">User to export</param>
        /// <returns>True if saved correctly, false if otherwise</returns>
        public bool SaveUser(PersonVM user)
        {
            if (ValidateUser(user) == false)
            {
                return false;
            }

            if (user.IsLegallyMarried && ValidateUser(user.PartnerInfo) == false)
            {
                return false;
            }

            string userExportPath = _configuration.GetValue<string>("UserExportPath");
            string partnerExportPathFormat = _configuration.GetValue<string>("UserPartnerExportPath");

            if (string.IsNullOrWhiteSpace(userExportPath))
            {
                _logger.LogError("There is no configured path for exporting user data; user data will not be persisted.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(partnerExportPathFormat))
            {
                _logger.LogError("There is no configured path for exporting user partner data; user data will not be persisted.");
                return false;
            }

            if (!File.Exists(userExportPath))
            {
                _logger.LogInformation($"No user data found at {userExportPath}--creating new data store.");
                Directory.CreateDirectory(Path.GetDirectoryName(userExportPath));
                File.Create(userExportPath).Close();
            }

            if (!Directory.Exists(Path.GetDirectoryName(partnerExportPathFormat)))
            {
                _logger.LogInformation($"Creating new partner data store at {Path.GetDirectoryName(partnerExportPathFormat)}");
                Directory.CreateDirectory(Path.GetDirectoryName(partnerExportPathFormat));
            }

            List<string> allUserData = File.ReadAllLines(userExportPath).ToList();
            string userData = GetUserInfoLine(user);

            if (user.PartnerInfo != null)
            {
                string partnerData = GetUserInfoLine(user.PartnerInfo);
                string partnerDataPath = string.Format(partnerExportPathFormat, $"{user.PartnerInfo.FirstName}.{Guid.NewGuid()}.txt");
                string fullPartnerDataPath = Path.GetFullPath(partnerDataPath);

                userData = $"{userData}{fullPartnerDataPath}";

                try
                {
                    File.WriteAllText(partnerDataPath, partnerData);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message);
                    return false;
                }
            }

            allUserData.Add(userData);
            try
            {
                File.WriteAllLines(userExportPath, allUserData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return false;
            }

            return true;
        }
    }
}
