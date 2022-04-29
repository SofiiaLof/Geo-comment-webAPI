using GeoComment.Data;
using GeoComment.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace GeoComment.Services
{
    public class GeoCommentHandler
    {

        private readonly GeoCommentDbContext _ctx;

        public GeoCommentHandler(GeoCommentDbContext ctx)
        {
            _ctx = ctx;
        }

      

        public async Task<Comment> PostComment(string name,string message, int longitude, int latitude)
        {
            var user = await _ctx.Users.Where(u => u.First_name == name).FirstOrDefaultAsync();

            var comment = new Comment
            {
                Message = message,
                User = user,
                maxLat = latitude,
                maxLon = longitude,

            };

            await _ctx.Comments.AddAsync(comment);
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
    }
}
