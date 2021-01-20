using System.Text.Json.Serialization;

namespace PollLibrary.Polls
{
    public class Statistics
    {
        [JsonPropertyName("tries")]
        public int Tries { get; set; }
        [JsonPropertyName("averageright")]
        public decimal AverageRight { get; set; }

        public override string ToString()
        {
            return $"Tries {Tries} , Average right answers: {AverageRight}" ;
        }
    }
}