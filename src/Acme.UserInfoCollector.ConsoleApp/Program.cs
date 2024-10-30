using Acme.UserInfoCollector.ConsoleApp;
using Acme.UserInfoCollector.Middleware;
using Acme.UserInfoCollector.Middleware.Enumerations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", false, true)
    .Build();

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(configuration)
    .CreateLogger();

var serviceProvider = new ServiceCollection()
    .AddLogging(o => o.AddSerilog(Log.Logger))
    .AddSingleton<IConfiguration>(configuration)
    .AddSingleton<UserExporterService>()
    .AddTransient<PersonVM>()
    .BuildServiceProvider();

var user = serviceProvider.GetRequiredService<PersonVM>();
string genericInputError = "Invalid entry";

Console.WriteLine("Hello, this is an application meant to collect and store basic data about its users.");
Console.WriteLine();

user.GetFromConsole(nameof(user.FirstName),
    o => o!, "Please enter your first name...",
    genericInputError);

user.GetFromConsole(nameof(user.Surname),
    o => o!,
    "Please enter your surname...",
    genericInputError);

user.GetFromConsole(nameof(user.DateOfBirth),
    o => DateTime.Parse(o!),
    "Please enter your date of birth in MM/DD/YR format...",
    genericInputError);

var parentConsentPromptMessages = new List<string>()
{
    "For users under the age of 16, parental consent is required. Please have a parental figure answer the following.",
    "Do you consent for your child to use this application?",
    "1. Yes",
    "2. No"
};

if (user.DateOfBirth.AddYears(18) > DateTime.UtcNow)
{
    user.GetFromConsole(nameof(user.ParentalConsent),
        o => (ParentalConsent)Enum.Parse(typeof(ParentalConsent), o!),
        parentConsentPromptMessages,
        genericInputError);
}

var maritalStatusPromptMessages = new List<string>()
{
    "Please enter your marital status...",
    "1. Married",
    "2. Widowed",
    "3. Divorced",
    "4. Separated",
    "5. Never married",
    "6. Living with partner but not married"

};

user.GetFromConsole(nameof(user.MaritalStatus),
    o => (MaritalStatus)int.Parse(o!),
    maritalStatusPromptMessages,
    genericInputError);

if (user.IsLegallyMarried)
{
    user.PartnerInfo = serviceProvider.GetRequiredService<PersonVM>();
    user.PartnerInfo.GetFromConsole(nameof(user.PartnerInfo.FirstName),
        o => o!, "Please enter your partner's first name...",
        genericInputError);

    user.PartnerInfo.GetFromConsole(nameof(user.PartnerInfo.Surname),
        o => o!,
        "Please enter your partner's surname...",
        genericInputError);

    user.PartnerInfo.GetFromConsole(nameof(user.PartnerInfo.DateOfBirth),
        o => DateTime.Parse(o!),
        "Please enter your partner's date of birth in MM/DD/YR format...",
        genericInputError);

    if (user.PartnerInfo.DateOfBirth.AddYears(18) > DateTime.UtcNow)
    {
        user.PartnerInfo.GetFromConsole(nameof(user.PartnerInfo.ParentalConsent),
            o => (ParentalConsent)Enum.Parse(typeof(ParentalConsent), o!),
            parentConsentPromptMessages,
            genericInputError);
    }
}

var exportService = serviceProvider.GetRequiredService<UserExporterService>();
bool exported = exportService.SaveUser(user);

if (exported)
{
    Console.WriteLine("User successfully exported; you can now close this application...");
}
else
{
    Console.WriteLine("Failed to export user; consult logs for more information.");
}