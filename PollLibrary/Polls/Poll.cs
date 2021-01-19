using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using PollLibrary.Exceptions;

namespace PollLibrary.Polls
{
    public class Poll : IPoll
    {
        private readonly string _workingDir = Directory.GetCurrentDirectory() + "/polls/";
        private string _fileName;
        public int QuestionAmount { get; set; }
        public List<Question> Questions { get; set; }
        public Statistics Statistics { get; set; }
        
        public void AddQuestions()
        {
            Console.WriteLine("Enter Question:");
            var name = Console.ReadLine();
            var dictionaryOfAnswers = new Dictionary<char, string>();
            Console.WriteLine("How many variants of answers?");
            Int32.TryParse(Console.ReadLine(), out var amount);
            if (amount == 1)
            {
                dictionaryOfAnswers = null;
            }
            else
            {
                Console.WriteLine("Enter each variant of answer like: {1. answer} ");
                            for (var i = 0; i < amount; i++)
                            {
                                var input = Console.ReadLine();
                                var key = Convert.ToChar(input[0]);
                                var value = input.Substring(2, input.Length - 2).Trim();
                                dictionaryOfAnswers.Add(key,value);
                            }
            }
            Console.WriteLine("Please enter right answer: ");
            var rightAnswer = Console.ReadLine();
            UpdateBin(new Question(name, dictionaryOfAnswers, rightAnswer));
        }

        public void DeleteQuestion(int questionId)
        {
            var deserializeFile = JsonSerializer.Deserialize<Poll>(File.ReadAllText(_workingDir + _fileName));
            if (deserializeFile.Questions != null)
            {
                var toDelete = deserializeFile.Questions[questionId];
                deserializeFile.Questions.Remove(toDelete);
                deserializeFile.QuestionAmount = deserializeFile.Questions.Count;
            }

            var serialize = JsonSerializer.Serialize(deserializeFile);
            File.WriteAllText(_workingDir+_fileName,serialize);
        }

        public void ShowStatistics()
        {
            var deserializeFile = JsonSerializer.Deserialize<Poll>(File.ReadAllText(_workingDir + _fileName));
            Console.WriteLine(deserializeFile.Statistics.ToString());
        }

        private void UpdateBin(Question question)
        {
            var deserializeFile = JsonSerializer.Deserialize<Poll>(File.ReadAllText(_workingDir + _fileName));
            if (deserializeFile.Questions == null)
            {
                Questions = new List<Question> {question};
            }
            else
            {
                Questions = deserializeFile.Questions;
                Questions.Add(question);
            }
            QuestionAmount = Questions.Count;
            var serialize = JsonSerializer.Serialize(this);
            File.WriteAllText(_workingDir+_fileName,serialize);
        }

        public void NewPoll(string fileName)
        {
            Questions = new List<Question>();
            Statistics = new Statistics();
            _fileName = fileName + ".bin";
            if (!Directory.Exists(_workingDir))
            {
                Directory.CreateDirectory(_workingDir);
                using var fs = File.Create(_workingDir+_fileName);
                fs.Close();
            }
            if(File.Exists(_workingDir+_fileName))
                return;
            var serialize = JsonSerializer.Serialize(this);
            File.WriteAllText(_workingDir+_fileName,serialize);
        }

        public void GetPoll(string fileName)
        {
            _fileName = fileName + ".bin";
            if (!File.Exists(_workingDir+_fileName))
            {
                throw new NotFoundException(_fileName);
            }
            //
        }

        public void TestPoll()
        {
            var deserialize = JsonSerializer.Deserialize<Poll>(File.ReadAllText(_workingDir + _fileName));
            var questionNumber = 1;
            var rightAnswers = 0;
            foreach (var question in deserialize.Questions)
            {
                Console.WriteLine(questionNumber + ". " + question.Name);
                if (question.MultipleQuestion != null)
                {
                    foreach (var (key, value) in question.MultipleQuestion)
                    {
                        Console.WriteLine("\t"+key+". " + value);
                    }
                }
                questionNumber++;
                Console.Write("Your Answer: ");
                var input = Console.ReadLine()?.Trim();
                if (input.Equals(question.RightAnswer))
                {
                    rightAnswers++;
                    Console.WriteLine($"Answer {input} is right!\n");
                }
                else
                {
                    Console.WriteLine($"Right answer is {question.RightAnswer}\n");
                }
            }
            deserialize.Statistics.Tries ++;
            deserialize.Statistics.AverageRight =
                (deserialize.Statistics.AverageRight + rightAnswers) / deserialize.Statistics.Tries;
            var serialize = JsonSerializer.Serialize(deserialize);
            File.WriteAllText(_workingDir+_fileName,serialize);
            Console.WriteLine($"Your answered on {rightAnswers} questions right.\nThis test was passed {deserialize.Statistics.Tries} times\nAverage right answers:{deserialize.Statistics.AverageRight}");
        }
    }
}