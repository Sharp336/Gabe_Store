using Gabe_Store.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using System.Text.Json;
using static System.Net.WebRequestMethods;

namespace Gabe_Store.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class GoodsController : ControllerBase
    {
        private readonly IUsersProvider _usersProvider;
        private readonly IGoodsProvider _goodsProvider;

        public GoodsController(IUsersProvider usersProvider, IGoodsProvider goodsProvider)
        {
            _usersProvider = usersProvider;
            _goodsProvider = goodsProvider;
        }

        //[HttpGet("get_all")]
        //[AllowAnonymous]
        //public async Task<ActionResult<List<Good>>> GetAll()
        //{
        //    return _goodsProvider.GetAll();
        //}

        [HttpGet("get_all")]
        [AllowAnonymous]
        public async Task<ActionResult<string>> GetAll()
        {
            string token = Request.Headers[HeaderNames.Authorization];
            if (!string.IsNullOrEmpty(token))
            {
                var identity = GetFuckingNameFromJWT(token);
                return identity;
            }
            return BadRequest();
        }

        //[HttpPost("delete_by_id")]
        //[Authorize(Roles = "Seller")]
        //public async Task<ActionResult<string>> DeleteGoodById(Good request)
        //{
        //    string token = Request.Headers[HeaderNames.Authorization];

        //    if (!string.IsNullOrEmpty(token))
        //    {
        //        var identity = new List<Claim>(ParseClaimsFromJwt(token));
        //    }

        //    _goodsProvider.DeleteGoodById(request.Id);
        //    return Ok("Good has been successfuly deleted.");
        //}

        public static IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
        {
            var payload = jwt.Split('.')[1];
            var jsonBytes = ParseBase64WithoutPadding(payload);
            var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);
            return keyValuePairs.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString()));
        }

        private static byte[] ParseBase64WithoutPadding(string base64)
        {
            switch (base64.Length % 4)
            {
                case 2: base64 += "=="; break;
                case 3: base64 += "="; break;
            }
            return Convert.FromBase64String(base64);
        }

        private static int IHIHIHAHA(string base64)
        {
            switch (base64.Length % 4)
            {
                case 2: return 2;
                case 3: return 3;
            }
            return 0;
        }

        public static string GetFuckingNameFromJWT(string jwt)
        {
            var payload = jwt.Split('.')[1];
            var jsonBytes = ParseBase64WithoutPadding(payload);
            var brusko = IHIHIHAHA(payload);
            var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);
            var dfnaWSikujf = keyValuePairs.SingleOrDefault(kvp => kvp.Key == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name").Value.ToString();
            return brusko.ToString();
        }
    }
}
