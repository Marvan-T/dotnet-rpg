There are quite a few advantages but some of the more interesting ones are as follows


- Flexibility and Encapsulation: Using interfaces for mocking can give you more flexibility because it forces you to test against the contract defined by the interface, rather than the implementation details of a class. It encourages good encapsulation by preventing your tests from knowing about the inner workings of the classes they are testing.
(i.e. helps you to focus your tests on what really matters)

- Dependency Injection: In .NET specifically, interfaces are often used with dependency injection. You can configure your DI container to provide a specific implementation of an interface whenever a class requests an instance of that interface in its constructor. This makes it easy to manage and control the lifetime and scope of services in your application.
(i.e. you can easily swap implementations)

``` csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddScoped<ICharacterService, CharacterService>(); //CharacterServiceV2 for example in one location
    // Other service registrations...
}
```