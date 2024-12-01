using WebFrameworks_CA2.Components.Models.Cinema;

namespace WebFrameworks_CA2.Components.Service;

public interface ICinemaService {
    Task<List<Cinema>> GetNearbyCinemasAsync(double latitude, double longitude, int radius);
}