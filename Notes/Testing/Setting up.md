# Setting up unit tests

The general structure of the project that include tests should be as follows:

```text
/prime-service-project
    prime-service-project.sln
    /PrimeService
        PrimeService.cs
        PrimeService.csproj
    /PrimeService.Tests
        PrimeService_IsPrimeShould.cs
        PrimeServiceTests.csproj
```
The setup instructions are provided in here: https://learn.microsoft.com/en-us/dotnet/core/testing/unit-testing-with-dotnet-test

In addition, you need the following libraries:
1. Moq - for mocking dependencies
2. FluentAssertions - nice to have, it makes the assertions more readable 


<br />
_side note: If you are introducing new test project and doing a folder restructure make sure to update the `.sln` files to point to right `.csproj` files._ 
