using API.Data;
using System.Threading.Tasks;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;
using API.DTOs;
using Microsoft.EntityFrameworkCore;
using API.Interfaces;

namespace API.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly DataContext context;
        private readonly ITokenService tokenService;

        public AccountController(DataContext context, ITokenService tokenService)
        {
            this.tokenService = tokenService;
            this.context = context;
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerdto)
        {
            if (await UserExists(registerdto.Username)) return BadRequest("Usernama is already taken");
            using var hmac = new HMACSHA512();
            var user = new AppUser
            {
                Username = registerdto.Username.ToLower(),
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerdto.Password)),
                PasswordSalt = hmac.Key
            };
            context.Users.Add(user);
            await context.SaveChangesAsync();
            return new UserDto{
                Username=user.Username,
                Token=tokenService.CreateToken(user)
            };
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto _LoginDto)
        {
            var user = await context.Users.SingleOrDefaultAsync(x => x.Username == _LoginDto.Username);
            if (user == null)
                return Unauthorized("Invalid username");
            using var hmac = new HMACSHA512(user.PasswordSalt);
            var ComputedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(_LoginDto.Password));
            for (int i = 0; i < ComputedHash.Length; i++)
            {
                if (ComputedHash[i] != user.PasswordHash[i]) return Unauthorized("Invalid Password");
            }
            return new UserDto{
                Username=user.Username,
                Token=tokenService.CreateToken(user)
            };
        }
        private async Task<bool> UserExists(string Username)
        {
            return await context.Users.AnyAsync(x => x.Username == Username.ToLower());
        }
    }
}