﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using System.Text.Json;
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
        public async Task<ActionResult<List<Good>>> GetAll()
        {
            return _goodsProvider.GetAll();
        }

        [HttpPost("TryAdjustUserBalance")]
        [AllowAnonymous]
        public async Task<ActionResult<string>> TryAdjustUserBalance(long amount)
        {
            string token = Request.Headers[HeaderNames.Authorization];

            if (string.IsNullOrEmpty(token))
                return BadRequest("Failed to get the token.");

            string rname = GetNameFromJWT(token);

            if (string.IsNullOrEmpty(rname))
                return BadRequest("Failed to parse the token.");

            var user = _usersProvider.TryGetUserByName(rname);

            if (user is null)
                return BadRequest("Failed to find user by name");

            bool a = _usersProvider.TryAdjustUserBalance(rname, amount);
            return Ok(a);
        }

        [HttpGet("get_balance")]
        [Authorize(Roles = "Buyer, Seller")]
        public async Task<ActionResult<uint>> GetBalance()
        {
            string token = Request.Headers[HeaderNames.Authorization];

            if (string.IsNullOrEmpty(token))
                return BadRequest("Failed to get the token.");

            string rname = GetNameFromJWT(token);

            if (string.IsNullOrEmpty(rname))
                return BadRequest("Failed to parse the token.");

            var user = _usersProvider.TryGetUserByName(rname);

            if (user is null)
                return BadRequest("Failed to find user by name");

            return Ok(user.Balance);
        }

        [HttpPost("delete_by_id")]
        [Authorize(Roles = "Seller")]
        public async Task<ActionResult<string>> DeleteGoodById(int id)
        {
            string token = Request.Headers[HeaderNames.Authorization];

            if (string.IsNullOrEmpty(token))
                return BadRequest("Failed to get the token.");

            string rname = GetNameFromJWT(token);

            if (string.IsNullOrEmpty(rname))
                return BadRequest("Failed to parse the token.");

            var good = _goodsProvider.GetGoodById(id);

            if (rname != good.SellerName)
                return BadRequest("Only owner can delete his product");

            if (good.IsSold)
                return BadRequest("Product can't be deleted due to already being sold");

            _goodsProvider.DeleteGoodById(id);
            return Ok("Good has been successfuly deleted.");
        }

        [HttpPost("add_good")]
        [Authorize(Roles = "Seller")]
        public async Task<ActionResult<string>> AddGood(Good rgood)
        {
            string token = Request.Headers[HeaderNames.Authorization];

            if (string.IsNullOrEmpty(token))
                return BadRequest("Failed to get the token.");

            string rname = GetNameFromJWT(token);

            if (string.IsNullOrEmpty(rname))
                return BadRequest("Failed to parse the token.");

            if (rname != rgood.SellerName)
                return BadRequest("Seller actual and good's name are different.");

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
                return BadRequest("Failed to find user by name");

            var good = _goodsProvider.GetGoodById(id);

            if (good.IsSold)
                return BadRequest("The good is already sold");

            if (_usersProvider.TryAdjustUserBalance(user.Username, good.price * -1))
                return BadRequest("Balance is not enough to buy this");

            user.ProductsBought.Add(good);

            return Ok("Good has been successfuly bought.");
        }


        public static string GetNameFromJWT(string jwt)
        {
            var payload = jwt.Split('.')[1];
            return JsonSerializer.Deserialize<Dictionary<string, object>>(Convert.FromBase64String(payload)).SingleOrDefault(kvp => kvp.Key == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name").Value.ToString();
        }
    }
}
