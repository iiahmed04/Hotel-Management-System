using HMS.Core.Entities.BookingEntities;
using HMS.Core.Entities.IdentityEntities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace HMS.Infrastructure.Data.DbContexts
{
    public class HotelDbContext : IdentityDbContext<HotelUser>
    {
        public HotelDbContext(DbContextOptions<HotelDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<HotelUser>().ToTable("Users");
            modelBuilder.Entity<IdentityRole>().ToTable("Roles");
            modelBuilder.Entity<StaffUser>().ToTable("StaffUsers");

            modelBuilder.Entity<Booking>().Property(x => x.TotalAmount).HasPrecision(8, 2);

            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }

    }
}
