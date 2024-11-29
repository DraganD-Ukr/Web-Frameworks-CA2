namespace WebFrameworks_CA2.Components.Models;

public class FullOmdbMovie {
    public String ImdbID { get; set; }
    public String Poster { get; set; }
    public String Title { get; set; }
    public DateOnly ReleaseDate { get; set; }
    public String ReleaseCountry { get; set; }
    public String Language { get; set; }
    public String RunTime { get; set; }
    public String Genre { get; set; }
    public String Type { get; set; }
    public String Actors { get; set; }
    public String Plot { get; set; }
    public List<Object> Ratings { get; set; }
    
}