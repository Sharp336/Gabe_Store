namespace Gabe_Store
{
    public class Good
    {
        public int Id { get; set; }

        public string Name = String.Empty;

        public string Description = String.Empty;
        
        public string SellerName = String.Empty;

        public Launchers? Launcher;

        public uint price;

    }
}
