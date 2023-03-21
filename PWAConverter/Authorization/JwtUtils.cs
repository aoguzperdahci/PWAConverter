using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using PWAConverter.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using PWAConverter.Helpers;
using Microsoft.Extensions.Configuration;

namespace PWAConverter.Authorization
{
    public interface IJwtUtils
    {
        public bool ValidateToken(string token);
        public string CreateToken(User user);
    }

    public class JwtUtils : IJwtUtils
    {
        private readonly IConfiguration _configuration;

        public JwtUtils(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public string CreateToken(User user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier,user.Id.ToString())
            };

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(
                _configuration.GetSection("Secret:Key").Value));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                
                claims: claims,
                expires: DateTime.Now.AddDays(7),
                signingCredentials: creds);

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }

        public bool ValidateToken(string token)
        {
            if (token == null)
                return false;

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(
                _configuration.GetSection("Secret:Key").Value));
            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = key,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime= true, 
                    // set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                
                // return true if validation successful
                return true;
            }
            catch
            {
                // return null if validation fails
                return false;
            }
        }
    }
}
