using System.Text;

namespace dotnet_rpg;

public static class ApplicationBuilderExtensions
{
    //// The lambda expression configures our context to connect to SQLServer DB, this method takes the ConnectionString
    public static void AddDefaultDbContext(this IServiceCollection services, string connectionString)
    {
        // DB context setup
        services.AddDbContext<DataContext>(options =>
            options.UseSqlServer(connectionString));
    }

    public static void AddJwt(this IServiceCollection services, IConfiguration configuration)
    {
        // JWT setup
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey =
                    new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(configuration.GetSection("AppSettings:JwtSigningKey").Value!)),
                ValidateIssuer = false,
                ValidateAudience = false
            };
        });
    }

    public static void AddSwagger(this IServiceCollection services)
    {
        // Swagger and SwaggerUI setup
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        services.AddSwaggerGen(c =>
        {
            c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
            {
                Description = """Standared Authorization header using the Bearer scheme. Example: "bearer {token}" """,
                In = ParameterLocation.Header,
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey
            });
            /*
              Add an operation filter to the Swagger configuration.
              An operation filter in Swagger is a piece of code that executes for each operation (like GET, POST, PUT, etc on the API endpoints) when the Swagger documentation is generated.
              This filter makes it easier to work with secured APIs directly from the Swagger UI by adding the option to input the bearer token when required.
             */
            c.OperationFilter<SecurityRequirementsOperationFilter>();
        });
    }

    public static void AddDefaultServices(this IServiceCollection services)
    {
        // Dependency injection setup
        // Inject CharacterService when looking for ICharacterService
        // Scoped - new instance of service is created for every request that comes in (stateful within the request)
        // Transient - new instance every time
        // Singleton - same instance always
        services.AddScoped<ICharacterService, CharacterService>();

        /* More on Scoped
        Scoped is useful when you require a new instance for each user request, making it ideal for handling user-specific operations like authentication,
        ensuring that the operations and data for one user do not interfere with those for another.

        If AuthRepository uses a context like DbContext in Entity Framework, it should be registered as Scoped, because DbContext is not thread-safe.

        However, the choice really depends on what exactly AuthRepository does. If it's stateless, you could even register it as a Singleton for better performance.
        Be careful, though, as singletons can cause thread safety issues unless they are stateless or thread-safe.
        */
        services.AddScoped<IAuthRepository, AuthRepository>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IWeaponService, WeaponService>();
        services.AddScoped<IRepository<Character>, CharacterRepository>();
        services.AddScoped<IRepository<Weapon>, WeaponRepository>();
        services.AddScoped<IRepository<Skill>, SkillRepository>();
        services.AddScoped<IfightService, FightService>();
    }

    public static void AddDefaultAutoMapper(this IServiceCollection services)
    {
        // Adds AutoMapper to the dependency injection container. 
        // The typeof(Program).Assembly parameter specifies the assembly where the AutoMapper profiles are located.
        services.AddAutoMapper(typeof(Program).Assembly);
    }
}