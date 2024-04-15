using System;
using System.Text.Json.Serialization;
using static System.Formats.Asn1.AsnWriter;

namespace Hacker.Models
{
    public class Story
    {
        public string? Title { get; set; }
        public string? Url { get; set; }
        public string? PostedBy { get; set; }
        public DateTime Time { get; set; }
        public int Score { get; set; }
        public int CommentCount { get; set; }
    }

    public class StoryDto
    {
        [JsonPropertyName("title")]
        public string? Title { get; set; }
        [JsonPropertyName("url")]
        public string? Url { get; set; }
        [JsonPropertyName("by")]
        public string? By { get; set; }
        [JsonPropertyName("time")]
        public long Time { get; set; }
        [JsonPropertyName("score")]
        public int Score { get; set; }
        [JsonPropertyName("descendants")]
        public int Descendants { get; set; }
    }

    public static class StoryExtensions
    {
        public static Story MapToObject(this StoryDto dto)
        {
            return new Story
            {
                Title = dto.Title,
                Url = dto.Url,
                PostedBy = dto.By,
                Time = DateTimeOffset.FromUnixTimeSeconds(dto.Time).UtcDateTime,
                Score = dto.Score,
                CommentCount = dto.Descendants,
            };
        }
    }
}
