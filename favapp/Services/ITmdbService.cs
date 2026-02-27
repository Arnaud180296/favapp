using favapp.Models;

namespace favapp.Services
{
    public interface ITmdbService
    {
        // Task permet de ne pas geler l'application
        // Je crois que c'est un thread
        // en gros pendant que l'application recupere les donnees de l'api le reste de l'application ne fige pas 
        Task<List<Movie>> GetPopularMovieAsync();

        Task<Movie> GetMovieDetailsAsync(int id);

    }
}
