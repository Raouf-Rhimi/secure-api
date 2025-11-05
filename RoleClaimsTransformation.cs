using Microsoft.AspNetCore.Authentication; // AJOUTEZ CETTE LIGNE
using System.Security.Claims;
using System.Text.Json;

public class RoleClaimsTransformation : IClaimsTransformation
{
    public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        var identity = principal.Identity as ClaimsIdentity;
        
        // Vérifier si déjà transformé
        if (identity?.HasClaim("processed", "true") == true)
            return Task.FromResult(principal);

        // when decoding the token with jwt.io roles is present under realm_access
        var realmAccessClaim = identity?.FindFirst("realm_access");
        if (realmAccessClaim != null)
        {
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

            // Deserialize the realm_access JSON to extract the roles
            var realmAccess = JsonSerializer.Deserialize<RealmAccess>(realmAccessClaim.Value, options);

            if (realmAccess?.Roles != null)
            {
                foreach (var role in realmAccess.Roles)
                {
                    // Add each role as a Claim of type ClaimTypes.Role
                    identity.AddClaim(new Claim(ClaimTypes.Role, role));
                }
            }
        }

        // Marquer comme traité pour éviter les transformations multiples
        identity?.AddClaim(new Claim("processed", "true"));
        
        return Task.FromResult(principal);
    }

    public class RealmAccess 
    { 
        public List<string>? Roles { get; set; } 
    }
}