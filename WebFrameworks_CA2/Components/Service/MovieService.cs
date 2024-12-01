using WebFrameworks_CA2.Components.Models.Movie;

namespace WebFrameworks_CA2.Components.Service;

public class MovieService {
 
    private readonly HttpClient _httpClient;
    private readonly ILogger<MovieService> _logger;

    public MovieService(HttpClient httpClient, ILogger<MovieService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<OmdbSearchResponse> FetchMoviesAsync(string searchTerm, int page)
    {
        var apiKey = Environment.GetEnvironmentVariable("OMDB_API_KEY");

        if (string.IsNullOrEmpty(apiKey))
        {
            _logger.LogError("API key is not set in the environment variables.");
            throw new InvalidOperationException("API key is not set in the environment variables.");
        }

        try
        {
            var response = await _httpClient.GetAsync($"https://www.omdbapi.com/?s={searchTerm}&page={page}&apikey={apiKey}");
            _logger.LogInformation("API Response: {Response}", response.Content.ReadAsStringAsync().Result);
            response.EnsureSuccessStatusCode(); // Will throw an exception for non-success codes

            var result = await response.Content.ReadFromJsonAsync<OmdbSearchResponse>();
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching movies.");
            return null;
        }
    }   
}