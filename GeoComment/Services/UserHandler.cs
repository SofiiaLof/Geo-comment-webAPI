using GeoComment.Data;
using GeoComment.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GeoComment.Services
{
    public class UserHandler
    {

        private readonly GeoCommentDbContext _ctx;
        private readonly JwtPrinter _jwtPrinter;
        private readonly UserManager<User> _userManager;

        public UserHandler(GeoCommentDbContext ctx, UserManager<User> userManager, JwtPrinter jwtPrinter)
        {
            _ctx = ctx;
            _userManager = userManager;
            _jwtPrinter = jwtPrinter;
        }


        public async Task<User> RegisterUser(string username, string password)
        {

            var user = await _userManager.CreateAsync(new User()
            {
                UserName = username,
                First_name = username
            }, password);

            if (user.Succeeded)
            {
                await _ctx.SaveChangesAsync();
            }
           

            var registeredUser = await _userManager.Users.Where(u => u.UserName == username).FirstOrDefaultAsync();

            return registeredUser;
        }

        public async Task<bool> PasswordValidation(string password)
        {
            var passwordValidator = new PasswordValidator<User>();
            var isValid = await passwordValidator.ValidateAsync(_userManager, null, password);

            if (!isValid.Succeeded)
            {
                return false;
            }

            return true;
        }


        public async Task<string> LoginAttempt(string username, string password)
        {
            var user = await _userManager.FindByNameAsync(username);

            if (user == null)
            {
                return null;
            }

            var login = await _userManager.CheckPasswordAsync(user, password);

            if (!login)
            {
                return null ;
            }

            var token = _jwtPrinter.Print(user);

            return token;
        }
    }
}
