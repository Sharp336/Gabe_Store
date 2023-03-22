using Gabe_Store.Shared;

namespace Gabe_Store.Services.UserProvider
{
    public interface IUsersProvider
    {
        public void CreateNewUser(UserRegisterDto user);

        public bool TryAuthUser(UserLoginDto user);

        public User? TryGetUserByName(string username);

        public bool TryAdjustUserBalance(string username, int amount);
    }
}
