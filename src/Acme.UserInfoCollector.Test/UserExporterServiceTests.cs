using Acme.UserInfoCollector.Middleware;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Acme.UserInfoCollector.Test
{
    public class Tests
    {
        private IServiceProvider _serviceProvider;

        [SetUp]
        public void Setup()
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", false, true)
                .Build();

            _serviceProvider = new ServiceCollection()
                .AddLogging()
                .AddSingleton<IConfiguration>(configuration)
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
            person.MaritalStatus = Middleware.Enumerations.MaritalStatus.Single;

            return person;
        }

        [Test]
        public void SaveUserTest()
        {
            var user = GetJohnDoe();

            var exportService = _serviceProvider.GetService<UserExporterService>();
            Assert.That(exportService, Is.Not.Null);

            exportService.SaveUser(user);

            Assert.Pass();
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
            user.ParentalConsent = Middleware.Enumerations.ParentalConsent.No;
            var exportService = _serviceProvider.GetRequiredService<UserExporterService>();
            bool success = exportService.SaveUser(user);
            Assert.That(success, Is.False);
        }
    }
}