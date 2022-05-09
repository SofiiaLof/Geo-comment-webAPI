using GeoComment.Controllers;
using GeoComment.Data;
using GeoComment.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace GeoComment.Services
{
    public class GeoCommentHandler
    {

        private readonly GeoCommentDbContext _ctx;
        private readonly UserManager<User> _userManager;

        public GeoCommentHandler(GeoCommentDbContext ctx, UserManager<User> userManager)
        {
            _ctx = ctx;
            _userManager = userManager;
        }

      

        public async Task<Comment> PostComment(string name,string message, int longitude, int latitude)
        {

            var user = await _userManager.Users.Where(u=>u.First_name == name).FirstOrDefaultAsync();
            
            var comment = new Comment
            {
                Message = message,
                User = user,
                maxLat = latitude,
                maxLon = longitude,

            };

           _ctx.Comments.Add(comment);
           await _ctx.SaveChangesAsync();

            return comment;
        }

        public async Task<Comment> GetComment(int id)
        {
            var comment = await _ctx.Comments.Where(c => c.Id == id).Include(u=>u.User).FirstOrDefaultAsync();
            return comment;
        }

        public async Task<List<Comment>> GetCommentsInRange(int minLon, int maxLon, int minLat, int maxLat)
        {
            var commentsInRange = await _ctx.Comments
                .Where(c => c.minLon <= minLon && c.maxLon<=maxLon && c.minLat<= minLat && c.maxLat<= maxLat)
                .Include(u => u.User).ToListAsync();

            return commentsInRange;
        }

        public async Task<Comment> PostCommentV_2(string title, string message, int longitude, int latitude, User user)
        {

            var comment = new Comment
            {
                Message = message,
                Title = title,
                User = user,
                maxLon = longitude,
                maxLat = latitude
            };

            _ctx.Comments.Add(comment);
            await _ctx.SaveChangesAsync();

            return comment;
        }

        public async Task<List<Comment>> GetCommentsFoUser(string username)
        {
            var user = await _userManager.Users.Where(u=>u.UserName == username).Include(c=>c.Comments).FirstOrDefaultAsync();
           

            if (user != null)
            {
                var comments = user.Comments.ToList();
                return comments;
            }

            return null;
        }
    }
}
