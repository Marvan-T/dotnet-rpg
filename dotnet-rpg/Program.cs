using dotnet_rpg;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;

// DB
services.AddDefaultDbContext(
    configuration
        .GetConnectionString("DefaultConnection")!);
services.AddControllers();
services.AddEndpointsApiExplorer();
services.AddSwagger();
services.AddDefaultAutoMapper();
services.AddDefaultServices();
services.AddJwt(configuration);
services.AddHttpContextAccessor();

builder.Services.AddLogging();

var app = builder.Build();

// Configure the HTTP request pipeline.

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();