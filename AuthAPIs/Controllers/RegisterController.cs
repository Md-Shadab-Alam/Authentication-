using AuthAPIs.Context;
using AuthAPIs.Helpers;
using AuthAPIs.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;

namespace AuthAPIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegisterController : ControllerBase
    {
        private readonly AppDbContext _appDbContext;
        public RegisterController(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        [HttpPost("authenticate")]
        public async Task<IActionResult> Authenticate([FromBody]User user)
        {
            if (user == null)
                return BadRequest();

           // user.Password = PasswordHasher.VerifyPassword(user.Password);
            var users = await _appDbContext.Users
                .FirstOrDefaultAsync(x=>x.Username== user.Username);
            
            if(users == null)
                return NotFound( new {Message = "User not found!"});

            if(!PasswordHasher.VerifyPassword(user.Password,users.Password))
            {
                return BadRequest(new {Message = "Password is incorrect"});
            }

            user.Token = CreateJwt(user);

            return Ok(new
            {
                Token = user.Token,
                Message = "Login Success!",
                users
            });;
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser([FromBody]User user)
        {
            if (user == null)
            {
                return BadRequest();
            }
            //check username is unique or not

            if(await CheckUserNameExistAsync(user.Username))
            {
                return BadRequest(new {Message = "Username already exist"});
            }

            //check Email is unique or not

            if (await CheckEmailExistAsync(user.Email))
            {
                return BadRequest(new { Message = "Email already exist" });
            }


            //Check password Strength

            var pass = CheckPasswordStrength(user.Password);
            if(!string.IsNullOrEmpty(pass))
            {
                return BadRequest(new { Message = pass });
            }

            //apply PasswordHasher
            user.Password = PasswordHasher.HashPassword(user.Password);
            user.Role = "User";
            user.Token = "";
            await _appDbContext.Users.AddAsync(user);
            await _appDbContext.SaveChangesAsync();
            return Ok(new
            {
                Message = user.Username + "   Registered", user
            }
            ) ;
        }

        private async Task<bool> CheckUserNameExistAsync(string userName)
        {
            return await _appDbContext.Users.AnyAsync(x=>x.Username == userName);
        }

        private async Task<bool> CheckEmailExistAsync(string email)
        {
            return await _appDbContext.Users.AnyAsync(x=>x.Email == email);
        }

        private string CheckPasswordStrength(string password)
        {
            StringBuilder sb = new StringBuilder();
            if(password.Length <8)
                 sb.Append("Minimum password length should be 8" + Environment.NewLine);
            
            if (!(Regex.IsMatch(password, "[a-z]") 
                && Regex.IsMatch(password, "[A-Z]") 
                && Regex.IsMatch(password, "[0-9]")))
                sb.Append("Password should be Alphanumeric" + Environment.NewLine);
            
            if(!Regex.IsMatch(password,"[<,>,@,$,&,*,#,!]"))
                sb.Append("Password should contain Special Char");

            return sb.ToString();
        }

        private string CreateJwt(User user)
        {
            var jwtTokenHandeler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("Veryverysecret.....");
            var identity = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Role, user.Role),
                new Claim(ClaimTypes.Name,$"{user.Firstname}{user.Lastname}")
            });
            var credentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = identity,
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = credentials
            };
            var token = jwtTokenHandeler.CreateToken(tokenDescriptor);
            return jwtTokenHandeler.WriteToken(token);
        }
    }
}
