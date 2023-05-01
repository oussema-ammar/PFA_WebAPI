using PFA_WebAPI.DTO;
using PFA_WebAPI.Models;
using System.Security.Cryptography;

namespace PFA_WebAPI.Interfaces
{
    public interface IUserRepository
    {
        public bool RegisterUser(User user);
        public User Login(UserLoginDTO user);
        public User GetUser(int id);
        public void UpdateUser(User user);
        public void DeleteUser(int id);
        public bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt);
    }
}
