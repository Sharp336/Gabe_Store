using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;
using Gabe_Store.Shared;
using Microsoft.IdentityModel.Tokens;

namespace Gabe_Store.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class GoodsController : ControllerBase
    {
        private readonly IGoodsProvider _goodsProvider;
        private readonly IUsersProvider _usersProvider;

        public GoodsController(IUsersProvider usersProvider, IGoodsProvider goodsProvider)
        {
            _goodsProvider = goodsProvider;
            _usersProvider = usersProvider;
        }

        [HttpGet("get_all")]
        [AllowAnonymous]
        public async Task<ActionResult<List<GoodPublicDto>>> GetAll()
        {
            return _goodsProvider.GetAll().Select(g_obj => (GoodPublicDto)g_obj).ToList();
        }

        [HttpPost("TryAdjustUserBalance")]
        [AllowAnonymous]
        public async Task<ActionResult<string>> TryAdjustUserBalance(IntegerDto data)
        {
            string token = Request.Headers[HeaderNames.Authorization];

            if (string.IsNullOrEmpty(token))
                return BadRequest("Failed to get the token.");

            string username_from_request = GetNameFromJWT(token);

            if (string.IsNullOrEmpty(username_from_request))
                return BadRequest("Failed to parse the token.");

            var user = _usersProvider.TryGetUserByName(username_from_request);

            if (user is null)
                return BadRequest("Failed to find user by name");

            bool a = _usersProvider.TryAdjustUserBalance(username_from_request, data.Value);

            return Ok(a);
        }

        [HttpPost("delete_by_id")]
        [Authorize(Roles = "Seller")]
        public async Task<ActionResult<string>> DeleteGoodById(IntegerDto id)
        {
            string token = Request.Headers[HeaderNames.Authorization];

            if (string.IsNullOrEmpty(token))
                return BadRequest("Failed to get the token.");

            string rname = GetNameFromJWT(token);

            if (string.IsNullOrEmpty(rname))
                return BadRequest("Failed to parse the token.");

            var good = _goodsProvider.GetGoodById(id.Value);


            if (rname != good.SellerName)
                return BadRequest("Only owner can delete his product");

            if (good.IsSold)
                return BadRequest("Product can't be deleted due to already being sold");

            _goodsProvider.DeleteGoodById(id.Value);
            return Ok("Good has been successfuly deleted.");
        }

        [HttpPost("add_good")]
        [Authorize(Roles = "Seller")]
        public async Task<ActionResult<string>> AddGood(Good rgood)
        {
            string token = Request.Headers[HeaderNames.Authorization];

            if (string.IsNullOrEmpty(token))
                return BadRequest("Failed to get the token.");

            string username_from_request = GetNameFromJWT(token);

            if (string.IsNullOrEmpty(username_from_request))
                return BadRequest("Failed to parse the token.");

            if (!rgood.SellerName.IsNullOrEmpty() && username_from_request != rgood.SellerName)
                return BadRequest("Seller actual and good's name are different.");

            rgood.SellerName = username_from_request;
            _goodsProvider.Add(rgood);

            return Ok("Good has been successfuly added.");
        }

        [HttpPost("try_buy_by_id")]
        [Authorize(Roles = "Buyer, Seller")]
        public async Task<ActionResult<string>> TryBuyById(int id)
        {
            string token = Request.Headers[HeaderNames.Authorization];

            if (string.IsNullOrEmpty(token))
                return BadRequest("Failed to get the token.");

            string rname = GetNameFromJWT(token);

            if (string.IsNullOrEmpty(rname))
                return BadRequest("Failed to parse the token.");

            var user = _usersProvider.TryGetUserByName(rname);

            if (user is null)
                return BadRequest("Failed to find user by name.");

            var good = _goodsProvider.GetGoodById(id);

            if (good.IsSold)
                return BadRequest("The good is already sold.");

            if (_usersProvider.TryAdjustUserBalance(user.Username, (int)good.Price * -1))
                return BadRequest("Balance is not enough to buy this");

            user.ProductsBought.Add(good);

            return Ok("Good has been successfuly bought.");
        }


        public static string GetNameFromJWT(string jwt)
        {
            jwt = jwt[7..];
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(jwt);
            var tokenS = jsonToken as JwtSecurityToken;
            var aaa = tokenS.Claims.SingleOrDefault(cl => cl.Type == ClaimTypes.Name).Value;
            return aaa;
        }
    }
}
