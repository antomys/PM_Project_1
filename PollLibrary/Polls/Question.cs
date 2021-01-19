using System.Collections.Generic;

namespace PollLibrary.Polls
{
    public class Question : IQuestion
    {
        public Question(string name, Dictionary<char, string> multipleQuestion, string rightAnswer)
        {
            Name = name;
            MultipleQuestion = multipleQuestion;
            RightAnswer = rightAnswer;
        }
        public string Name { get; set; }
        public Dictionary<char, string> MultipleQuestion { get; set; }
        public string RightAnswer { get; set; }
    }
}