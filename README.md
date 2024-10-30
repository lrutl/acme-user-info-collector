# acme-user-info-collector

This repository stores a dotnet application meant to collect and store basic user data.

## Services and view models

The `Acme.UserInfoCollector.Middleware` project offers a service and view model that are consumable via depenedency injection.

The `PersonVM` view model contains fields necessary for serialization of a person and validation that a given person's info is valid and exportable.

The `UserExporterService` service contains methods to export a given person's info to a pipe-delineated file store.

The services can be used like so:
```
var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", false, true)
    .Build();

_serviceProvider = new ServiceCollection()
    .AddLogging()
    .AddSingleton(configuration)
    .AddSingleton<UserExporterService>()
    .AddTransient<PersonVM>()
    .BuildServiceProvider();
```

## Configuration

The two services can be configured using Microsoft's `IConfiguration` interface and dependency injection.

For an application using an `appsettings.json` file, the minimum necessary configuration would look like this:

```
{
  "UserExportPath": "./users/mainfile.txt",
  "UserPartnerExportPath": "./users/partners/{0}.txt"
}
```
