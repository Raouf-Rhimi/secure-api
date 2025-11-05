using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace SecureApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UserController : ControllerBase
{
    [HttpGet("profile")]
    public IActionResult GetProfile()
    {
        var userData = new
        {
            UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value,
            Username = User.FindFirst(ClaimTypes.Name)?.Value,
            Email = User.FindFirst(ClaimTypes.Email)?.Value,
            Roles = User.FindAll(ClaimTypes.Role).Select(c => c.Value),
            Groups = User.FindAll("groups").Select(c => c.Value),
            AllClaims = User.Claims.Select(c => new { c.Type, c.Value })
        };

        return Ok(userData);
    }

    [HttpGet("roles")]
    [AllowAnonymous]
    public IActionResult GetRoles()
    {
        var roles = User.FindAll(ClaimTypes.Role).Select(c => c.Value);
        return Ok(new { UserRoles = roles });
    }

    [HttpGet("groups")]
    public IActionResult GetGroups()
    {
        var groups = User.FindAll("groups").Select(c => c.Value);
        return Ok(new { UserGroups = groups });
    }
}