using Microsoft.IdentityModel.Tokens;
using SimpleLoginSystem.Objects;
using SimpleLoginSystem.Settings;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace SimpleLoginSystem.Services
{
    public class TokenService
    {
        internal string GenerateToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var key = ApiSettings.GenerateSecretByte();

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Username ?? string.Empty),
                    new Claim(ClaimTypes.Role, user.Role ?? string.Empty),
                }),
                Expires = DateTime.UtcNow.AddMinutes(30),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}
