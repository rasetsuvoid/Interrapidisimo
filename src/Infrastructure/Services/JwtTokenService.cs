using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Interrapidisimo.Application.Common.Interfaces;
using Interrapidisimo.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Interrapidisimo.Infrastructure.Services;

public class JwtTokenService(IConfiguration configuration) : IJwtTokenService
{
    public (string Token, DateTime ExpiresAt) GenerateToken(Student student)
    {
        var issuer = configuration["Jwt:Issuer"] ?? "Interrapidisimo";
        var audience = configuration["Jwt:Audience"] ?? "Interrapidisimo.Frontend";
        var secret = configuration["Jwt:Secret"]
            ?? throw new InvalidOperationException("JWT secret is not configured.");
        var expiresMinutes = int.TryParse(configuration["Jwt:ExpiresMinutes"], out var parsedMinutes)
            ? parsedMinutes
            : 120;

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, student.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, student.Email),
            new Claim(JwtRegisteredClaimNames.UniqueName, student.FullName),
            new Claim("studentId", student.Id.ToString()),
            new Claim("studentCode", student.StudentCode)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expiresAt = DateTime.UtcNow.AddMinutes(expiresMinutes);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: expiresAt,
            signingCredentials: credentials);

        return (new JwtSecurityTokenHandler().WriteToken(token), expiresAt);
    }
}
