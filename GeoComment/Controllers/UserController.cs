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
       
        private readonly UserHandler _userHandler;
        public UserController(UserHandler userHandler)
        {
          
            _userHandler = userHandler;
        }


        [ApiVersion("0.2")]
        [HttpPost]
        [Route("register")]
        
        public async Task<ActionResult<UserOutput>> Register(UserInput input) {
            var isValid = await _userHandler.PasswordValidation(input.Password);


            if (!isValid)
            {
                return BadRequest();
            }

            var user = await _userHandler.RegisterUser(input.Username, input.Password);

            
           
            if (user == null)
            {
                return BadRequest();
            }

          
            return Created("", new UserOutput
            {
                Id= user.Id,
                Username =  user.UserName
            });
        }

        [ApiVersion("0.2")]
        [HttpPost]
        [Route("login")]
     
        public async Task<ActionResult<string>> Login(UserInput input)
        {


            var loginAttempt = await _userHandler.LoginAttempt(input.Username, input.Password);

            if (loginAttempt == null)
            {
                return BadRequest();
            }


            return Ok(new TokenOutput
            {
                Token = loginAttempt,
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
