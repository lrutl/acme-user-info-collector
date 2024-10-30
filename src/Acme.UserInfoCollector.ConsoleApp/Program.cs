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

user.FirstName = Prompter.GetFromUser(o => o!, "Please enter your first name...", genericInputError);
user.Surname = Prompter.GetFromUser(o => o!, "Please enter your surname...", genericInputError);
user.DateOfBirth = Prompter.GetFromUser(o => DateTime.Parse(o!), "Please enter your date of birth in MM/DD/YR format...", genericInputError);

var maritalStatusPromptMessages = new List<string>()
{
    "Please enter your marital status..."
};

foreach (int maritalStatus in typeof(MaritalStatus).GetEnumValues())
{
    maritalStatusPromptMessages.Add($"{maritalStatus + 1}. {((MaritalStatus)maritalStatus)}");
}

user.MaritalStatus = Prompter.GetFromUser(o => (MaritalStatus)int.Parse(o!) - 1, maritalStatusPromptMessages, genericInputError);

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