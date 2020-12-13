# Tax Calculator
Full stack solution to do tax calculations using ASP.NET Core 5.0, Razor pages, EF Core and  Sql Server. Tests were created using NUnit and Moq.
## Requisites
- Install [.Net 5.0 SDK](https://dotnet.microsoft.com/download/dotnet/5.0) or [Visual Studio Version 16.8](https://docs.microsoft.com/en-us/visualstudio/releases/2019/release-notes)
- Install SQL Server Express LocalDB 2016 or higher

## How to run the project from Visual Studio 2019 
- Open visual studio and set TaxCalculator.API and TaxCalculator.UI as start up projects
- Run the solution
- Swagger page comes up for TaxCalculator.API and  the calculator page comes up for the TaxCalculator.UI project
- A database called TaxCalculator is automatically created on you localdb instance. The database is also seeded with the required tax settings. A backup of the database can be found in the root folder of the application.

## Some design patterns utilized
### 1. Rules engine
   The `ValidationRuleEngine.cs` class when setup and injected into a class, it will pick up all rules that match `IValidationRule<in TRequest, TResponse>`. On calling the `ValidationRuleEngine.Validate` method, all matching rules are validated and the combined result is returned in `OperationResult` object. 

### 2. Decorator 
The cached repositories below utilize the decorator pattern by dynamically adding caching abilities to the normal repositories:
- CachedPostalCodeTaxCalculationMappingRepository.cs
- CachedTaxRateSettingRepository.cs
- CachedTaxYearRepository.cs

### 3. Factory Pattern
We pass the `TaxCalculationType` to `TaxCalculatorFactory.cs` so that it creates and returns the right instance of the calculator to use.

### 4. Template Method
The `BaseTaxRateCalculator.cs` class defines the steps that each TaxCalculator should take to calculate the tax amount. The subclasses need to override the `CalculateTax` method with the right algorithim.

### 5. Strategy
`ITaxCalculator.cs` interface allows us to define a family of Tax calculation algorithims that we can use interchangeably depending on the user input.

### 6. Singleton
The caching repositories utilize IMemoryCache which is wired up as a singleton through DI.

## Database Design
- `TaxYears` table contains FromDate and ToDate columns to help us define the tax periods. This allows us to pick the right tax settings (by date) and also be able to  pre-load future tax settings. 
- `TaxCalculations` stores the results of the calculations. The table is a bit denormalized as it is acting as an audit table. This allows you to quickly see the required information without doing multiple joins. 