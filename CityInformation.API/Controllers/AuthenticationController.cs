using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace CityInformation.API.Controllers
{
    [Route("api/authentication")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public class AuthenticationRequestBody
        {
            public string? UserName { get; set; }
            public string? Password { get; set; }
        }

        private class CityInfoUser
        {
            public int UserId { get; set; }
            public string UserName { get; set; }
            public string FirstName { get; set; }   
            public string LastName { get; set; }
            public string City { get; set; }

            public CityInfoUser(int userId, string userName, string firstName, string lastName, string city)
            {
                UserId = userId;
                UserName = userName;
                FirstName = firstName;
                LastName = lastName;
                City = city;
            }
        }

        public AuthenticationController(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        [HttpPost("authenticate")]
        public ActionResult<string> Authenticate(AuthenticationRequestBody authenticationRequestBody)
        {
            // Step 1: Validate Username/password
            var user = ValidateUserCredentials(
                authenticationRequestBody.UserName,
                authenticationRequestBody.Password
            );

            if (user is null)
            {
                return Unauthorized();
            }
            

            // Step 2: Create a token
            var secretKey = _configuration["Authentication:SecretForKey"];
            SymmetricSecurityKey? securityKey = null;
            if (secretKey is not null)
            {
                securityKey = new SymmetricSecurityKey(Convert.FromBase64String(secretKey));
            }

            var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            // Claims identity of the User
            var claimsForToken = new List<Claim>
            {
                new("sub", user.UserId.ToString()), // username claim
                new("given_name", user.FirstName),
                new("family_name", user.LastName),
                new("city", user.City)
            };

            var jwtSecurityToken = new JwtSecurityToken(
                _configuration["Authentication:Issuer"],
                _configuration["Authentication:Audience"],
                claimsForToken,
                DateTime.UtcNow, // start of validity
                DateTime.UtcNow.AddHours(1), // end of validity
                signingCredentials
            );

            var tokenToReturn = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);

            return Ok(tokenToReturn);
        }

        private CityInfoUser ValidateUserCredentials(string? userName, string? password)
        {
            // We don't have a user DB or table. If you have, check the passed-through
            // username/password against what's stored in the database.
            //
            // For demo purposes, we assume credentials are valid

            // return a new CityInfoUser(values would normally come from your user DB/table)
            return new CityInfoUser(1, userName ?? "", "Ethan", "Alcantara", "Dumagetme");
        }
    }
}
