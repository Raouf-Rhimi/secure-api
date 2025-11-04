using Microsoft.OpenApi.Models;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Configure Swagger with JWT support
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Secure API", Version = "v1" });

});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => 
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Secure API v1");
        c.RoutePrefix = "swagger"; // Access Swagger UI at /swagger
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


app.MapControllers();

// Add a simple health check endpoint
app.MapGet("/", () => "Secure API is running!");

app.Run();