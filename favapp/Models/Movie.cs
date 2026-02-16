using System.Text.Json.Serialization;

namespace favapp.Models
{
    public class Movie
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName ("genre_ids")] 
        public string Genre { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("release_date")]
        public string ReleaseDate {  get; set; }

        [JsonPropertyName("vote_average")]
        public int VoteAverage { get; set; }

        [JsonPropertyName("poster_path")]
        public string PosterPath { get; set; }

        
    }
}
