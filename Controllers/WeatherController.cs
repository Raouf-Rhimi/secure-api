using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SecureApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class WeatherController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    [HttpGet("public")]
    public IActionResult GetPublicWeather()
    {
        return Ok(new { message = "This is public data", data = "Public weather info" });
    }

    [HttpGet("user-data")]
    [Authorize(Policy = "RequireUserRole")]
    public IActionResult GetUserWeather()
    {
        var userClaims = User.Claims.Select(c => new { c.Type, c.Value }).ToList();
        
        var weatherData = Enumerable.Range(1, 5).Select(index => new WeatherForecast
        {
            Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        }).ToArray();

        return Ok(new { 
            Message = "User-specific weather data", 
            UserClaims = userClaims,
            WeatherData = weatherData 
        });
    }

    [HttpGet("manager-data")]
    [Authorize(Policy = "RequireManagerRole")]
    public IActionResult GetManagerWeather()
    {
        var userClaims = User.Claims.Select(c => new { c.Type, c.Value }).ToList();
        
        return Ok(new { 
            Message = "Manager-level data access", 
            UserClaims = userClaims,
            SensitiveData = "Confidential business metrics" 
        });
    }

    [HttpGet("admin-data")]
    [Authorize(Policy = "RequireAdminRole")]
    public IActionResult GetAdminWeather()
    {
        var userClaims = User.Claims.Select(c => new { c.Type, c.Value }).ToList();
        
        return Ok(new { 
            Message = "Administrator access granted", 
            UserClaims = userClaims,
            SystemData = "Server configurations and user management data" 
        });
    }

    [HttpGet("administrators-group")]
    [Authorize(Policy = "RequireAdministratorsGroup")]
    public IActionResult GetAdministratorsGroupWeather()
    {
        return Ok(new { 
            Message = "Access granted to Administrators group members only", 
            Data = "Group-specific resources" 
        });
    }

    [HttpGet("managers-group")]
    [Authorize(Policy = "RequireManagersGroup")]
    public IActionResult GetManagersGroupWeather()
    {
        return Ok(new { 
            Message = "Access granted to managers group members only", 
            Data = "Group-specific resources" 
        });
    }
}

public class WeatherForecast
{
    public DateOnly Date { get; set; }
    public int TemperatureC { get; set; }
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
    public string? Summary { get; set; }
}