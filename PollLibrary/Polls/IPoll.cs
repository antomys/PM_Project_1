using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PollLibrary.Polls
{
    public interface IPoll
    {
        [JsonPropertyName("poll")] string PollName { get; }
        [JsonPropertyName("amount")] int QuestionAmount { get; }
        [JsonPropertyName("questions")] List<Question> Questions { get; }
        [JsonPropertyName("stats")] Statistics Statistics { get; }
        
    }
}