namespace WebFrameworks_CA2.Components.Models.Movie;

public class FullOmdbMovie {
    public String ImdbID { get; set; }
    public String Poster { get; set; }
    public String Title { get; set; }
    public String Released { get; set; }
    public String Country { get; set; }
    public String Language { get; set; }
    public String RunTime { get; set; }
    public String Genre { get; set; }
    public String Type { get; set; }
    public String Actors { get; set; }
    public String Plot { get; set; }
    public List<Rating> Ratings { get; set; }
    
}