using CmsSample.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace CmsSample.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        // Bu satırlar veritabanındaki tablolara karşılık gelir
        public DbSet<StaticPage> Pages { get; set; }

        public DbSet<BlogPost> BlogPosts { get; set; }
        public DbSet<SliderItem> SliderItems { get; set; }
        public DbSet<ContactMessage> ContactMessages { get; set; }
    }
}
