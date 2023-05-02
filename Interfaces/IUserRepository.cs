using PFA_WebAPI.DTO;
using PFA_WebAPI.Models;

namespace PFA_WebAPI.Interfaces
{
    public interface IUserRepository
    {
        public bool RegisterUser(User user);
        public User Login(UserLoginDTO user);
        public User GetUser(int id);
        public void UpdateUser(User user);
        public void DeleteUser(int id);
    }
}
