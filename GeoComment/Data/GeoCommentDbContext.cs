using System.Security.Cryptography.X509Certificates;
using GeoComment.Models;
using Microsoft.EntityFrameworkCore;

namespace GeoComment.Data
{
    public class GeoCommentDbContext : DbContext
    {
        public GeoCommentDbContext(DbContextOptions options) : base(options)
        {
            
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Comment> Comments { get; set; }
    }
}
