using GeoComment.Data;
using GeoComment.Models;

namespace GeoComment.Services
{
    public class DatabaseHandler
    {
        private readonly GeoCommentDbContext _ctx;

        public DatabaseHandler(GeoCommentDbContext ctx)
        {
            _ctx = ctx;
        }

        public async Task<bool> ResetDatabase()
        {  
            await _ctx.Database.EnsureDeletedAsync();


           bool databaseCreated = await _ctx.Database.EnsureCreatedAsync();

           if (databaseCreated)
           {
              await Seed();
                return true;
             
           }

           return false;
        }
        public async Task Seed()
        {
            var users = new List<User>
            {
                new(){First_name = "Ada"},
                new(){First_name = "Bill"}
            };

            await _ctx.AddRangeAsync(users);
            await _ctx.SaveChangesAsync();
        }
    }
}
