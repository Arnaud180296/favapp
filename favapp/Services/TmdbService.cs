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
