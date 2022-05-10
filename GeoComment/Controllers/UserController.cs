using System.Runtime.CompilerServices;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using GeoComment.Data;
using GeoComment.Models;
using GeoComment.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GeoComment.Controllers
{
    [Route("api/user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly GeoCommentDbContext _ctx;
        private readonly JwtPrinter _jwtPrinter;
        public UserController(UserManager<User> userManager, GeoCommentDbContext ctx, JwtPrinter jwtPrinter)
        {
            _userManager = userManager;
            _ctx = ctx;
            _jwtPrinter = jwtPrinter;
        }


        [ApiVersion("0.2")]
        [HttpPost]
        [Route("register")]
        
        public async Task<ActionResult<UserOutput>> Register(UserInput input)
        {
            var passwordValidator = new PasswordValidator<User>();
            var isValid = await passwordValidator.ValidateAsync(_userManager,null,input.Password);
            if (!isValid.Succeeded)
            {
                return BadRequest();
            }

            var user = await _userManager.CreateAsync(new User()
            {
                UserName = input.Username,
                First_name = input.Username
            }, input.Password);

           
            if (!user.Succeeded)
            {
                return BadRequest();
            }

          
            await _ctx.SaveChangesAsync();

            var registeredUser = await _userManager.Users.Where(u => u.UserName == input.Username).FirstOrDefaultAsync();
          
            return Created("", new UserOutput
            {
                Id= registeredUser.Id,
                Username = registeredUser.UserName
            });
        }

        [ApiVersion("0.2")]
        [HttpPost]
        [Route("login")]
     
        public async Task<ActionResult<string>> Login(UserInput input)
        {


            var user = await _userManager.FindByNameAsync(input.Username);

            if (user == null)
            {
                return BadRequest();
            }

            var login = await _userManager.CheckPasswordAsync(user, input.Password);

            if (!login)
            {
                return BadRequest();
            }

            var token = _jwtPrinter.Print(user);


            return Ok(new TokenOutput
            {
                Token = token,
            });
        }
    }

    public class UserInput
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class UserOutput
    {
        public string Id { get; set; }
        public string Username { get; set; }
       
    }

    public class TokenOutput
    {
        public string Token { get; set; }
    }
}
