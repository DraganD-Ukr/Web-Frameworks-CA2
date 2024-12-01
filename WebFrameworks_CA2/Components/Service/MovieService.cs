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

    /// <summary>
    /// Asynchronously fetches movies from the OMDB API based on the provided search term and page number.
    /// </summary>
    /// <param name="searchTerm">The term to search for movies.</param>
    /// <param name="page">The page number for the paginated search results.</param>
    /// <returns>
    /// A task representing the asynchronous operation, containing an <see cref="OmdbSearchResponse"/> with search results.
    /// If the operation fails, the method logs an error and returns <c>null</c>.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the OMDB API key is not set in the environment variables.
    /// </exception>
    /// <exception cref="Exception">
    /// Thrown when there is an error during the HTTP request or processing of the response.
    /// </exception>
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


    /// <summary>
    /// Asynchronously fetches detailed information about a specific movie using its IMDb ID.
    /// </summary>
    /// <param name="imdbID">The IMDb ID of the movie to fetch details for.</param>
    /// <returns>
    /// A task representing the asynchronous operation, containing a <see cref="FullOmdbMovie"/> object with detailed movie information.
    /// If the movie details cannot be fetched, the method returns <c>null</c>.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the OMDB API key is not set in the environment variables.
    /// </exception>
    /// <exception cref="Exception">
    /// Thrown when there is an error during the HTTP request or processing of the response.
    /// </exception>
    public async Task<FullOmdbMovie> GetMovieDetailsAsync(string imdbID) {

        if (string.IsNullOrEmpty(_apiKey)) {
            _logger.LogError("API key is not set in the environment variables.");
            throw new InvalidOperationException("API key is not set in the environment variables.");
        }

        try {
            var response = await _httpClient.GetAsync($"https://www.omdbapi.com/?i={imdbID}&apikey={_apiKey}");

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