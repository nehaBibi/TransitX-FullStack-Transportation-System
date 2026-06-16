using TransitX.Models;

namespace TransitX.Services
{
    public interface ITransitService
    {
        Task<List<RouteCard>> GetRoutesAsync(string? searchQuery = null);
        Task<RouteCard?> GetRouteByIdAsync(int id);
        Task<List<TrackingInfo>> GetTrackingDataAsync();
        Task<DashboardStats> GetStatsAsync();

        Task<(bool Success, string BookingRef, string Message)> CreateBookingAsync(BookingModel model);
        Task<Booking?> GetBookingByRefAsync(string bookingRef);
        Task<bool> CancelBookingAsync(string bookingRef);

        Task<bool> SaveContactMessageAsync(ContactFormModel model);
        Task<int> ClearAllBookingsAsync();
    }
}
