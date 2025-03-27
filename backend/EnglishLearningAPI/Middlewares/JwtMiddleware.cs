using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

using EnglishLearningAPI.Services;
using EnglishLearningAPI.Models;

public class JwtMiddleware
{
    private readonly RequestDelegate _next;
    private readonly string _secretKey;
    private readonly string _issuer;
    private readonly string _audience;

    public JwtMiddleware(RequestDelegate next, IOptions<JwtSettings> jwtSettings)
    {
        _next = next;
        _secretKey = jwtSettings.Value.SecretKey;
        _issuer = jwtSettings.Value.Issuer;
        _audience = jwtSettings.Value.Audience;
    }

    public async Task Invoke(HttpContext context, AuthService authService)
    {
        var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

        if (token != null)
        {
            var user = ValidateToken(token);
            if (user != null)
                context.Items["User"] = user;
        }

        await _next(context);
    }

    private ClaimsPrincipal? ValidateToken(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
            return null;

        var key = Encoding.UTF8.GetBytes(_secretKey);
        var tokenHandler = new JwtSecurityTokenHandler();
        try
        {
            var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _issuer,
                ValidAudience = _audience,
                IssuerSigningKey = new SymmetricSecurityKey(key)
            }, out _);

            return principal;
        }
        catch
        {
            return null; // Token không hợp lệ
        }
    }
}
