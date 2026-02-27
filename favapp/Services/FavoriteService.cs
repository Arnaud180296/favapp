using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using System.Text.Json;

namespace favapp.Services
{
    public class FavoriteService
    {
        private readonly IJSRuntime _jsRuntime;
        private readonly AuthenticationStateProvider _authStateProvider; // On ajoute le douanier
        private List<int> _favoriteMovieIds = new List<int>();

        // On modifie le constructeur pour accepter le douanier
        public FavoriteService(IJSRuntime jsRuntime, AuthenticationStateProvider authStateProvider)
        {
            _jsRuntime = jsRuntime;
            _authStateProvider = authStateProvider;
        }

        // LA MAGIE EST ICI : On crée une clé unique par utilisateur
        private async Task<string> GetStorageKeyAsync()
        {
            // On demande au douanier qui est là
            var authState = await _authStateProvider.GetAuthenticationStateAsync();

            // On récupère le nom (ou "anonyme" par sécurité)
            var username = authState.User.Identity?.Name ?? "anonyme";

            // On renvoie un nom de tiroir unique, ex: "favoris_Thomas"
            return $"favoris_{username}";
        }

        public async Task LoadFavoritesAsync()
        {
            // On demande le bon tiroir
            string key = await GetStorageKeyAsync();

            var json = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", key);
            if (!string.IsNullOrEmpty(json))
            {
                _favoriteMovieIds = JsonSerializer.Deserialize<List<int>>(json) ?? new List<int>();
            }
            else
            {
                // Très important : si c'est un nouvel utilisateur, on vide la liste en mémoire !
                _favoriteMovieIds = new List<int>();
            }
        }

        public async Task AddToFavoritesAsync(int movieId)
        {
            if (!_favoriteMovieIds.Contains(movieId))
            {
                _favoriteMovieIds.Add(movieId);
                await SaveToLocalStorageAsync();
            }
        }

        public async Task RemoveFromFavoritesAsync(int movieId)
        {
            if (_favoriteMovieIds.Contains(movieId))
            {
                _favoriteMovieIds.Remove(movieId);
                await SaveToLocalStorageAsync();
            }
        }

        public bool IsFavorite(int movieId)
        {
            return _favoriteMovieIds.Contains(movieId);
        }

        public List<int> GetFavoriteIds()
        {
            return _favoriteMovieIds;
        }

        private async Task SaveToLocalStorageAsync()
        {
            // On sauvegarde dans le bon tiroir
            string key = await GetStorageKeyAsync();
            var json = JsonSerializer.Serialize(_favoriteMovieIds);
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", key, json);
        }
    }
}