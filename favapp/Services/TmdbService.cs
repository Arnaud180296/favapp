using favapp.Config;
using favapp.Models;
using System.Net.Http.Json;

namespace favapp.Services;


public class TmdbService : ITmdbService
{
    //HttpClient sert à effectuer des requetes Http
    private readonly HttpClient _httpClient;
    public TmdbService(HttpClient httpClient) 
    { 
        _httpClient = httpClient;
    }

    public async Task<List<Movie>> GetPopularMovieAsync()
    {
        string url = $"https://api.themoviedb.org/3/movie/popular?api_key={ApiConfig.API_KEY}&language=fr-FR";

        var reponse = await _httpClient.GetFromJsonAsync<ReponseApiTmdb>(url);

        if (reponse != null)
            return reponse.ApiResultats;

        return new List<Movie>();
    }
}
