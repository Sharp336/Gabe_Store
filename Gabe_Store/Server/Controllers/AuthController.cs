using Gabe_Store.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text.Json;

namespace Gabe_Store.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IUsersProvider _usersProvider;

        public AuthController(IConfiguration configuration, IUsersProvider usersProvider)
        {
            _configuration = configuration;
            _usersProvider = usersProvider;
        }

        [HttpPost("register")]
        public async Task<ActionResult<string>> Register(UserRegisterDto request)
        {
            if (_usersProvider.TryGetUserByName(request.Username) is not null)
                return BadRequest("This useername is already taken.");

            _usersProvider.CreateNewUser(request);
            return Ok("User successfuly created.");
        }

        [HttpPost("login")]
        public async Task<ActionResult<string>> Login(UserLoginDto request)
        {
            var _user = _usersProvider.TryGetUserByName(request.Username);

            if (_user is null)
                return BadRequest("User not found.");

            if (!_usersProvider.TryAuthUser(request))
                return BadRequest("Wrong password.");

            string token = CreateToken(_user);

            var refreshToken = GenerateRefreshToken();
            SetRefreshToken(refreshToken, _user);

            return Ok(token);
        }

        [HttpGet("refresh-token")]
        public async Task<ActionResult<string>> RefreshToken()
        {
            var refreshToken = Request.Cookies["refreshToken"];
            string _username = GetNameFromJWT(Request.Headers[HeaderNames.Authorization]);
            var _user = _usersProvider.TryGetUserByName(_username);

            if (_user is null)
                return BadRequest("Failed to find user by name");

            if (!_user.RefreshToken.Equals(refreshToken))
            {
                return Unauthorized("Invalid Refresh Token.");
            }
            else if (_user.TokenExpires < DateTime.Now)
            {
                return Unauthorized("Token expired.");
            }

            string token = CreateToken(_user);
            var newRefreshToken = GenerateRefreshToken();
            SetRefreshToken(newRefreshToken, _user);

            return Ok(token);
        }

        private RefreshToken GenerateRefreshToken()
        {
            var refreshToken = new RefreshToken
            {
                Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
                Expires = DateTime.Now.AddDays(7),
                Created = DateTime.Now
            };

            return refreshToken;
        }

        private void SetRefreshToken(RefreshToken newRefreshToken, User _user)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = newRefreshToken.Expires
            };
            Response.Cookies.Append("refreshToken", newRefreshToken.Token, cookieOptions);

            _user.RefreshToken = newRefreshToken.Token;
            _user.TokenCreated = newRefreshToken.Created;
            _user.TokenExpires = newRefreshToken.Expires;
        }

        private string CreateToken(User user)
        {
            List<Claim> claims = new()
            {
                new (ClaimTypes.Name, user.Username),
                new (ClaimTypes.Role, user.Role),
                new ("Balance", user.Balance.ToString())
            };

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(
                _configuration.GetSection("AppSettings:Token").Value));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds);

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);


            return jwt;
        }

        public static string GetNameFromJWT(string jwt)
        {
            var payload = jwt.Split('.')[1];
            return JsonSerializer.Deserialize<Dictionary<string, object>>(Convert.FromBase64String(payload.Length % 4 == 3 ? payload + "=" : payload + "==")).SingleOrDefault(kvp => kvp.Key == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name").Value.ToString();
        }
    }
}
