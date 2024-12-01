using WebFrameworks_CA2.Components.Models.Movie;

namespace WebFrameworks_CA2.Components.Service;

public class MovieService {
    private readonly HttpClient _httpClient;
    private readonly ILogger<MovieService> _logger;
    private readonly string _apiKey;

    public MovieService(HttpClient httpClient, ILogger<MovieService> logger) {
        _httpClient = httpClient;
        _logger = logger;
        _apiKey = Environment.GetEnvironmentVariable("OMDB_API_KEY");
    }

    public async Task<OmdbSearchResponse> FetchMoviesAsync(string searchTerm, int page) {
        if (string.IsNullOrEmpty(_apiKey)) {
            _logger.LogError("API key is not set in the environment variables.");
            throw new InvalidOperationException("API key is not set in the environment variables.");
        }

        try {
            var response =
                await _httpClient.GetAsync($"https://www.omdbapi.com/?s={searchTerm}&page={page}&apikey={_apiKey}");
            _logger.LogInformation("API Response: {Response}", response.Content.ReadAsStringAsync().Result);
            response.EnsureSuccessStatusCode(); // Will throw an exception for non-success codes

            var result = await response.Content.ReadFromJsonAsync<OmdbSearchResponse>();
            return result;
        }
        catch (Exception ex) {
            _logger.LogError(ex, "Error occurred while fetching movies.");
            return null;
        }
    }


    public async Task<FullOmdbMovie> GetMovieDetailsAsync(string imdbID) {
        var apiKey = Environment.GetEnvironmentVariable("OMDB_API_KEY");

        if (string.IsNullOrEmpty(apiKey)) {
            _logger.LogError("API key is not set in the environment variables.");
            throw new InvalidOperationException("API key is not set in the environment variables.");
        }

        try {
            var response = await _httpClient.GetAsync($"https://www.omdbapi.com/?i={imdbID}&apikey={apiKey}");

            if (response.IsSuccessStatusCode) {
                return await response.Content.ReadFromJsonAsync<FullOmdbMovie>();
            }
            else {
                _logger.LogWarning("Failed to fetch movie details. Status code: {StatusCode}", response.StatusCode);
                return null;
            }
        }
        catch (Exception ex) {
            _logger.LogError(ex, "Error fetching movie details.");
            throw;
        }
    }
}