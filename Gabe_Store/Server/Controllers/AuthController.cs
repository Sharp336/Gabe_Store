using Gabe_Store.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace Gabe_Store.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IDataStorage _dataStorage;

        public AuthController(IConfiguration configuration, IDataStorage dataStorage)
        {
            _configuration = configuration;
            _dataStorage = dataStorage;
        }

        [HttpPost("register")]
        public async Task<ActionResult<string>> Register(UserLoginDto request)
        {
            if ( _dataStorage.GetUserByName(request.Username) is not null )
                return BadRequest("This useername is already taken.");
            
            _dataStorage.CreateNewUser(request);
            return Ok("User successfuly created.");
        }

        [HttpPost("login")]
        public async Task<ActionResult<string>> Login(UserLoginDto request)
        {
            var _user = _dataStorage.GetUserByName(request.Username);

            //if (_user is null )
            //    return BadRequest("User not found.");


            if (_user is null)
                return BadRequest(_dataStorage.GetUsersCount().ToString());

            if ( !_dataStorage.TryAuthUser(request) )
                return BadRequest("Wrong password.");

            string token = CreateToken(_user);

            var refreshToken = GenerateRefreshToken();
            SetRefreshToken(refreshToken, _user);

            return Ok(token);
        }

        [HttpPost("refresh-token")]
        public async Task<ActionResult<string>> RefreshToken(UserLoginDto request)
        {
            var refreshToken = Request.Cookies["refreshToken"];
            var _user = _dataStorage.GetUserByName(request.Username);

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
                new (ClaimTypes.Name, user.Username)
            };

            foreach (string role in user.Roles)
                claims.Add(new (ClaimTypes.Role, role));

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

    }
}
