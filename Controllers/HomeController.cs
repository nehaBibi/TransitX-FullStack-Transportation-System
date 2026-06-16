using Microsoft.AspNetCore.Mvc;
using TransitX.Models;
using TransitX.Services;

namespace TransitX.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ITransitService _transitService;

        public HomeController(ILogger<HomeController> logger, ITransitService transitService)
        {
            _logger = logger;
            _transitService = transitService;
        }

        // GET: /
        public async Task<IActionResult> Index()
        {
            var vm = new HomeViewModel
            {
                Routes = await _transitService.GetRoutesAsync(),
                TrackingList = await _transitService.GetTrackingDataAsync(),
                Stats = await _transitService.GetStatsAsync()
            };
            return View(vm);
        }

        // POST: /Home/Contact
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Contact(ContactFormModel model)
        {
            if (!ModelState.IsValid)
            {
                var vm = new HomeViewModel
                {
                    Routes = await _transitService.GetRoutesAsync(),
                    TrackingList = await _transitService.GetTrackingDataAsync(),
                    ContactForm = model,
                    Stats = await _transitService.GetStatsAsync()
                };
                return View("Index", vm);
            }

            await _transitService.SaveContactMessageAsync(model);
            _logger.LogInformation("Contact form submitted by {Name} ({Email})", model.Name, model.Email);

            TempData["ContactSuccess"] = "Your message has been sent successfully!";
            return RedirectToAction(nameof(Index), "Home", fragment: "contact");
        }

        // POST: /Home/Book  (AJAX)
        [HttpPost]
        public async Task<IActionResult> Book([FromBody] BookingModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Invalid booking data." });

            var (success, bookingRef, message) = await _transitService.CreateBookingAsync(model);

            if (!success)
                return BadRequest(new { success = false, message });

            return Json(new
            {
                success = true,
                message,
                bookingRef,
                totalPrice = model.TotalPrice
            });
        }

        // GET: /Home/Routes?q=karachi  (AJAX search)
        [HttpGet]
        public async Task<IActionResult> Routes(string? q)
        {
            var routes = await _transitService.GetRoutesAsync(q);
            return Json(routes);
        }

        // GET: /Home/TrackBooking?ref=ABC12345
        [HttpGet]
        public async Task<IActionResult> TrackBooking(string? @ref)
        {
            if (string.IsNullOrWhiteSpace(@ref))
                return Json(new { success = false, message = "Please enter a booking reference." });

            var booking = await _transitService.GetBookingByRefAsync(@ref);
            if (booking == null)
                return Json(new { success = false, message = "Booking not found. Please check your reference." });

            return Json(new
            {
                success = true,
                bookingRef = booking.BookingRef,
                route = booking.Route != null ? $"{booking.Route.Origin} → {booking.Route.Destination}" : "N/A",
                vehicleType = booking.Route?.VehicleType ?? "N/A",
                departureTime = booking.Route?.DepartureTime ?? "N/A",
                passengerName = booking.PassengerName,
                seats = booking.Seats,
                totalPrice = booking.TotalPrice,
                status = booking.Status,
                bookedAt = booking.CreatedAt.ToString("dd MMM yyyy, hh:mm tt")
            });
        }

        // POST: /Home/CancelBooking  (AJAX)
        [HttpPost]
        public async Task<IActionResult> CancelBooking([FromBody] CancelBookingRequest req)
        {
            if (string.IsNullOrWhiteSpace(req.BookingRef))
                return BadRequest(new { success = false, message = "Booking reference required." });

            var success = await _transitService.CancelBookingAsync(req.BookingRef);
            return Json(new
            {
                success,
                message = success ? "Booking cancelled successfully." : "Booking not found or already cancelled."
            });
        }

        // POST: /Home/ClearAllBookings
        [HttpPost]
        public async Task<IActionResult> ClearAllBookings()
        {
            var count = await _transitService.ClearAllBookingsAsync();
            _logger.LogInformation("All bookings cleared. Removed {Count} record(s)", count);
            return Json(new { success = true, message = $"Cleared {count} booking(s)." });
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() => View();
    }

    public class CancelBookingRequest
    {
        public string BookingRef { get; set; } = string.Empty;
    }
}
