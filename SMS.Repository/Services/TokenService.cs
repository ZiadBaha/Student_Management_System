using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SMS.Core.Models.Identity;
using SMS.Core.Services;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SMS.Repository.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;

        public TokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public Task<string> CreateTokenAsync(AppUser user)
        {
            var claims = new List<Claim>
    {
        new Claim("_id", user.Id),
        new Claim("email", user.Email ?? ""),
        new Claim("First_name", $"{user.FirstName} "),
        new Claim("Last_name", $"{user.LastName}"),
        new Claim("role", user.UserRole.ToString()),
        new Claim("isActive", true.ToString().ToLower())
    };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],           
                audience: _configuration["JWT:ValidAudience"],
                claims: claims,
                expires: DateTime.UtcNow.AddDays(double.Parse(_configuration["JWT:DurationInDays"]!)),
                signingCredentials: creds
            );

            return Task.FromResult(new JwtSecurityTokenHandler().WriteToken(token));
        }

    }
}

