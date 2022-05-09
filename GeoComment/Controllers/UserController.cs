using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using GeoComment.Data;
using GeoComment.Models;
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
        public UserController(UserManager<User> userManager, GeoCommentDbContext ctx)
        {
            _userManager = userManager;
            _ctx = ctx;
        }


        [ApiVersion("0.2")]
        [HttpPost]
        [Route("register")]
        
        public async Task<ActionResult<RegistrationOutput>> Register(RegistrationInput input)
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
            return Created("", new RegistrationOutput
            {
                Id= registeredUser.Id,
                Username = registeredUser.UserName
            });
        }
    }

    public class RegistrationInput
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class RegistrationOutput
    {
        public string Id { get; set; }
        public string Username { get; set; }
       
    }
}
