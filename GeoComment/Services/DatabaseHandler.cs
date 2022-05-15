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

               return true;
             
           }

           return false;
        }
       
    }
}
