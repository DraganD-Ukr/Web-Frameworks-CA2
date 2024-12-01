using Moq;
using WebFrameworks_CA2.Components.Service;
using Microsoft.Extensions.Logging;
using System.Net;
using Moq.Protected;
using WebFrameworks_CA2.Components.Models.Movie;
using System.Text.Json;

public class MovieServiceTests
{
    private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;
    private readonly Mock<ILogger<MovieService>> _mockLogger;
    private readonly MovieService _movieService;
    private readonly HttpClient _httpClient;

    public MovieServiceTests()
    {
        // A mock for HttpMessageHandler to intercept the SendAsync call
        _mockHttpMessageHandler = new Mock<HttpMessageHandler>();

        // HttpClient instance using the mocked HttpMessageHandler
        _httpClient = new HttpClient(_mockHttpMessageHandler.Object);

        _mockLogger = new Mock<ILogger<MovieService>>();

        // Instantiate MovieService with the mocked HttpClient and logger
        _movieService = new MovieService(_httpClient, _mockLogger.Object);
    }

    [Fact]
    public async Task FetchMoviesAsync_ShouldReturnMovies_WhenApiCallSucceeds()
    {
     
        var expectedResponse = new OmdbSearchResponse
        {
            Search = new List<OmdbMovie>
            {
                new OmdbMovie
                {
                    Title = "Movie 1",
                    Year = "2023",
                    ImdbID = "tt1234567"
                },
                new OmdbMovie
                {
                    Title = "Movie 2",
                    Year = "2022",
                    ImdbID = "tt2345678"
                }
            }
        };

        var jsonResponse = JsonSerializer.Serialize(expectedResponse);
        var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(jsonResponse)
        };

        // Mock the SendAsync method of HttpMessageHandler to return the mocked response
        _mockHttpMessageHandler
            .Protected<HttpMessageHandler>()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(httpResponseMessage);

       
        var result = await _movieService.FetchMoviesAsync("Test", 1);

        
        Assert.NotNull(result);
        Assert.Equal(2, result.Search.Count);  // Ensure two movies are returned
        Assert.Equal("Movie 1", result.Search[0].Title);  // Ensure the first movie title is correct
    }

    [Fact]
    public async Task FetchMoviesAsync_ShouldThrowException_WhenApiCallFails()
    {
   
        var searchTerm = "Test";
        var page = 1;

        // Mocking an unsuccessful response to simulate an API failure
        var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.BadRequest)
        {
            Content = new StringContent("Error")
        };

        // Mock the SendAsync method of HttpMessageHandler to return the mocked response
        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(httpResponseMessage);
        
        var result = await _movieService.FetchMoviesAsync(searchTerm, page);
        
        Assert.Null(result);  // Expecting null or handle the error as your service logic dictates
    }

    [Fact]
    public async Task GetMovieDetailsAsync_ShouldReturnMovieDetails_WhenApiCallSucceeds()
    {
        var expectedMovie = new FullOmdbMovie
        {
            Title = "Movie 1",
            Released = "29 Oct 2004",
            ImdbID = "tt1234567",
            Plot = "A thrilling adventure"
        };

        var jsonResponse = JsonSerializer.Serialize(expectedMovie);
        var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(jsonResponse)
        };

        // Mock the SendAsync method of HttpMessageHandler to return the mocked response
        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(httpResponseMessage);
        
        var result = await _movieService.GetMovieDetailsAsync("tt1234567");

        Assert.NotNull(result);
        Assert.Equal("Movie 1", result.Title);  // Ensure the movie title is correct
        Assert.Equal("29 Oct 2004", result.Released);  // Ensure the movie year is correct
    }

    [Fact]
    public async Task GetMovieDetailsAsync_ShouldReturnNull_WhenApiCallFails()
    {
        var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.BadRequest)
        {
            Content = new StringContent("Error")
        };

        // Mock the SendAsync method to simulate a failed API call
        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(httpResponseMessage);

        var result = await _movieService.GetMovieDetailsAsync("tt1234567");
        
        Assert.Null(result);  // Expecting null when the API call fails
    }

    [Fact]
    public async Task FetchMoviesAsync_ShouldThrowInvalidOperationException_WhenApiKeyIsMissing()
    {
        // Arrange: Set the API key to null or empty
        var originalApiKey = Environment.GetEnvironmentVariable("OMDB_API_KEY");
        try
        {
            // Set the API key to null or empty
            Environment.SetEnvironmentVariable("OMDB_API_KEY", null);

            // Create a new instance of MovieService with mocked dependencies
            var movieService = new MovieService(_httpClient, _mockLogger.Object);
            
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                movieService.FetchMoviesAsync("Test", 1));

            Assert.Equal("API key is not set in the environment variables.", exception.Message);
        }
        finally
        {
            // Restore the original API key
            Environment.SetEnvironmentVariable("OMDB_API_KEY", originalApiKey);
        }
    }
}
