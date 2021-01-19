using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PollLibrary.Polls
{
    public interface IQuestion
    {
        [JsonPropertyName("Name")] string Name { get; set; }
        [JsonPropertyName("MultipleQuestions")] Dictionary<char, string> MultipleQuestion { get; set; }
        [JsonPropertyName("RightAnswer")] string RightAnswer { get; set; }
    }
}