using Microsoft.EntityFrameworkCore;
using TransitX.Data;
using TransitX.Models;

namespace TransitX.Services
{
    public class TransitService : ITransitService
    {
        private readonly TransitXDbContext _db;
        private readonly ILogger<TransitService> _logger;

        public TransitService(TransitXDbContext db, ILogger<TransitService> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<List<RouteCard>> GetRoutesAsync(string? searchQuery = null)
        {
            var query = _db.Routes.Where(r => r.IsActive).AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                var q = searchQuery.Trim().ToLower();
                query = query.Where(r =>
                    r.Origin.ToLower().Contains(q) ||
                    r.Destination.ToLower().Contains(q) ||
                    r.VehicleType.ToLower().Contains(q));
            }

            return await query.Select(r => new RouteCard
            {
                Id = r.Id,
                Origin = r.Origin,
                Destination = r.Destination,
                VehicleType = r.VehicleType,
                DepartureTime = r.DepartureTime,
                Status = r.Status,
                DelayInfo = r.DelayInfo,
                Price = r.Price,
                ImageUrl = r.ImageUrl,
                AvailableSeats = r.AvailableSeats
            }).ToListAsync();
        }

        public async Task<RouteCard?> GetRouteByIdAsync(int id)
        {
            var r = await _db.Routes.FindAsync(id);
            if (r == null || !r.IsActive) return null;
            return new RouteCard
            {
                Id = r.Id, Origin = r.Origin, Destination = r.Destination,
                VehicleType = r.VehicleType, DepartureTime = r.DepartureTime,
                Status = r.Status, DelayInfo = r.DelayInfo, Price = r.Price,
                ImageUrl = r.ImageUrl, AvailableSeats = r.AvailableSeats
            };
        }

        public async Task<List<TrackingInfo>> GetTrackingDataAsync()
        {
            // Build tracking from active routes with some simulated live progress
            var routes = await _db.Routes.Where(r => r.IsActive).Take(4).ToListAsync();
            var rng = new Random();
            return routes.Select((r, i) => new TrackingInfo
            {
                Route = $"{r.Origin} → {r.Destination}",
                VehicleType = r.VehicleType,
                DepartureTime = r.DepartureTime,
                Status = r.Status,
                ProgressPercent = new[] { 76, 42, 63, 25 }[i % 4],
                Eta = new[] { "2h 35m", "4h 10m", "3h 10m", "5h 00m" }[i % 4]
            }).ToList();
        }

        public async Task<DashboardStats> GetStatsAsync()
        {
            var confirmedBookings = await _db.Bookings
                .Where(b => b.Status == "Confirmed")
                .Select(b => b.TotalPrice)
                .ToListAsync();
            
            return new DashboardStats
            {
                TotalRoutes = await _db.Routes.CountAsync(r => r.IsActive),
                TotalBookings = await _db.Bookings.CountAsync(b => b.Status == "Confirmed"),
                TotalMessages = await _db.ContactMessages.CountAsync(),
                TotalRevenue = confirmedBookings.Sum()
            };
        }

        public async Task<(bool Success, string BookingRef, string Message)> CreateBookingAsync(BookingModel model)
        {
            var route = await _db.Routes.FindAsync(model.RouteId);
            if (route == null || !route.IsActive)
                return (false, "", "Route not found.");

            if (route.AvailableSeats < model.Seats)
                return (false, "", $"Only {route.AvailableSeats} seat(s) available.");

            var bookingRef = Guid.NewGuid().ToString("N")[..8].ToUpper();

            var booking = new Booking
            {
                BookingRef = bookingRef,
                RouteId = model.RouteId,
                PassengerName = model.PassengerName,
                PassengerEmail = model.PassengerEmail,
                Seats = model.Seats,
                TotalPrice = model.TotalPrice,
                Status = "Confirmed",
                CreatedAt = DateTime.UtcNow
            };

            route.AvailableSeats -= model.Seats;

            _db.Bookings.Add(booking);
            await _db.SaveChangesAsync();

            _logger.LogInformation("Booking {Ref} created: {Route} x{Seats}", bookingRef, model.Route, model.Seats);
            return (true, bookingRef, $"Booking confirmed! {model.Seats} seat(s) on {model.Route}");
        }

        public async Task<Booking?> GetBookingByRefAsync(string bookingRef)
        {
            return await _db.Bookings
                .Include(b => b.Route)
                .FirstOrDefaultAsync(b => b.BookingRef == bookingRef.ToUpper());
        }

        public async Task<bool> CancelBookingAsync(string bookingRef)
        {
            var booking = await _db.Bookings
                .Include(b => b.Route)
                .FirstOrDefaultAsync(b => b.BookingRef == bookingRef.ToUpper() && b.Status == "Confirmed");

            if (booking == null) return false;

            booking.Status = "Cancelled";
            if (booking.Route != null)
                booking.Route.AvailableSeats += booking.Seats;

            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> SaveContactMessageAsync(ContactFormModel model)
        {
            _db.ContactMessages.Add(new ContactMessage
            {
                Name = model.Name,
                Email = model.Email,
                Message = model.Message,
                CreatedAt = DateTime.UtcNow
            });
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<int> ClearAllBookingsAsync()
        {
            // Restore available seats for all routes before clearing bookings
            var bookings = await _db.Bookings.Include(b => b.Route).ToListAsync();
            foreach (var booking in bookings)
            {
                if (booking.Route != null && booking.Status == "Confirmed")
                    booking.Route.AvailableSeats += booking.Seats;
            }

            // Remove all bookings
            _db.Bookings.RemoveRange(bookings);
            int count = await _db.SaveChangesAsync();

            _logger.LogInformation("Cleared {Count} booking(s)", count);
            return count;
        }
    }
}
