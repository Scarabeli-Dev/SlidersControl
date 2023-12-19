using SlidersControl.Entities;
using Microsoft.EntityFrameworkCore;

namespace SlidersControl.Data
{
    public class SlidersControlDbContext : DbContext
    {
        public SlidersControlDbContext(DbContextOptions<SlidersControlDbContext> options) : base(options) { }

        public DbSet<Slider> sliders { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
