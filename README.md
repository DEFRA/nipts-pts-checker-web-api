# nipts-pts-checker-web-api

The Pet Travel Scheme Checker API is used by port officials to validate Pet Travel Documents presented by travelers moving from Great Britain to Northern Ireland with pets.

This backend API provides endpoints for verifying document authenticity and status, supporting the operational needs of border control and compliance teams.

## Prerequisites
To work with this project locally, ensure you have the following installed:

.NET SDK 6.0+
Visual Studio 2022 or Visual Studio Code
Access to required environment variables or secrets (e.g., via Azure Key Vault or local settings)

## Setup
1. Clone the repository:
```
git clone https://github.com/DEFRA/nipts-pts-checker-web-api.git
cd nipts-pts-checker-web-api
```

2. Restore dependencies:
```
dotnet restore
```

3. Configure local settings: Create a appsettings.json file in the root with the following structure:
```
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AddressApi": {
    "BaseUrl": "<insert address api here>"
  },
  "ConnectionStrings": {
    "sql_db": "data source=<insert local database here>"

  }
}
```

### Development
To run the API locally:
```
dotnet run
```
Or use Visual Studio to start debugging via F5.

The API will be accessible at http://localhost:5000 or the configured port. Using /swagger will navigate to the API UI.

### Test
Unit tests are located in the /test directory.

Unit and integration tests are located in:

- Defra.PTS.Checker.Web.Api.Tests
- Defra.PTS.Checker.Services.Tests

Ensure all dependencies are restored and the test project builds successfully.

## Running in development
1. Ensure all dependencies are installed.
2. Start the API using dotnet run.
3. Use tools like Postman or Swagger to test endpoints.

## Running tests
Run all tests using:
```
dotnet test
```

## Contributing to this project

Please read the [contribution guidelines](/CONTRIBUTING.md) before submitting a pull request.

## Licence

THIS INFORMATION IS LICENSED UNDER THE CONDITIONS OF THE OPEN GOVERNMENT LICENCE found at:

<http://www.nationalarchives.gov.uk/doc/open-government-licence/version/3>

The following attribution statement MUST be cited in your products and applications when using this information.

>Contains public sector information licensed under the Open Government licence v3

### About the licence

The Open Government Licence (OGL) was developed by the Controller of Her Majesty's Stationery Office (HMSO) to enable information providers in the public sector to license the use and re-use of their information under a common open licence.

It is designed to encourage use and re-use of information freely and flexibly, with only a few conditions.
