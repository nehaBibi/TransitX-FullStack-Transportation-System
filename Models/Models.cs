using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TransitX.Models
{
    // ── Database Entities ──────────────────────────────────────────────────────

    public class Route
    {
        public int Id { get; set; }

        [Required, StringLength(100)]
        public string Origin { get; set; } = string.Empty;

        [Required, StringLength(100)]
        public string Destination { get; set; } = string.Empty;

        [Required, StringLength(50)]
        public string VehicleType { get; set; } = string.Empty;

        [Required, StringLength(20)]
        public string DepartureTime { get; set; } = string.Empty;

        [Required, StringLength(20)]
        public string Status { get; set; } = "On Time"; // "On Time" | "Delayed"

        [StringLength(20)]
        public string DelayInfo { get; set; } = string.Empty;

        [Column(TypeName = "decimal(10,2)")]
        public decimal Price { get; set; }

        [StringLength(500)]
        public string ImageUrl { get; set; } = string.Empty;

        public int AvailableSeats { get; set; } = 40;

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();

        [NotMapped]
        public string FullRoute => $"{Origin} → {Destination}";
        [NotMapped]
        public bool IsOnTime => Status == "On Time";
    }

    public class Booking
    {
        public int Id { get; set; }

        [Required, StringLength(20)]
        public string BookingRef { get; set; } = string.Empty;

        public int RouteId { get; set; }

        [Required, StringLength(100)]
        public string PassengerName { get; set; } = string.Empty;

        [Required, EmailAddress, StringLength(200)]
        public string PassengerEmail { get; set; } = string.Empty;

        [Range(1, 10)]
        public int Seats { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal TotalPrice { get; set; }

        [Required, StringLength(20)]
        public string Status { get; set; } = "Confirmed"; // "Confirmed" | "Cancelled"

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public Route? Route { get; set; }
    }

    public class ContactMessage
    {
        public int Id { get; set; }

        [Required, StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required, EmailAddress, StringLength(200)]
        public string Email { get; set; } = string.Empty;

        [Required, StringLength(2000)]
        public string Message { get; set; } = string.Empty;

        public bool IsRead { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    // ── View / DTO Models ──────────────────────────────────────────────────────

    public class RouteCard
    {
        public int Id { get; set; }
        public string Origin { get; set; } = string.Empty;
        public string Destination { get; set; } = string.Empty;
        public string VehicleType { get; set; } = string.Empty;
        public string DepartureTime { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string DelayInfo { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public int AvailableSeats { get; set; }

        public string FullRoute => $"{Origin} → {Destination}";
        public bool IsOnTime => Status == "On Time";
    }

    public class TrackingInfo
    {
        public string Route { get; set; } = string.Empty;
        public string VehicleType { get; set; } = string.Empty;
        public string DepartureTime { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public int ProgressPercent { get; set; }
        public string Eta { get; set; } = string.Empty;
        public bool IsDelayed => Status == "Delayed";
    }

    public class ContactFormModel
    {
        [Required(ErrorMessage = "Name is required")]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Message is required")]
        [StringLength(2000, MinimumLength = 10)]
        public string Message { get; set; } = string.Empty;
    }

    public class BookingModel
    {
        [Required]
        public int RouteId { get; set; }

        [Required]
        public string Route { get; set; } = string.Empty;

        [Required]
        [Range(1, 10)]
        public int Seats { get; set; }

        [Required]
        public decimal Price { get; set; }

        [Required, StringLength(100)]
        public string PassengerName { get; set; } = string.Empty;

        [Required, EmailAddress, StringLength(200)]
        public string PassengerEmail { get; set; } = string.Empty;

        public decimal TotalPrice => Price * Seats;
    }

    public class HomeViewModel
    {
        public List<RouteCard> Routes { get; set; } = new();
        public List<TrackingInfo> TrackingList { get; set; } = new();
        public ContactFormModel ContactForm { get; set; } = new();
        public DashboardStats Stats { get; set; } = new();
    }

    public class DashboardStats
    {
        public int TotalRoutes { get; set; }
        public int TotalBookings { get; set; }
        public int TotalMessages { get; set; }
        public decimal TotalRevenue { get; set; }
    }
}
