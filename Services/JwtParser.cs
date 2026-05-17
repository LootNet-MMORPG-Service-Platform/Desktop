using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using desktop_app.Models;
using System;
using System.Linq;

namespace desktop_app.Services;

public class JwtParser
{
    public AuthenticatedUser? Parse(string token)
    {
        var handler = new JwtSecurityTokenHandler();

        if (!handler.CanReadToken(token))
            return null;

        var jwt = handler.ReadJwtToken(token);

        var userId = jwt.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value;
        var username = jwt.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
        var roleValue = jwt.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

        if (string.IsNullOrWhiteSpace(userId) ||
            string.IsNullOrWhiteSpace(username) ||
            string.IsNullOrWhiteSpace(roleValue))
        {
            return null;
        }

        if (!Enum.TryParse<UserRole>(roleValue, out var role))
            return null;

        return new AuthenticatedUser
        {
            UserId = userId,
            Username = username,
            Role = role
        };
    }
}