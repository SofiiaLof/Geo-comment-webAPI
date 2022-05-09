using GeoComment.Data;
using GeoComment.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace GeoComment.Services
{
    public class DatabaseHandler
    {
        private readonly GeoCommentDbContext _ctx;
        private readonly UserManager<User> _userManager;

        public DatabaseHandler(GeoCommentDbContext ctx, UserManager<User> userManager)
        {
            _ctx = ctx;
            _userManager = userManager;
        }

        public async Task<bool> ResetDatabase()
        {  
            await _ctx.Database.EnsureDeletedAsync();


           bool databaseCreated = await _ctx.Database.EnsureCreatedAsync();

           if (databaseCreated)
           {
              // await Seed();
               
                return true;
             
           }

           return false;
        }
        public async Task Seed()
        {
            var user1 = new User
            {
                First_name = "Ada",
                UserName = "Ada",
                
            };

            
             await _userManager.CreateAsync(user1, "Passw0rd1@");
         
            
            await _ctx.SaveChangesAsync();
        }
    }
}
