// Namespace imports - bringing in project-specific and external dependencies
using GeekStore.API.Data; // For EF Core DbContexts
using GeekStore.API.Mappings; // For AutoMapper profiles
using GeekStore.API.Repositories; // Repository implementations
using Microsoft.EntityFrameworkCore; // EF Core for database access
using Microsoft.AspNetCore.Authentication.JwtBearer; // JWT bearer authentication
using Microsoft.IdentityModel.Tokens; // For JWT token validation
using System.Text; // For encoding the JWT signing key
using Microsoft.AspNetCore.Identity; // For ASP.NET Identity (User, Role, etc.)
using Microsoft.OpenApi.Models; // For Swagger configuration
using GeekStore.API.Middlewares; // Custom middleware (like error handling)
using Serilog; // Structured logging
using DotNetEnv; // For loading environment variables from .env
using GeekStore.API.Services; // Application service layer
using GroqApiLibrary; // For calling Groq API (LLM/embedding)
using Python.Runtime; // For PythonNET interop
using GeekStore.API.Services.Interfaces; // Service interfaces
using GeekStore.API.Repositories.Interfaces; // Repository interfaces

var builder = WebApplication.CreateBuilder(args);

// Load environment variables from .env file (e.g., connection strings, secrets)
Env.Load();

// Configure Serilog for logging
var logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("Logs/GeekStore_Log.txt", rollingInterval: RollingInterval.Day)
    .MinimumLevel.Information()
    .CreateLogger();

// Replace default logging with Serilog
builder.Logging.ClearProviders();
builder.Logging.AddSerilog(logger);

// Register Swagger and configure it to use JWT auth
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Geek Store API", Version = "v1" });
    options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
    {
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = JwtBearerDefaults.AuthenticationScheme
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = JwtBearerDefaults.AuthenticationScheme
                },
                Scheme = "Oauth2",
                Name = JwtBearerDefaults.AuthenticationScheme,
                In = ParameterLocation.Header
            },
            new List<string>()
        }
    });
});

// Register MVC controllers
builder.Services.AddControllers();

// Register background embedding queue (runs as hosted service)
builder.Services.AddSingleton<EmbeddingQueue>();
builder.Services.AddSingleton<IEmbeddingQueue>(sp => sp.GetRequiredService<EmbeddingQueue>());
builder.Services.AddHostedService(sp => sp.GetRequiredService<EmbeddingQueue>());

// Register PostgreSQL main database with vector support for similarity search
builder.Services.AddDbContext<GeekStoreDbContext>(options => options.UseNpgsql(
    Environment.GetEnvironmentVariable("GeekStoreConnectionString"),
    o => o.UseVector())
);

// Register SQL Server authentication database
builder.Services.AddDbContext<GeekStoreAuthDbContext>(options =>
    options.UseSqlServer(Environment.GetEnvironmentVariable("GeekStoreAuthDbConnectionString")));

// Register application services with scoped lifetime
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IRecommendationService, RecommendationService>();
builder.Services.AddScoped<IEmbeddingService, PythonEmbeddingService>();
builder.Services.AddScoped<ILLMService, GroqLLMService>();

// Register Groq API client with injected API key
builder.Services.AddScoped<GroqApiClient>(sp =>
    new GroqApiClient(Environment.GetEnvironmentVariable("GroqApiKey")));

// Register repositories for clean data access
builder.Services.AddScoped<ITierRepository, SQLTierRepository>();
builder.Services.AddScoped<IProductRepository, SQLProductRepository>();
builder.Services.AddScoped<ICategoryRepository, SQLCategoryRepository>();
builder.Services.AddScoped<ITokenRepository, TokenRepository>();

// Register AutoMapper with your profile mappings
builder.Services.AddAutoMapper(typeof(MappingProfiles));

// Configure ASP.NET Identity for auth and role management
builder.Services.AddIdentityCore<IdentityUser>()
    .AddRoles<IdentityRole>()
    .AddTokenProvider<DataProtectorTokenProvider<IdentityUser>>("GeekStore")
    .AddDefaultTokenProviders()
    .AddEntityFrameworkStores<GeekStoreAuthDbContext>();

// Customize password requirements for Identity
builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 1;
});

// Configure JWT authentication and validation parameters
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = Environment.GetEnvironmentVariable("JWT_Issuer"),
            ValidateAudience = true,
            ValidAudience = Environment.GetEnvironmentVariable("JWT_Audience"),
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                Environment.GetEnvironmentVariable("JWT_Key"))),
            ValidateLifetime = true
        };
    });

var app = builder.Build();

// Enable Swagger only in development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Use global error handling middleware
app.UseMiddleware<ExceptionHandlerMiddleware>();

// Enforce HTTPS
app.UseHttpsRedirection();

// Enable authentication and authorization middlewares
app.UseAuthentication();
app.UseAuthorization();

// Map controller endpoints
app.MapControllers();

// Initialize Python runtime for embedding service
Runtime.PythonDLL = Environment.GetEnvironmentVariable("PythonDLLPath");
_ = PythonEngineSingleton.Instance;

// Ensure Python runtime is shut down on app exit
app.Lifetime.ApplicationStopping.Register(() =>
{
    PythonEngineSingleton.Instance.Shutdown();
});

// Run the web application
app.Run();
