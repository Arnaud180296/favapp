using favapp.Models;
using System.Text.Json.Serialization;

namespace favapp.Services
{
    /// <summary>
    /// Classe "Wrapper" (conteneur) utilisée pour désérialiser les réponses de l'API TMDB 
    /// qui renvoient des listes (comme la liste des films populaires).
    /// TMDB ne renvoie pas directement un tableau JSON, mais un objet global contenant 
    /// un "tiroir" nommé "results" dans lequel se trouvent les films.
    /// </summary>
    public class ReponseApiTmdb
    {
        /// <summary>
        /// La liste des films extraite de la réponse API.
        /// L'attribut [JsonPropertyName("results")] indique au système de lire 
        /// précisément cette clé dans le fichier JSON reçu.
        /// </summary>
        [JsonPropertyName("results")]
        public List<Movie>? ApiResults { get; set; }

    }
}
