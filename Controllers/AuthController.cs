using Microsoft.AspNetCore.Mvc;
using PFA_WebAPI.DTO;
using PFA_WebAPI.Models;
using System.Security.Claims;
using PFA_WebAPI.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace PFA_WebAPI.Controllers
{
    [Route("api/")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserRepository _UserRepository;
        private readonly IPasswordHasher _passwordHasher;
        public AuthController(IUserRepository userRepository, IPasswordHasher passwordHasher)
        {
            _UserRepository = userRepository;
            _passwordHasher = passwordHasher;
        }

        [HttpPost("register")]
        public IActionResult Register(UserDTO request)
        {
            _passwordHasher.CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);
            var user = new User
            {
                Email = request.Email,
                Name = request.Name,
                Role = request.Role,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt
            };
            try
            {
                //Saving the new User to the Database
                bool t= _UserRepository.RegisterUser(user);
                return Ok("User Added");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("login")]
        public IActionResult Login(UserLoginDTO request)
        {
            try
            {
                var user = new User();
                user = _UserRepository.Login(request);
                string token = _passwordHasher.CreateToken(user);
                return Ok(token);
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("user/{id}"),Authorize]
        public IActionResult GetUserDetails(int id)
        {
            id = GetMe();
            var user = _UserRepository.GetUser(id);
            return Ok(user);
        }

        [HttpPut("user/edit"),Authorize]
        public IActionResult EditCurrentUser(UserEditDTO userUpdateDTO)
        {
            try
            {
                // Retrieve the user from the repository
                var user = _UserRepository.GetUser(GetMe());

                // Update the user's properties with the new values
                user.Name = userUpdateDTO.Name;
                user.Email = userUpdateDTO.Email;
                _passwordHasher.CreatePasswordHash(userUpdateDTO.Password, out byte[] passwordHash, out byte[] passwordSalt);
                user.PasswordHash = passwordHash;
                user.PasswordSalt = passwordSalt;

                // Save the changes to the database
                _UserRepository.UpdateUser(user);

                return Ok(user);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("user/edit/{id}"), Authorize(Roles = "Admin")]
        public IActionResult EditUser(int id, string password, User user)
        {
            try
            {
                // Retrieve the user from the repository
                var updatedUser = _UserRepository.GetUser(id);

                // Update the user's properties with the new values
                updatedUser.Id = id;
                updatedUser.Name = user.Name;
                updatedUser.Email = user.Email;
                _passwordHasher.CreatePasswordHash(password, out byte[] passwordHash, out byte[] passwordSalt);
                updatedUser.PasswordHash = passwordHash;
                updatedUser.PasswordSalt = passwordSalt;

                // Save the changes to the database
                _UserRepository.UpdateUser(user);

                return Ok(user);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("user/{id}"), Authorize(Roles = "Admin")]
        public IActionResult Delete(int id)
        {
            try
            {
                _UserRepository.DeleteUser(id);
                return Ok(id);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet, Authorize]
        public int GetMe()
        {
            var id = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.Parse(id);
        }
    }
}
