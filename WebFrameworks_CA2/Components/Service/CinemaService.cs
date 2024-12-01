namespace WebFrameworks_CA2.Components.Service;

using System.Net.Http;
using System.Xml.Serialization;
using Models.Cinema;

public class CinemaService
{
    private readonly ILogger<CinemaService> _logger;
    private readonly string _googleApiKey;
    private readonly HttpClient _httpClient;

    public CinemaService(ILogger<CinemaService> logger)
    {
        _httpClient = new HttpClient();
        _logger = logger;
        _googleApiKey = Environment.GetEnvironmentVariable("GOOGLE_API_KEY") ?? throw new InvalidOperationException("Google API key is not set");
    }

    public async Task<List<Cinema>> GetNearbyCinemasAsync(double latitude, double longitude, int radius)
    {
        var apiUrl = $"https://maps.googleapis.com/maps/api/place/nearbysearch/xml?location={latitude},{longitude}&radius={radius}&type=movie_theater&key={_googleApiKey}";
        _logger.LogDebug($"Fetching cinemas with API URL: {apiUrl}");

        try
        {
            var response = await _httpClient.GetStringAsync(apiUrl);
            _logger.LogDebug("Response received from Google API.");

            var xmlSerializer = new XmlSerializer(typeof(GooglePlacesResponse));
            using var stringReader = new StringReader(response);

            var result = (GooglePlacesResponse)xmlSerializer.Deserialize(stringReader);
            if (result?.Results == null || !result.Results.Any())
            {
                _logger.LogInformation("No cinemas found within the specified radius.");
                return new List<Cinema>();
            }

            _logger.LogInformation($"Found {result.Results.Count} cinemas.");
            return result.Results.Select(r => new Cinema
            {
                Name = r.Name,
                Address = r.Vicinity,
                Rating = r.Rating,
                UserRatingsCount = r.UserRatingsCount,
                GoogleLink = $"https://www.google.com/maps/place/?q=place_id:{r.PlaceId}"
            }).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error fetching cinemas: {ex.Message}");
            throw;
        }
    }
}
