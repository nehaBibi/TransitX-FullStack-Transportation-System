using Microsoft.EntityFrameworkCore;
using TransitX.Models;
using Route = TransitX.Models.Route;

namespace TransitX.Data
{
    public class TransitXDbContext : DbContext
    {
        public TransitXDbContext(DbContextOptions<TransitXDbContext> options) : base(options) { }

        public DbSet<Route> Routes { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<ContactMessage> ContactMessages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Route>().HasData(
                new Route
                {
                    Id = 1,
                    Origin = "Karachi", Destination = "Hyderabad",
                    VehicleType = "Luxury Bus", DepartureTime = "10:00 AM",
                    Status = "On Time", DelayInfo = "", Price = 2500, AvailableSeats = 32,
                    IsActive = true, CreatedAt = new DateTime(2024, 1, 1),
                    ImageUrl = "https://images.unsplash.com/photo-1519003722824-194d4455a60c?q=80&w=600&auto=format&fit=crop"
                },
                new Route
                {
                    Id = 2,
                    Origin = "Karachi", Destination = "Lahore",
                    VehicleType = "Express Train", DepartureTime = "1:30 PM",
                    Status = "Delayed", DelayInfo = "15min", Price = 3200, AvailableSeats = 50,
                    IsActive = true, CreatedAt = new DateTime(2024, 1, 1),
                    ImageUrl = "https://images.unsplash.com/photo-1474487548417-781cb71495f3?q=80&w=600&auto=format&fit=crop"
                },
                new Route
                {
                    Id = 3,
                    Origin = "Islamabad", Destination = "Murree",
                    VehicleType = "Tour Bus", DepartureTime = "6:00 PM",
                    Status = "On Time", DelayInfo = "", Price = 1800, AvailableSeats = 28,
                    IsActive = true, CreatedAt = new DateTime(2024, 1, 1),
                    ImageUrl = "https://images.unsplash.com/photo-1500530855697-b586d89ba3ee?q=80&w=600&auto=format&fit=crop"
                },
                new Route
                {
                    Id = 4,
                    Origin = "Lahore", Destination = "Multan",
                    VehicleType = "Intercity Bus", DepartureTime = "9:00 AM",
                    Status = "On Time", DelayInfo = "", Price = 2100, AvailableSeats = 40,
                    IsActive = true, CreatedAt = new DateTime(2024, 1, 1),
                    ImageUrl = "https://images.unsplash.com/photo-1527786356703-4b100091cd2c?q=80&w=600&auto=format&fit=crop"
                },
                new Route
                {
                    Id = 5,
                    Origin = "Peshawar", Destination = "Islamabad",
                    VehicleType = "Luxury Bus", DepartureTime = "7:00 AM",
                    Status = "On Time", DelayInfo = "", Price = 1500, AvailableSeats = 36,
                    IsActive = true, CreatedAt = new DateTime(2024, 1, 1),
                    ImageUrl = "https://images.unsplash.com/photo-1544620347-c4fd4a3d5957?q=80&w=600&auto=format&fit=crop"
                },
                new Route
                {
                    Id = 6,
                    Origin = "Quetta", Destination = "Karachi",
                    VehicleType = "Sleeper Bus", DepartureTime = "8:00 PM",
                    Status = "On Time", DelayInfo = "", Price = 4500, AvailableSeats = 20,
                    IsActive = true, CreatedAt = new DateTime(2024, 1, 1),
                    ImageUrl = "https://images.unsplash.com/photo-1558618666-fcd25c85cd64?q=80&w=600&auto=format&fit=crop"
                }
            );
        }
    }
}
