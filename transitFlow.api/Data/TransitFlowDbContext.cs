using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using transitFlow.api.Models;
using RouteEntity = transitFlow.api.Models.Route;

namespace transitFlow.api.Data
{
    public class TransitFlowDbContext : IdentityDbContext<AppUser, IdentityRole<int>, int>
    {
        public TransitFlowDbContext(DbContextOptions<TransitFlowDbContext> options) : base(options)
        {
        }
    
        public DbSet<AppUser> Users { get; set; }
        public DbSet<Stop> Stops { get; set; }
        public DbSet<Vehicle> Vehicles { get; set; }    
        public DbSet<RouteEntity> Routes { get; set; }    
        public DbSet<RouteStop> RouteStops { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<RouteStop>()
                .HasOne(rs => rs.Route)
                .WithMany(r => r.RouteStops)
                .HasForeignKey(rs => rs.RouteId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<RouteStop>()
                .HasOne(rs => rs.Stop)
                .WithMany(s => s.RouteStops)
                .HasForeignKey(rs => rs.StopId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<RouteEntity>()
                .HasOne(r => r.Creator)
                .WithMany(u => u.CreatedRoutes)
                .HasForeignKey(r => r.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Stop>()
                .HasOne(s => s.Creator)
                .WithMany(u => u.CreatedStops)
                .HasForeignKey(s => s.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<RouteEntity>()
                .HasIndex(r => r.Number)
                .IsUnique();

            modelBuilder.Entity<IdentityRole<int>>().HasData(
                new IdentityRole<int>
                {
                    Id = 1,
                    Name = "user",
                    NormalizedName = "USER",
                    ConcurrencyStamp = "a6f8b9d2-3c4e-4f5a-8b1c-2d3e4f5a6b7c"
                },
                new IdentityRole<int>
                {
                    Id = 2,
                    Name = "admin",
                    NormalizedName = "ADMIN",
                    ConcurrencyStamp = "b7e9c0a1-4d5e-5f6a-9b2c-3d4e5f6a7b8c"
                },
                new IdentityRole<int>
                {
                    Id = 3,
                    Name = "moderator",
                    NormalizedName = "MODERATOR",
                    ConcurrencyStamp = "c8d0e1b2-5e6f-6a7b-0c3d-4e5f6a7b8c9d"
                }
            );
        }


    }
}
