namespace WebFrameworks_CA2.Components.Models.Cinema;

public class GooglePlaceResult {
    
    [System.Xml.Serialization.XmlElement("name")]
    public string Name { get; set; }
    
    [System.Xml.Serialization.XmlElement("vicinity")]
    public string Vicinity { get; set; }
    
    [System.Xml.Serialization.XmlElement("rating")]
    public double? Rating { get; set; }
    
    [System.Xml.Serialization.XmlElement("user_ratings_total")]
    public int? UserRatingsCount { get; set; }
    
    [System.Xml.Serialization.XmlElement("place_id")]
    public string PlaceId { get; set; } // Unique ID for the cinema place
}