using Gabe_Store.Shared;
using Microsoft.AspNetCore.Mvc;

namespace Gabe_Store.Services.UserService
{
    public interface IDataStorage
    {
        public int GetUsersCount();
        public void CreateNewUser(UserLoginDto user);

        public bool TryAuthUser(UserLoginDto user);

        public User GetUserByName(string username);
    }
}
