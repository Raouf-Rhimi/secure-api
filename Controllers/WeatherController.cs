using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SecureApi.Controllers;

[ApiController]
[Route("api/[controller]")]
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
    public IActionResult GetAdminWeather()
    {
        var userClaims = User.Claims.Select(c => new { c.Type, c.Value }).ToList();
        
        return Ok(new { 
            Message = "Administrator access granted", 
            UserClaims = userClaims,
            SystemData = "Server configurations and user management data" 
        });
    }

    [HttpGet("group-data")]
    public IActionResult GetGroupWeather()
    {
        return Ok(new { 
            Message = "Access granted via group membership", 
            Data = "Group-specific resources" 
        });
    }

    [HttpGet("user-info")]
    public IActionResult GetUserInfo()
    {
        var userInfo = new
        {
            Username = User.Identity?.Name,
            IsAuthenticated = User.Identity?.IsAuthenticated,
            Claims = User.Claims.Select(c => new { c.Type, c.Value }).ToList()
        };

        return Ok(userInfo);
    }
}

public class WeatherForecast
{
    public DateOnly Date { get; set; }
    public int TemperatureC { get; set; }
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
    public string? Summary { get; set; }
}