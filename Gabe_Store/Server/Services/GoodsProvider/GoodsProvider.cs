namespace Gabe_Store.Services.GoodsProvider
{
    public class GoodsProvider : IGoodsProvider
    {
        private List<Good> _goodsStorage = new()
        {
            //new()
            //{
            //    Id = 1,
            //    Name = "TestGood",
            //    Description = "A good that is added for test",
            //    Product = "SNIFF BEBRA",
            //    SellerName = "Not existing seller",
            //    Launcher = Launchers.None,
            //    Price = 777,
            //    IsSold = false
            //}
        };

        private int maxindex = 1;

        public List<Good> GetAll() => _goodsStorage.Where(g => !g.IsSold).ToList();

        public void DeleteGoodById(int id)
        {
            _goodsStorage.RemoveAll(sg => sg.Id == id);
        }

        public Good GetGoodById(int id)
        {
            return _goodsStorage.SingleOrDefault(sg => sg.Id == id);
        }
        public void Add(Good g)
        {
            g.Id = maxindex + 1;
            maxindex++;
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
