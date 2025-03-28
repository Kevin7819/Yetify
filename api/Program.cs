using api.Data;
using api.Config;
using Microsoft.AspNetCore.Authentication.JwtBearer; 
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using api.Services;
using api.Custome;

var builder = WebApplication.CreateBuilder(args);

// Database context configuration with SQL Server
builder.Services.AddDbContext<ApplicationDBContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// JWT configuration
var jwtSettings = new JwtSettings();

builder.Configuration.GetSection("Jwt").Bind(jwtSettings); // Binds the configuration from appsettings.json to the instance
builder.Services.AddSingleton(jwtSettings); // Registers the configuration

// Authentication configuration
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        // JWT token validation parameters
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,                  // Validates the token issuer
            ValidateAudience = true,                // Validates the token audience
            ValidateLifetime = true,                // Checks if the token has expired
            ValidateIssuerSigningKey = true,        // Validates the token signature
            ValidIssuer = jwtSettings.Issuer,       // Valid issuer
            ValidAudience = jwtSettings.Audience,   // Valid audience
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key)) // Signing key
        };
    });

// Authorization system configuration
builder.Services.AddAuthorization();

builder.Services.AddScoped<AuthService>(); // Authentication service
builder.Services.AddScoped<Utils>();       // Utility service 

builder.Services.AddControllers();          // Enables controllers
builder.Services.AddEndpointsApiExplorer(); // Enables endpoints
builder.Services.AddSwaggerGen();           // Enables Swagger for API documentation

// Builds the application with all configurations
var app = builder.Build();

// Enables Swagger (a tool that automatically documents the API) only in development environment
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();      // Generates a JSON file with the description of all endpoints
    app.UseSwaggerUI();    // Creates an interactive web page to test the API
}

app.UseHttpsRedirection(); // Automatically redirects HTTP to HTTPS

app.UseAuthentication();
app.UseAuthorization();

// Maps the controllers
app.MapControllers();

// Starts the application
app.Run();