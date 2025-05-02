using Microsoft.EntityFrameworkCore;
using RevApi.Models;

namespace RevApi.DbsContext
{
    public class RevDbContext : DbContext
    {
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Response> Responses { get; set; }

        public RevDbContext(DbContextOptions<RevDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Review>()
               .HasOne(r => r.Response)
               .WithOne(res => res.Review)
               .HasForeignKey<Response>(res => res.ReviewId)
               .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
