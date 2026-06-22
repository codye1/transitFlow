using Microsoft.EntityFrameworkCore;
using transitFlow.api.Models;

namespace transitFlow.api.Data
{
    public class TransitFlowDbContext : DbContext
    {
        public TransitFlowDbContext(DbContextOptions<TransitFlowDbContext> options) : base(options)
        {
        }
    
        public DbSet<User> Users { get; set; }
        public DbSet<Stop> Stops { get; set; }
        public DbSet<Vehicle> Vehicles { get; set; }    
        public DbSet<Route> Routes { get; set; }    
        public DbSet<RouteStop> RouteStops { get; set; }

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

            modelBuilder.Entity<Route>()
                .HasOne(r => r.Creator)
                .WithMany(u => u.CreatedRoutes)
                .HasForeignKey(r => r.CreatedById)
                .OnDelete(DeleteBehavior.Restrict); 

            modelBuilder.Entity<Stop>()
                .HasOne(s => s.Creator)
                .WithMany(u => u.CreatedStops)
                .HasForeignKey(s => s.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Route>()
                .HasIndex(r => r.Number)
                .IsUnique();

        }
    }
}
