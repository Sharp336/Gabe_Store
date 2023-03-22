namespace Gabe_Store
{
    public class Good
    {
        public int Id { get; set; }

        public string Name { get; set; } = String.Empty;

        public string Description { get; set; } = String.Empty;

        public string Product { get; set; } = String.Empty;

        public string SellerName { get; set; } = String.Empty;

        public Launchers Launcher { get; set; } = Launchers.None;

        public uint Price { get; set; } = uint.MaxValue;

        public bool IsSold { get; set; } = false;

        public static explicit operator GoodPublicDto(Good obj)
        {
            return new GoodPublicDto() { Id = obj.Id, Name = obj.Name, Description = obj.Description, SellerName = obj.SellerName, Launcher= obj.Launcher, Price = obj.Price };
        }
    }
}
