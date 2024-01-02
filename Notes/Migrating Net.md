## Migrating

Migrating is very easy to do and it's simple as updating the `TargetFramework` and packages

```diff
<Project Sdk="Microsoft.NET.Sdk.Web">

  <PropertyGroup>
-    <TargetFramework>net7.0</TargetFramework>
+    <TargetFramework>net8.0</TargetFramework>
  </PropertyGroup>

</Project>
```

For more information refer to this link:

[ASP.NET Core migration from 7.0 to 8.0](https://learn.microsoft.com/en-us/aspnet/core/migration/70-80?view=aspnetcore-8.0&tabs=visual-studio)
