using System.Text.Json.Serialization;

namespace favapp.Models
{
    /// <summary>
    /// Modèle de données représentant un film.
    /// Utilisé pour désérialiser automatiquement les réponses JSON provenant de l'API publique TMDB.
    /// Les attributs [JsonPropertyName] font le lien exact entre les clés du JSON de l'API et les propriétés C#.
    /// </summary>
    public class Movie
    {
        /// <summary>
        /// Identifiant unique du film défini par la base de données TMDB.
        /// </summary>
        [JsonPropertyName("id")]
        public int Id { get; set; }

        /// <summary>
        /// Liste des identifiants correspondant aux genres du film (ex: Action, Comédie).
        /// </summary>
        [JsonPropertyName ("genre_ids")] 
        public List<int> Genre { get; set; }

        /// <summary>
        /// Titre du film.
        /// </summary>
        [JsonPropertyName("title")]
        public string Title { get; set; }

        /// <summary>
        /// Date de sortie officielle du film (généralement au format "YYYY-MM-DD").
        /// </summary>
        [JsonPropertyName("release_date")]
        public string ReleaseDate {  get; set; }

        /// <summary>
        /// Note moyenne attribuée au film par la communauté TMDB (sur 10).
        /// </summary>
        [JsonPropertyName("vote_average")]
        public double VoteAverage { get; set; }

        /// <summary>
        /// Chemin d'accès relatif vers l'image de l'affiche du film.
        /// Pour afficher l'image, cette chaîne doit être combinée avec l'URL de base de TMDB 
        /// (ex: "https://image.tmdb.org/t/p/w500" + PosterPath).
        /// </summary>
        [JsonPropertyName("poster_path")]
        public string PosterPath { get; set; }

        
    }
}
