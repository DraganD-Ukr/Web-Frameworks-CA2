namespace WebFrameworks_CA2.Components.Models.Movie;

public class OmdbSearchResponse {
    public string Response { get; set; }
    public string Error { get; set; }
    public List<OmdbMovie> Search { get; set; }
}