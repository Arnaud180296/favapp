using favapp.Config;
using favapp.Models;
using System.Net.Http.Json;

namespace favapp.Services;

/// <summary>
/// Service responsable de la communication avec l'API publique The Movie Database (TMDB).
/// Gère la création des requêtes HTTP, l'authentification par Bearer Token, 
/// et la désérialisation des réponses JSON en objets C#.
/// </summary>
public class TmdbService : ITmdbService
{
    //HttpClient sert à effectuer des requetes Http
    private readonly HttpClient _httpClient;

    /// <summary>
    /// Initialise une nouvelle instance du service TMDB.
    /// </summary>
    /// <param name="httpClient">L'instance de HttpClient injectée par le système de dépendances de Blazor.</param>
    public TmdbService(HttpClient httpClient) 
    { 
        _httpClient = httpClient;
    }

    /// <summary>
    /// Récupère la liste des films actuellement populaires sur TMDB.
    /// </summary>
    /// <returns>Une liste d'objets <see cref="Movie"/>. Retourne une liste vide en cas d'erreur réseau ou d'API.</returns>
    public async Task<List<Movie>> GetPopularMovieAsync()
    {
        try
        {
            /*$"https://api.themoviedb.org/3/movie/popular?api_key={ApiConfig.API_KEY}&language=fr-FR";*/
            string url = $"https://api.themoviedb.org/3/movie/popular?language=fr-FR";

            var request = new HttpRequestMessage(HttpMethod.Get, url);

            request.Headers.Add("Authorization", $"Bearer {ApiConfig.API_KEY}");

            var reponse = await _httpClient.SendAsync(request);

            if (reponse.IsSuccessStatusCode)
            {
                var reponseApi = await reponse.Content.ReadFromJsonAsync<ReponseApiTmdb>();

                if (reponseApi != null)
                    return reponseApi.ApiResults;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ERREUR CRITIQUE : {ex.Message}");
        }

        return new List<Movie>();
    }

    /// <summary>
    /// Récupère les détails complets d'un film spécifique en interrogeant l'API TMDB.
    /// </summary>
    /// <param name="id">L'identifiant unique du film (ID TMDB).</param>
    /// <returns>Un objet <see cref="Movie"/> contenant les informations du film. Retourne un film vide en cas d'erreur.</returns>
    public async Task<Movie> GetMovieDetailsAsync(int id)
    {
        try
        {
            /*$"https://api.themoviedb.org/3/movie/popular?api_key={ApiConfig.API_KEY}&language=fr-FR";*/
            string url = $"https://api.themoviedb.org/3/movie/{id}?language=fr-FR";

            var request = new HttpRequestMessage(HttpMethod.Get, url);

            request.Headers.Add("Authorization", $"Bearer {ApiConfig.API_KEY}");

            var reponse = await _httpClient.SendAsync(request);

            if (reponse.IsSuccessStatusCode)
            {
                var reponseApi = await reponse.Content.ReadFromJsonAsync<Movie>();

                if (reponseApi != null)
                    return reponseApi;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ERREUR CRITIQUE : {ex.Message}");
        }

        return new Movie();
    }
}
