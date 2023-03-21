using Gabe_Store.Shared;

namespace Gabe_Store.Services.GoodsProvider
{
    public interface IGoodsProvider
    {
        public List<Good> GetAll();

        public void DeleteGoodById(int id);

        public Good GetGoodById(int id);

        public void Add(Good g);
    }
}
