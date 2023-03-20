using Gabe_Store.Shared;

namespace Gabe_Store.Services.UserProvider
{
    public interface IUsersProvider
    {
        public int GetUsersCount();
        public void CreateNewUser(UserLoginDto user);

        public bool TryAuthUser(UserLoginDto user);

        public User? GetUserByName(string username);
    }
}
