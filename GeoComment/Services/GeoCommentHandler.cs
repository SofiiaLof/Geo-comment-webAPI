﻿using GeoComment.Data;
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
                minLat = 0,
                minLon = 0,
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
    }
}
