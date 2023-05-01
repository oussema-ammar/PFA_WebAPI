using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using PFA_WebAPI.DTO;
using PFA_WebAPI.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using PFA_WebAPI.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace PFA_WebAPI.Controllers
{
    [Route("api/")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserRepository _UserRepository;
        private readonly IConfiguration _configuration;
        public AuthController(IConfiguration configuration, IUserRepository userRepository)
        {
            _configuration = configuration;
            _UserRepository = userRepository;
        }

        [HttpPost("register")]
        public IActionResult Register(UserDTO request)
        {
            CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);
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
                string token = CreateToken(user);
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
                CreatePasswordHash(userUpdateDTO.Password, out byte[] passwordHash, out byte[] passwordSalt);
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
                CreatePasswordHash(password, out byte[] passwordHash, out byte[] passwordSalt);
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
        
        private string CreateToken(User user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(
                _configuration.GetSection("AppSettings:Token").Value));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: creds);

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }
    }
}
