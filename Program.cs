global using dotnet_rpg.Models; //this is visible everywhere (first thing before other usings)
global using dotnet_rpg.Services.CharacterService;
global using dotnet_rpg.Dtos.Character;
global using AutoMapper;
global using Microsoft.EntityFrameworkCore;
global using dotnet_rpg.Data;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// DB
// The lambda expression configures our context to connect to SQLServer DB, this method takes the ConnectionString
builder.Services.AddDbContext<DataContext>(options =>
 options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))); // This is why the convetion "ConnectionStrings" makes sense

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
 // Adds AutoMapper to the dependency injection container. 
 // The typeof(Program).Assembly parameter specifies the assembly where the AutoMapper profiles are located.
builder.Services.AddAutoMapper(typeof(Program).Assembly);

// Inject CharacterService when looking for ICharacterService
// Scoped - new instance of service is created for every request that comes in (stateful within the request)
builder.Services.AddScoped<ICharacterService, CharacterService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
