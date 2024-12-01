namespace WebFrameworks_CA2.Components.Models.Cinema;

[System.Xml.Serialization.XmlRoot("PlaceSearchResponse")]
public class GooglePlacesResponse {
    
    [System.Xml.Serialization.XmlElement("result")]
    public List<GooglePlaceResult> Results { get; set; }
    
}