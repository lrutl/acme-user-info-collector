using Acme.UserInfoCollector.Middleware;
using Acme.UserInfoCollector.Middleware.Enumerations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Acme.UserInfoCollector.Test
{
    public class Tests
    {
        private IConfiguration _configuration;
        private IServiceProvider _serviceProvider;

        [SetUp]
        public void Setup()
        {
            _configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", false, true)
                .Build();

            _serviceProvider = new ServiceCollection()
                .AddLogging()
                .AddSingleton(_configuration)
                .AddSingleton<UserExporterService>()
                .AddTransient<PersonVM>()
                .BuildServiceProvider();
        }

        private PersonVM GetJohnDoe()
        {
            var person = _serviceProvider.GetService<PersonVM>();
            Assert.That(person, Is.Not.Null);

            person.FirstName = "John";
            person.Surname = "Doe";
            person.DateOfBirth = DateTime.UnixEpoch;
            person.MaritalStatus = MaritalStatus.Single;

            return person;
        }

        [Test]
        public void SaveUserTest()
        {
            var user = GetJohnDoe();

            var exportService = _serviceProvider.GetService<UserExporterService>();
            Assert.That(exportService, Is.Not.Null);

            exportService.SaveUser(user);

            var filePath = _configuration.GetValue<string>("UserExportPath");
            Assert.That(filePath, Is.Not.Null);

            List<string> allUsers = File.ReadAllLines(filePath).ToList();
            Assert.That(allUsers.Last(), Is.EqualTo("John|Doe|01-01-1970|Single|null|"));
        }

        [Test]
        public void SaveUserPartnerTest()
        {
            var user = GetJohnDoe();
            user.MaritalStatus = MaritalStatus.Married;

            user.PartnerInfo = GetJohnDoe();
            user.PartnerInfo.FirstName = "Jane";
            user.PartnerInfo.MaritalStatus = MaritalStatus.Married;

            var exportService = _serviceProvider.GetService<UserExporterService>();
            Assert.That(exportService, Is.Not.Null);

            exportService.SaveUser(user);

            var filePath = _configuration.GetValue<string>("UserExportPath");
            Assert.That(filePath, Is.Not.Null);

            List<string> allUsers = File.ReadAllLines(filePath).ToList();
            var latestUser = allUsers.Last();
            var partnerInfoPath = latestUser.Split("|").Last();
            string partnerInfo = File.ReadAllText(partnerInfoPath);
            Assert.That(partnerInfo, Is.EqualTo("Jane|Doe|01-01-1970|Married|null|"));

            Assert.Pass();
        }

        [Test]
        public void SaveUserInvalidPartnerTest()
        {
            var user = GetJohnDoe();
            user.MaritalStatus = MaritalStatus.Married;

            var exportService = _serviceProvider.GetService<UserExporterService>();
            Assert.That(exportService, Is.Not.Null);

            bool success = exportService.SaveUser(user);
            Assert.That(success, Is.False);
        }

        [Test]
        public void SaveInvalidUserNameTest()
        {
            var user = GetJohnDoe();
            user.FirstName = "should|fail";
            var exportService = _serviceProvider.GetRequiredService<UserExporterService>();
            bool success = exportService.SaveUser(user);
            Assert.That(success, Is.False);
        }

        [Test]
        public void SaveInvalidUserAgeTest()
        {
            var user = GetJohnDoe();
            user.DateOfBirth = DateTime.Now.AddYears(-14);
            var exportService = _serviceProvider.GetRequiredService<UserExporterService>();
            bool success = exportService.SaveUser(user);
            Assert.That(success, Is.False);
        }

        [Test]
        public void SaveInvalidUserParentalConsentTest()
        {
            var user = GetJohnDoe();
            user.DateOfBirth = DateTime.Now.AddYears(-17);
            user.ParentalConsent = ParentalConsent.No;
            var exportService = _serviceProvider.GetRequiredService<UserExporterService>();
            bool success = exportService.SaveUser(user);
            Assert.That(success, Is.False);
        }
    }
}