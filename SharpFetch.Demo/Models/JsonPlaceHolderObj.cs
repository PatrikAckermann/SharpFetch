using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace SharpFetch.Demo.Models
{
    public class JsonPlaceHolderObj
    {
        [JsonPropertyName("userId")]
        public int UserId { get; set; }

        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("title")]
        public string? Title { get; set; }

        [JsonPropertyName("completed")]
        public bool Completed { get; set; }
    }
}
