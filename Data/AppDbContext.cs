using Microsoft.EntityFrameworkCore;
using EstoqueApi.Models;

namespace EstoqueApi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext() { }
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Product> Products => Set<Product>();

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Default SQLite DB file in project folder
            if (!optionsBuilder.IsConfigured)
                optionsBuilder.UseSqlite("Data Source=estoque.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>(e =>
            {
                e.HasKey(p => p.Id);
                e.Property(p => p.Name).IsRequired().HasMaxLength(100);
                e.HasIndex(p => p.Name).IsUnique();
                e.Property(p => p.Category).HasMaxLength(50);
                e.Property(p => p.SKU).HasMaxLength(50);
                e.HasIndex(p => p.SKU).IsUnique();
                e.Property(p => p.Price).IsRequired();
            });
        }
    }
}
