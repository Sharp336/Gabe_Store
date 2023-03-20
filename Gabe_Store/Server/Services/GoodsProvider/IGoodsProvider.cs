using Gabe_Store.Shared;

namespace Gabe_Store.Services.GoodsProvider
{
    public interface IGoodsProvider
    {
        public List<Good> GetAll();
    }
}
