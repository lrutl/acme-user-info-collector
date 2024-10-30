using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Acme.UserInfoCollector.Middleware
{
    /// <summary>
    /// This service is meant to facilitate exporting data for a user.
    /// </summary>
    public class UserExporterService
    {
        private ILogger<UserExporterService> _logger;
        private IConfiguration _configuration;

        /// <summary>
        /// This is the parameterized constructor for the user exporter service.
        /// </summary>
        /// <param name="logger">DI provided logger</param>
        public UserExporterService(ILogger<UserExporterService> logger, IConfiguration configuration)
        {
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
            return $"{user.FirstName}|{user.Surname}|{user.DateOfBirth}|{user.MaritalStatus}|{user.ParentalConsent}|";
        }

        /// <summary>
        /// Persist a given user to a pre-configured *.csv location
        /// </summary>
        /// <param name="user">User to export</param>
        /// <returns>True if saved correctly, false if otherwise</returns>
        public bool SaveUser(PersonVM user)
        {
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

            List<string> allUserData = File.ReadAllLines(userExportPath).ToList();
            string userData = GetUserInfoLine(user);

            if (user.PartnerInfo != null)
            {
                string partnerData = GetUserInfoLine(user.PartnerInfo);
                string partnerDataPath = string.Format(partnerExportPathFormat, $"{user.PartnerInfo.FirstName}.{Guid.NewGuid()}.txt");
                userData = $"{userData}{partnerDataPath}";

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
