using favapp.Models;
using System.Text.Json.Serialization;

namespace favapp.Services
{
    public class ReponseApiTmdb
    {
        [JsonPropertyName("results")]
        public List<Movie>? ApiResults { get; set; }

        //[JsonPropertyName("person")]
        // public Person ApiResults ...
    }
}
