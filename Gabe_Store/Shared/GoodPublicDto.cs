namespace Gabe_Store
{
    public class GoodPublicDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = String.Empty;

        public string Description { get; set; } = String.Empty;

        public string SellerName { get; set; } = String.Empty;

        public Launchers Launcher { get; set; } = Launchers.None;

        public uint Price { get; set; } = uint.MaxValue;

    }
}
