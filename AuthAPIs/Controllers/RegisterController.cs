using AuthAPIs.Context;
using AuthAPIs.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
            {
                return BadRequest();
            }
            var users = await _appDbContext.Users.FirstOrDefaultAsync(x=>x.Username== user.Username && x.Password == user.Password);
            if(users == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(users);
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser([FromBody]User user)
        {
            if (user == null)
            {
                return BadRequest();
            }
            await _appDbContext.Users.AddAsync(user);
            await _appDbContext.SaveChangesAsync();
            return Ok(new
            {
                Message = user.Username + "   Registered"
            });
        }
    }
}
