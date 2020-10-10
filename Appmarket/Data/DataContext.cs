using Appmarket.Data.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Appmarket.Data
{
    public class DataContext :  IdentityDbContext<User>
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        public DbSet<Order> Orders { get; set; }

        public DbSet<OrderDetail> OrderDetails { get; set; }

        public DbSet<OrderDetailTemp> OrderDetailTemps { get; set; }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Country>(cou =>
            {
                cou.HasIndex("Name").IsUnique();
                cou.HasMany(c => c.Cities).WithOne(d => d.Country).OnDelete(DeleteBehavior.Cascade);
            });


            modelBuilder.Entity<City>(cit =>
            {
                cit.HasIndex("Name", "CountryId").IsUnique();
                cit.HasOne(c => c.Country).WithMany(d => d.Cities).OnDelete(DeleteBehavior.Cascade);
            });


            modelBuilder.Entity<Category>()
            .HasIndex(t => t.Name)
            .IsUnique();

            modelBuilder.Entity<Product>()
            .HasIndex(t => t.Name)
            .IsUnique();

        }
    }
}
