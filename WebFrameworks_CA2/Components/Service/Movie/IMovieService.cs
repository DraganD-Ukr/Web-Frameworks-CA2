using WebFrameworks_CA2.Components.Models.Movie;

namespace WebFrameworks_CA2.Components.Service;

public interface IMovieService {
    Task<OmdbSearchResponse> FetchMoviesAsync(string searchTerm, int page);
    Task<FullOmdbMovie> GetMovieDetailsAsync(string imdbID);
}