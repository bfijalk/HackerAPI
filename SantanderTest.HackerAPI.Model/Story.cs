using SantanderTest.HackerAPI.Model.Helpers;
using System.Text.Json.Serialization;

namespace SantanderTest.HackerAPI.Model
{
    public class StoryDto
    {
        public int Id { get; set; }
        [JsonPropertyName("title")]
        public string? Title { get; set; }

        [JsonPropertyName("url")]
        public string? Uri { get; set; }

        [JsonPropertyName("by")]
        public string? PostedBy { get; set; }

        [JsonPropertyName("time")]
        [JsonConverter(typeof(UnixDateTimeOffsetConverter))]
        public DateTimeOffset Time { get; set; }

        [JsonPropertyName("score")]
        public int Score { get; set; }


        [JsonPropertyName("descendants")]
        public int Descendants { get; set; }

        public Story MapToObject()
        {
            return new Story
            {
                Title = Title,
                Uri = Uri,
                PostedBy = PostedBy,
                Time = Time,
                Score = Score,
                CommentCount = Descendants
            };
        }
    }

    public class Story
    {
        public string? Title { get; set; }
        public string? Uri { get; set; }
        public string? PostedBy { get; set; }
        public DateTimeOffset Time { get; set; }
        public int Score { get; set; }
        public int CommentCount { get; set; }
    }
}
