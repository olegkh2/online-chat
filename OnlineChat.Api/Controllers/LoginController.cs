using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using OnlineChat.Api.Context;
using OnlineChat.Api.Models;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace OnlineChat.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private IConfiguration _config;
        private ChatDbContext _context;

        public LoginController(IConfiguration config, ChatDbContext context)
        {
            _config = config;
            _context = context;
        }

        /// <summary>
        /// Gets current user's id
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet("getUserId")]
        public ActionResult<int> GetUserId()
        {
            int id = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
            return id;
        }

        /// <summary>
        /// Authenticates user and generates JWT
        /// </summary>
        /// <param name="userLogin"></param>
        /// <returns>Json web token</returns>
        [AllowAnonymous]
        [HttpPost]
        public IActionResult Login([FromBody] UserLogin userLogin)
        {
            var user = Authenticate(userLogin);

            if (user != null)
            {
                var token = Generate(user);
                return Ok(token);
            }

            return NotFound("User not found");
        }

        /// <summary>
        /// Generate token 
        /// </summary>
        private string Generate(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Name),
            };

            var token = new JwtSecurityToken(_config["Jwt:Issuer"],
              _config["Jwt:Audience"],
              claims,
              expires: DateTime.Now.AddMinutes(15),
              signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        /// <summary>
        /// Authenticate user
        /// </summary>
        private User Authenticate(UserLogin userLogin)
        {
            var currentUser = _context.Users
                .FirstOrDefault(o => o.Login.ToLower() == userLogin.Username.ToLower() 
                && o.Password == userLogin.Password);

            if (currentUser != null)
            {
                return currentUser;
            }

            return null;
        }
    }
}
