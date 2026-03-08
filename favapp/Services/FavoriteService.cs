using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using System.Text.Json;
using favapp.Models;

namespace favapp.Services
{
    /// <summary>
    /// Service responsable de la gestion des films favoris et des notes personnelles de l'utilisateur.
    /// Fait le pont entre les identifiants de l'API externe (TMDB) et les données enrichies localement.
    /// Utilise le LocalStorage pour persister ces données de manière sécurisée et nominative.
    /// </summary>
    public class FavoriteService
    {
        private readonly IJSRuntime _jsRuntime;
        private readonly AuthenticationStateProvider _authStateProvider;

        // La liste en mémoire qui contient nos boîtes (ID + Note)
        private List<FavoriteItem> _favorites = new List<FavoriteItem>();

        /// <summary>
        /// Initialise une nouvelle instance du service de gestion des favoris.
        /// </summary>
        /// <param name="jsRuntime">Outil pour interagir avec l'API JavaScript du navigateur (LocalStorage).</param>
        /// <param name="authStateProvider">Fournisseur d'état pour identifier l'utilisateur actuellement connecté.</param>
        public FavoriteService(IJSRuntime jsRuntime, AuthenticationStateProvider authStateProvider)
        {
            _jsRuntime = jsRuntime;
            _authStateProvider = authStateProvider;
        }

        /// <summary>
        /// Génère une clé de stockage unique basée sur le nom de l'utilisateur.
        /// Permet de cloisonner les données si plusieurs personnes utilisent le même ordinateur.
        /// </summary>
        /// <returns>Une chaîne formatée servant de clé pour le LocalStorage (ex: "favoris_notes_Thomas").</returns>
        private async Task<string> GetStorageKeyAsync()
        {
            var authState = await _authStateProvider.GetAuthenticationStateAsync();
            var username = authState.User.Identity?.Name ?? "anonyme";
            return $"favoris_notes_{username}";
        }

        /// <summary>
        /// Charge la liste des favoris enrichis (ID + Notes) depuis la mémoire du navigateur.
        /// À appeler généralement lors de l'initialisation de l'application ou à la connexion.
        /// </summary>
        public async Task LoadFavoritesAsync()
        {
            string key = await GetStorageKeyAsync();
            var json = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", key);

            if (!string.IsNullOrEmpty(json))
            {
                // On transforme le texte JSON en une vraie liste d'objets C#
                _favorites = JsonSerializer.Deserialize<List<FavoriteItem>>(json) ?? new List<FavoriteItem>();
            }
            else
            {
                _favorites = new List<FavoriteItem>();
            }
        }

        /// <summary>
        /// Ajoute un nouveau film à la liste des favoris de l'utilisateur, avec la possibilité d'y joindre une note.
        /// Intègre une sécurité anti-doublon basée sur l'identifiant du film.
        /// </summary>
        /// <param name="movieId">L'identifiant unique du film provenant de TMDB.</param>
        /// <param name="note">Un commentaire optionnel de l'utilisateur (vide par défaut).</param>
        public async Task AddToFavoritesAsync(int movieId, string note = "")
        {
            // LINQ Any() : Vérifie si la liste contient déjà au moins un élément avec cet ID
            if (!_favorites.Any(f => f.MovieId == movieId))
            {
                _favorites.Add(new FavoriteItem { MovieId = movieId, PersonalNote = note });
                await SaveToLocalStorageAsync();
            }
        }

        /// <summary>
        /// Retire définitivement un film et sa note associée de la liste des favoris.
        /// </summary>
        /// <param name="movieId">L'identifiant du film à supprimer.</param>
        public async Task RemoveFromFavoritesAsync(int movieId)
        {
            // LINQ FirstOrDefault() : Cherche le premier objet qui correspond, ou renvoie null s'il ne trouve rien
            var itemToRemove = _favorites.FirstOrDefault(f => f.MovieId == movieId);
            if (itemToRemove != null)
            {
                _favorites.Remove(itemToRemove);
                await SaveToLocalStorageAsync();
            }
        }

        /// <summary>
        /// Met à jour la note personnelle d'un film déjà présent dans les favoris.
        /// </summary>
        /// <param name="movieId">L'identifiant du film concerné.</param>
        /// <param name="newNote">Le nouveau texte de la note qui écrasera l'ancien.</param>
        public async Task UpdateNoteAsync(int movieId, string newNote)
        {
            var item = _favorites.FirstOrDefault(f => f.MovieId == movieId);
            if (item != null)
            {
                item.PersonalNote = newNote; // On modifie juste la note de cet objet précis
                await SaveToLocalStorageAsync();
            }
        }

        /// <summary>
        /// Vérifie de manière synchrone si un film fait partie des favoris de l'utilisateur.
        /// Particulièrement utile pour basculer l'état visuel d'un bouton (ex: Coeur vide / Coeur plein).
        /// </summary>
        /// <param name="movieId">L'identifiant du film à vérifier.</param>
        /// <returns>True si le film est dans la liste, False sinon.</returns>
        public bool IsFavorite(int movieId)
        {
            return _favorites.Any(f => f.MovieId == movieId);
        }

        /// <summary>
        /// Récupère la note personnelle associée à un film spécifique.
        /// </summary>
        /// <param name="movieId">L'identifiant du film.</param>
        /// <returns>La note en texte, ou une chaîne vide si le film n'a pas de note ou n'est pas en favori.</returns>
        public string GetNote(int movieId)
        {
            var item = _favorites.FirstOrDefault(f => f.MovieId == movieId);
            // L'opérateur '?.' vérifie si item est null avant de lire PersonalNote. 
            // '??' fournit une valeur de secours (string.Empty) si c'était null.
            return item?.PersonalNote ?? string.Empty;
        }

        /// <summary>
        /// Récupère l'intégralité de la collection des favoris (Films et Notes) de l'utilisateur actuel.
        /// </summary>
        /// <returns>Une liste d'objets <see cref="FavoriteItem"/>.</returns>
        public List<FavoriteItem> GetFavorites()
        {
            return _favorites;
        }

        /// <summary>
        /// Sérialise la liste courante des favoris en JSON et l'enregistre physiquement 
        /// dans le LocalStorage du navigateur pour garantir la persistance des données.
        /// </summary>
        private async Task SaveToLocalStorageAsync()
        {
            string key = await GetStorageKeyAsync();
            var json = JsonSerializer.Serialize(_favorites);
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", key, json);
        }
    }
}