using Gabe_Store.Shared;

namespace Gabe_Store.Services.GoodsProvider
{
    public class GoodsProvider : IGoodsProvider
    {
        private List<Good> _goodsStorage = new();

        public List<Good> GetAll() => _goodsStorage;
    }
}
