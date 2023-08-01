# Configuring the middleware for JWT Authentication

To use JWT authentication we have to let the application know that we will be using it. For configuring `Jwt Authentication` a package is used

<link src="https://www.nuget.org/packages/Microsoft.AspNetCore.Authentication.JwtBearer/8.0.0-preview.6.23329.11">


## Snippet for configuration

This is specified in the `Program.cs` file

```csharp
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8
        .GetBytes(builder.Configuration.GetSection("AppSettings:Token").Value!)),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

app.UseAuthentication();

app.UseAuthorization();
```
**What is happening here**
is configuring the authentication service to use the JwtBearer scheme, which means that the web API will expect the client to send a JWT in the Authorization header of each request. The AddJwtBearer method takes an options parameter, which allows you to customize how the JWT is validated by the server. The options.TokenValidationParameters property is an object that specifies various validation rules for the JWT, such as:

- ValidateIssuerSigningKey: This is a boolean value that indicates whether the server should verify that the JWT was signed by a trusted issuer using a valid key. If this is true, then the server will use the IssuerSigningKey property to check the signature of the JWT.
- IssuerSigningKey: This is an object that represents the secret key that was used to sign the JWT by the issuer. In your code snippet, this is set to a `SymmetricSecurityKey` object, which is a type of key that uses a single secret value for both signing and verifying. The secret value is obtained from the configuration file (appsettings.json) using the `builder.Configuration.GetSection` method. The secret value is encoded as a UTF-8 string and converted to a byte array using `System.Text.Encoding.UTF8.GetBytes`.
- ValidateIssuer: This is a boolean value that indicates whether the server should validate that the issuer of the JWT (the entity that created and signed it) is a trusted one. If this is true, then the server will use the ValidIssuer property to check the issuer of the JWT.
- ValidIssuer: This is a string value that specifies the expected issuer of the JWT. For example, if your web API uses Auth0 as an identity provider, then this could be set to your Auth0 domain name (such as https://example.auth0.com).
- ValidateAudience: This is a boolean value that indicates whether the server should validate that the audience of the JWT (the intended recipient of it) matches the web API. If this is true, then the server will use the ValidAudience property to check the audience of the JWT.
- ValidAudience: This is a string value that specifies the expected audience of the JWT. For example, if your web API has an identifier (such as https://example.com/api), then this could be set to that identifier.

By setting these validation parameters, you are telling the server how to verify that the JWTs it receives are authentic and authorized for your web API. If any of these validation rules fail, then the server will reject the request and return an HTTP 401 Unauthorized response.


<blockquote class="callout">
    Read configuring routing as this is relevant to configuration of authentication and authorisation. 
</blockquote>

<br />

### See [Routing](../Routing/Configuring%20Routing.md)


