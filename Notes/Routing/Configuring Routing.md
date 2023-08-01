# Configuring routing 

A specific pattern has to be followed when configuring routing. This explains the pattern and the reasons behind it.

```csharp
app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();
```

- The ideal approach is to use app.UseRouting() followed by authentication, authorization and then app.MapControllers(). This is the recommended order of middleware components for a web API that uses endpoint routing in ASP.NET Core 7. Endpoint routing is a feature that allows you to map requests to endpoints, which are units of executable code that handle requests. Endpoints can be defined using attributes or conventions, and can have metadata associated with them, such as authorization policies or CORS policies.


- `app.MapControllers()` is a helper method introduced in ASP.NET 7 that simplifies the configuration of attribute routing and provides the full functionality and flexibility of endpoint routing. It is equivalent to calling `app.UseEndpoints(endpoints => endpoints.MapControllers())` and is the preferred way of registering routes in minimal API applications in ASP.NET 7. It implicitly calls `app.UseRouting()` under the hood, which means that it matches the request to an endpoint based on the route template.
  
- The caveats of using `app.MapControllers()` without `app.UseRouting()` are:

  - There are middleware components that depend on endpoint information, such as authentication or authorization. These middleware components need to access the HttpContext object to get the user and route information for their decisions. However, if you don't use `app.UseRouting()`, then the HttpContext object will not have any endpoint information set by it. Therefore, the middleware components that depend on endpoint information will not be able to access it and will throw an exception. This is why you need to use `app.UseRouting()` before any middleware that depends on endpoint information.
  - You won't be able to use convention-based routing, which allows you to define routes using conventions (such as **MapGet** or **MapPost**) on the app. Convention-based routing can be useful for **creating simple routes without controllers or actions**. However, if you don't use `app.UseRouting()`, then you won't be able to use `app.MapGet()`, `app.MapPost()`, or any other convention-based methods, because they require the app.UseRouting() method to match the request to an endpoint based on the route template.
  - You won't be able to use endpoint middleware, which allows you to apply middleware components (such as authentication or authorization) to specific endpoints or groups of endpoints. Endpoint middleware can help you customize the request pipeline for your endpoints, such as adding logging or error handling. However, if you don't use `app.UseRouting()`, then you won't be able to use `endpoints.Map()`, `endpoints.MapGet()`, `endpoints.MapPost()`, or any other endpoint middleware methods, because they require the `app.UseRouting()` method to match the request to an endpoint based on the route template.



## Note well though
It is not necessary to use `app.UseRouting()` before calling `app.MapControllers()`. This is because `app.MapControllers()` implicitly calls `app.UseRouting()` under the hood. However, if you are using other middleware components that depend on endpoint information, such as authentication or authorization, then you still need to call `app.UseRouting()` before those middleware components as explained above.