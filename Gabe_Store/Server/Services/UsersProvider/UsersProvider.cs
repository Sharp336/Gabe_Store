using Gabe_Store.Shared;
using System.Security.Cryptography;

namespace Gabe_Store.Services.UserProvider
{
    public class UsersProvider : IUsersProvider
    {
        private List<User> _usersStorage = new();

        public void CreateNewUser(UserLoginDto user)
        {
            CreatePasswordHash(user.Password, out byte[] passwordHash, out byte[] passwordSalt);

            _usersStorage.Add(new User()
            {
                Username = user.Username,
                Balance = 0,
                Roles = new() { "Buyer" },
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt
            });

        }

        public bool TryAuthUser(UserLoginDto user)
        {
            var storedUser = _usersStorage.SingleOrDefault(u => u.Username == user.Username);

            return VerifyPasswordHash(user.Password, storedUser.PasswordHash, storedUser.PasswordSalt);
        }

        public User? GetUserByName(string username) => _usersStorage.SingleOrDefault(u => u.Username == username);

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(passwordHash);
            }
        }


        public int GetUsersCount() => _usersStorage.Count();
    }
}
