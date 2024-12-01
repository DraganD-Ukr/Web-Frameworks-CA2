namespace WebFrameworks_CA2.Components.Models.Cinema;

public class Cinema {
    public string Name { get; set; }
    public string Address { get; set; }
    public double? Rating { get; set; }
    public int? UserRatingsCount { get; set; }
    public string Types { get; set; }
    public string GoogleLink { get; set; } // Link to Google Maps page
}