# Configuring routing 

A specific pattern has to be followed when configuring routing. This explains the pattern and the reasons behind it.

```csharp
app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});
```

- The ideal approach is to use `app.UseRouting()` followed by authentication, authorization and then `app.UseEndpoints(endpoints => endpoints.MapControllers())`. This is the recommended order of middleware components for a web API that uses endpoint routing in ASP.NET Core 7. Endpoint routing is a feature that allows you to map requests to endpoints, which are units of executable code that handle requests. Endpoints can be defined using attributes or conventions, and can have metadata associated with them, such as authorization policies or CORS policies.


- `app.MapControllers()` is a helper method that simplifies the configuration of attribute routing, but it does not provide the full functionality and flexibility of endpoint routing. It is a shortcut for `app.UseEndpoints(endpoints => endpoints.MapControllers())`, which means that it implicitly calls `app.UseEndpoints()` under the hood. However, it does not call `app.UseRouting()` explicitly, which means that it does not match the request to an endpoint based on the route template.
  
- The caveats of using `app.MapControllers()` without `app.UseRouting()` are:

  - There are middleware components that depend on endpoint information, such as authentication or authorization. These middleware components need to access the HttpContext object to get the user and route information for their decisions. However, if you don't use `app.UseRouting()`, then the HttpContext object will not have any endpoint information set by it. Therefore, the middleware components that depend on endpoint information will not be able to access it and will throw an exception. This is why you need to use `app.UseRouting()` before any middleware that depends on endpoint information.
  - You won't be able to use convention-based routing, which allows you to define routes using conventions (such as **MapGet** or **MapPost**) on the app. Convention-based routing can be useful for **creating simple routes without controllers or actions**. However, if you don't use `app.UseRouting()`, then you won't be able to use `app.MapGet()`, `app.MapPost()`, or any other convention-based methods, because they require the app.UseRouting() method to match the request to an endpoint based on the route template.
  - You won't be able to use endpoint middleware, which allows you to apply middleware components (such as authentication or authorization) to specific endpoints or groups of endpoints. Endpoint middleware can help you customize the request pipeline for your endpoints, such as adding logging or error handling. However, if you don't use `app.UseRouting()`, then you won't be able to use `endpoints.Map()`, `endpoints.MapGet()`, `endpoints.MapPost()`, or any other endpoint middleware methods, because they require the `app.UseRouting()` method to match the request to an endpoint based on the route template.


## Using `app.useRouting` with `app.MapControllers()`

You can use `app.UseRouting()` and then `app.MapControllers()` instead of `app.UseEndpoints(endpoints => endpoints.MapControllers())` in your web API. That would achieve the same result of mapping your attribute-routed controllers to endpoints, but it would not be the best practice.

-  `app.MapControllers()` is a helper method that simplifies the configuration of attribute routing, but it does not provide the full functionality and flexibility of endpoint routing. 


It is better to use `app.UseEndpoints()` explicitly in your code, because it allows you to use other features of endpoint routing, such as convention-based routing and endpoint middleware. Convention-based routing allows you to define routes using conventions (such as **MapGet** or **MapPost**) on the app. Endpoint middleware allows you to apply middleware components (such as authentication or authorization) to specific endpoints or groups of endpoints.

By using `app.UseEndpoints()` explicitly, you can also make your code more consistent and readable, as it clearly shows the order of middleware components in your web API. 