using Microsoft.AspNetCore.Authentication; // AJOUTER
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Security.Claims;
using System.Text.Json; 

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Configure Swagger with JWT support
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Secure API", Version = "v1" });
    
    // Add JWT Authentication to Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

// Configure Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.Authority = "http://localhost:8087/realms/formation";
    options.Audience = "secure-api";
    options.RequireHttpsMetadata = false;
    
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = "http://localhost:8087/realms/formation",
        NameClaimType = ClaimTypes.Name,
        RoleClaimType = ClaimTypes.Role // IMPORTANT: Dire à .NET d'utiliser les claims de rôle
    };
    
    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            Console.WriteLine($"Authentication failed: {context.Exception.Message}");
            return Task.CompletedTask;
        },
        OnTokenValidated = context =>
        {
            Console.WriteLine("Token validated successfully");
            return Task.CompletedTask;
        }
    };
});

// ENREGISTRER le transformation de claims
builder.Services.AddTransient<IClaimsTransformation, RoleClaimsTransformation>();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireUserRole", policy => 
        policy.RequireRole("User"));

    options.AddPolicy("RequireAdminRole", policy => 
        policy.RequireRole("Admin"));

    options.AddPolicy("RequireManagerRole", policy => 
        policy.RequireRole("Manager"));

    options.AddPolicy("RequireAdministratorsGroup", policy => 
        policy.RequireClaim("groups","Administrators"));

    options.AddPolicy("RequireManagersGroup", policy => 
        policy.RequireClaim("groups","Managers"));
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => 
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Secure API v1");
        c.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();

// Add CORS for development
app.UseCors(policy => 
{
    policy.AllowAnyOrigin();
    policy.AllowAnyMethod();
    policy.AllowAnyHeader();
});

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Add a simple health check endpoint
app.MapGet("/", () => "Secure API is running!");

app.Run();