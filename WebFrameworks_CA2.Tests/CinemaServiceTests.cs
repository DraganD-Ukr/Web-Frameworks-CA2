using Moq;
using WebFrameworks_CA2.Components.Service;
using Microsoft.Extensions.Logging;
using System.Net;
using Moq.Protected;
using System.Xml.Serialization;
using WebFrameworks_CA2.Components.Models.Cinema;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

public class CinemaServiceTests
{
    private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;
    private readonly Mock<ILogger<CinemaService>> _mockLogger;
    private readonly CinemaService _cinemaService;
    private readonly HttpClient _httpClient;

    public CinemaServiceTests()
    {
        // A mock for HttpMessageHandler to intercept the SendAsync call
        _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        
        // HttpClient instance using the mocked HttpMessageHandler
        _httpClient = new HttpClient(_mockHttpMessageHandler.Object);
        
        _mockLogger = new Mock<ILogger<CinemaService>>();
        
        // Instantiate CinemaService with the mocked HttpClient and logger
        _cinemaService = new CinemaService(_httpClient, _mockLogger.Object);
    }

    [Fact]
    public async Task GetNearbyCinemasAsync_ShouldReturnCinemas_WhenApiCallSucceeds()
    {
       
        var expectedCinemas = new GooglePlacesResponse
        {
            Results = new List<GooglePlaceResult>
            {
                new GooglePlaceResult
                {
                    Name = "Cinema 1", Vicinity = "New York", Rating = 4.5, UserRatingsCount = 100,
                    PlaceId = "place_id_1"
                },
                new GooglePlaceResult
                {
                    Name = "Cinema 2", Vicinity = "New York", Rating = 4.0, UserRatingsCount = 80,
                    PlaceId = "place_id_2"
                }
            }
        };
        
        // Serialize the expected response to XML
        var serializer = new XmlSerializer(typeof(GooglePlacesResponse));
        using var writer = new StringWriter();
        serializer.Serialize(writer, expectedCinemas);
        var responseContent = writer.ToString();

        var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseContent)
        };

        // Mock the SendAsync method of HttpMessageHandler to return the mocked response
        _mockHttpMessageHandler
            .Protected<HttpMessageHandler>()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(httpResponseMessage);
        
       
        var result = await _cinemaService.GetNearbyCinemasAsync(40.7128, -74.0060, 1000);

       
        Assert.NotNull(result);  
        Assert.NotEmpty(result);  
        Assert.Equal(2, result.Count);  // Ensure the expected number of cinemas are returned
        Assert.Equal("Cinema 1", result[0].Name);  // Ensure the first cinema name is correct
    }

    [Fact]
    public async Task GetNearbyCinemasAsync_ShouldThrowException_WhenApiCallFails()
    {
        
        var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.BadRequest)
        {
            Content = new StringContent("Error")
        };

        
        _mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync", 
                ItExpr.IsAny<HttpRequestMessage>(), 
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(httpResponseMessage);

        
        await Assert.ThrowsAsync<HttpRequestException>(() => 
            _cinemaService.GetNearbyCinemasAsync(40.7128, -74.0060, 1000));
    }
}
