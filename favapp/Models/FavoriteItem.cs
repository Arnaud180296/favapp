namespace favapp.Models
{
    /// <summary>
    /// Représente un film sauvegardé en favori avec les données personnalisées de l'utilisateur.
    /// Fait le lien entre l'API externe (MovieId) et les données locales (PersonalNote).
    /// </summary>
    public class FavoriteItem
    {
        /// <summary>
        /// L'identifiant unique du film provenant de l'API TMDB.
        /// </summary>
        public int MovieId { get; set; }

        /// <summary>
        /// Le commentaire ou la note personnelle ajoutée par l'utilisateur.
        /// </summary>
        public string PersonalNote { get; set; } = string.Empty;
    }
}
