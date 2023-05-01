using Microsoft.EntityFrameworkCore;
using PFA_WebAPI.Data;
using PFA_WebAPI.DTO;
using PFA_WebAPI.Interfaces;
using PFA_WebAPI.Models;
using System.Security.Cryptography;

namespace PFA_WebAPI.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext _context;
        public UserRepository(DataContext context)
        {
            _context = context;
        }

        public bool RegisterUser(User user)
        {
            //Checking if another User with the same Email already exists
            if (_context.Users.Any(u => u.Email == user.Email))
            {
                throw new Exception("A user with the same email already exists.");
            }
            _context.Users.Add(user);
            _context.SaveChanges();
            return true;
        }

        public User Login(UserLoginDTO user)
        {
            //Checking the Existence of the Email
            var FoundUser = _context.Users.Where(b => b.Email == user.Email).FirstOrDefault() 
            ?? throw new Exception("No user with this email exists.");
            if (!VerifyPasswordHash(user.Password,FoundUser.PasswordHash,FoundUser.PasswordSalt))
            {
                throw new Exception("Password is wrong.");
            }
            return FoundUser;
        }

        public bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            var hmac = new HMACSHA512(passwordSalt);
            var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            return computedHash.SequenceEqual(passwordHash);
        }

        public User GetUser(int id)
        {
            var user = _context.Users.Where(b => b.Id == id).FirstOrDefault()
            ?? throw new Exception("User doesn't exist.");
            return user;
        }

        public void UpdateUser(User user)
        {
            // Retrieve the existing user from the database
            var existingUser = _context.Users.FirstOrDefault(u => u.Id == user.Id);

            // If the user exists, update its properties with the new values
            if (existingUser != null)
            {
                existingUser.Id = user.Id;
                existingUser.Name = user.Name;
                existingUser.Email = user.Email;
                existingUser.PasswordHash = user.PasswordHash;
                existingUser.PasswordSalt = user.PasswordSalt;
                existingUser.Role = user.Role;

                // Call the SaveChanges method to persist the changes to the database
                _context.SaveChanges();
            }
        }

        public void DeleteUser(int id)
        {
            var user = _context.Users.FirstOrDefault(u => u.Id == id)
            ?? throw new Exception("User doesn't exist.");
            _context.Users.Remove(user);
            _context.SaveChanges();
        }
    }
}
