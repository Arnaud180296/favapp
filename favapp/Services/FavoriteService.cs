using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using System.Text.Json;

namespace favapp.Services
{
    /// <summary>
    /// Service responsable de la gestion des films favoris de l'utilisateur.
    /// Utilise le LocalStorage du navigateur pour sauvegarder les données de manière persistante et nominative.
    /// </summary>
    public class FavoriteService
    {
        private readonly IJSRuntime _jsRuntime;
        private readonly AuthenticationStateProvider _authStateProvider; // On ajoute le douanier
        private List<int> _favoriteMovieIds = new List<int>();

        /// <summary>
        /// Initialise une nouvelle instance du service des favoris.
        /// </summary>
        /// <param name="jsRuntime">L'outil permettant d'exécuter du code JavaScript (pour accéder au LocalStorage).</param>
        /// <param name="authStateProvider">Le service d'authentification pour connaître l'utilisateur connecté.</param>
        public FavoriteService(IJSRuntime jsRuntime, AuthenticationStateProvider authStateProvider)
        {
            _jsRuntime = jsRuntime;
            _authStateProvider = authStateProvider;
        }

        /// <summary>
        /// Génère une clé de stockage unique basée sur le nom de l'utilisateur connecté.
        /// Permet de séparer les favoris si plusieurs utilisateurs utilisent le même navigateur.
        /// </summary>
        /// <returns>Une chaîne de caractères représentant la clé du LocalStorage (ex: "favoris_Thomas").</returns>
        private async Task<string> GetStorageKeyAsync()
        {
            // On demande au douanier qui est là
            var authState = await _authStateProvider.GetAuthenticationStateAsync();

            // On récupère le nom (ou "anonyme" par sécurité)
            var username = authState.User.Identity?.Name ?? "anonyme";

            // On renvoie un nom de tiroir unique, ex: "favoris_Thomas"
            return $"favoris_{username}";
        }

        /// <summary>
        /// Charge la liste des favoris depuis le LocalStorage pour l'utilisateur actuel.
        /// Si l'utilisateur n'a pas encore de favoris, initialise une liste vide en mémoire.
        /// </summary>
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

        /// <summary>
        /// Ajoute l'identifiant d'un film à la liste des favoris de l'utilisateur et sauvegarde l'état.
        /// </summary>
        /// <param name="movieId">L'identifiant unique du film (ID TMDB).</param>
        public async Task AddToFavoritesAsync(int movieId)
        {
            if (!_favoriteMovieIds.Contains(movieId))
            {
                _favoriteMovieIds.Add(movieId);
                await SaveToLocalStorageAsync();
            }
        }

        /// <summary>
        /// Retire l'identifiant d'un film de la liste des favoris et met à jour le LocalStorage.
        /// </summary>
        /// <param name="movieId">L'identifiant unique du film à retirer.</param>
        public async Task RemoveFromFavoritesAsync(int movieId)
        {
            if (_favoriteMovieIds.Contains(movieId))
            {
                _favoriteMovieIds.Remove(movieId);
                await SaveToLocalStorageAsync();
            }
        }

        /// <summary>
        /// Vérifie de manière synchrone si un film est présent dans la liste des favoris en mémoire.
        /// Utile pour modifier rapidement l'interface utilisateur (ex: couleur d'un bouton).
        /// </summary>
        /// <param name="movieId">L'identifiant du film à vérifier.</param>
        /// <returns>True si le film est en favori, sinon False.</returns>
        public bool IsFavorite(int movieId)
        {
            return _favoriteMovieIds.Contains(movieId);
        }


        /// <summary>
        /// Récupère la liste complète des identifiants des films mis en favoris par l'utilisateur.
        /// </summary>
        /// <returns>Une liste d'entiers représentant les ID des films.</returns>
        public List<int> GetFavoriteIds()
        {
            return _favoriteMovieIds;
        }

        /// <summary>
        /// Sérialise la liste actuelle des favoris au format JSON et l'enregistre dans le LocalStorage du navigateur.
        /// </summary>
        private async Task SaveToLocalStorageAsync()
        {
            // On sauvegarde dans le bon tiroir
            string key = await GetStorageKeyAsync();
            var json = JsonSerializer.Serialize(_favoriteMovieIds);
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", key, json);
        }
    }
}