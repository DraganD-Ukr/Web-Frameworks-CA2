namespace WebFrameworks_CA2.Components.Models;

public class OmdbSearchResponse {
    public string Response { get; set; }
    public string Error { get; set; }
    public List<OmdbMovie> Search { get; set; }
}