using System;
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
        public override string ToString()
        {
            var result = $"Question: {Name}\n\tAnswers: ";
            if (MultipleQuestion == null) return result + $"\nRight Answer: {RightAnswer}";
            foreach (var (key, value) in MultipleQuestion)
            {
                result += key + ". " + value+" ";
            }
            return result + $"\nRight Answer: {RightAnswer}. {MultipleQuestion[Convert.ToChar(RightAnswer)]}\n";
        }
    }
}