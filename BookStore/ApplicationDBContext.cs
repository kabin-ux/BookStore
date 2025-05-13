using BookStore.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace BookStore
{
    public class ApplicationDBContext : IdentityDbContext<Users, Roles, long>
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : base(options)
        {
        }

        public DbSet<Books> Books { get; set; }
        public DbSet<Discounts> Discounts { get; set; }
        public DbSet<Orders> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Announcements> Announcements { get; set; }
        public DbSet<Reviews> Reviews { get; set; }
        public DbSet<Carts> Carts { get; set; }
        public DbSet<Whitelists> Whitelists { get; set; }
        public DbSet<Banners> Banners { get; set; }



        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Seeeding all roles
            builder.Entity<Roles>().HasData(
                new Roles { Id = 1, Name = "Admin", NormalizedName = "ADMIN" },
                new Roles { Id = 2, Name = "Member", NormalizedName = "MEMBER" },
                new Roles { Id = 3, Name = "Staff", NormalizedName = "STAFF" }
            );
        }
    }
}