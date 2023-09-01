# The following links are useful references to good articles on testing

Note:
- In most cases when dealing with action results in ASP.NET Core, they come wrapped in a higher-order type like ActionResult<T>,
  so it's important to assert the type of the actual result that is included in this wrapper.

- Best practices in writing unit tests: https://learn.microsoft.com/en-us/dotnet/core/testing/unit-testing-best-practices
- Writing unit tests in ASP.net core: https://learn.microsoft.com/en-us/aspnet/core/mvc/controllers/testing?view=aspnetcore-7.0