using Gabe_Store.Shared;
using Microsoft.AspNetCore.Mvc;

namespace Gabe_Store.Services.GoodsProvider
{
    public class GoodsProvider : IGoodsProvider
    {
        private List<Good> _goodsStorage = new();

        public List<Good> GetAll() => _goodsStorage;

        public void DeleteGoodById(int id)
        {
            _goodsStorage.RemoveAll(sg => sg.Id == id);
        }

        public Good GetGoodById(int id)
        {
            return _goodsStorage.SingleOrDefault(sg => sg.Id == id) ?? new Good();
        }
        public void Add(Good g)
        {
            _goodsStorage.Add(g);
        }

        public void Sell(int id)
        {
            var g = GetGoodById(id);
            if (g != null)
                g.IsSold = true;

        }

    }
}
