using favapp.Models;

namespace favapp.Services
{
    /// <summary>
    /// Interface définissant le contrat pour le service de communication avec l'API TMDB.
    /// Avoir une interface permet de séparer la définition des méthodes de leur implémentation,
    /// ce qui est une bonne pratique pour l'injection de dépendances dans Program.cs.
    /// </summary>
    public interface ITmdbService
    {
        // Task permet de ne pas geler l'application
        // Je crois que c'est un thread
        // en gros pendant que l'application recupere les donnees de l'api le reste de l'application ne fige pas
        // 

        /// <summary>
        /// Récupère de manière asynchrone la liste des films populaires actuels.
        /// </summary>
        /// <returns>Une tâche asynchrone contenant la liste des films.</returns>
        Task<List<Movie>> GetPopularMovieAsync();


        /// <summary>
        /// Récupère de manière asynchrone les détails complets d'un film selon son identifiant.
        /// </summary>
        /// <param name="id">L'identifiant unique du film (ID TMDB).</param>
        /// <returns>Une tâche asynchrone contenant l'objet <see cref="Movie"/>.</returns>
        Task<Movie> GetMovieDetailsAsync(int id);

    }
}
