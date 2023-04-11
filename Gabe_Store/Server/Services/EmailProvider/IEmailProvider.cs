namespace Gabe_Store.Services.EmailProvider
{
    public interface IEmailProvider
    {
        public void SendGoodEmail(Good g, string email);
    }
}
