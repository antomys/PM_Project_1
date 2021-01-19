using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using PollLibrary.Exceptions;

namespace PollLibrary.Polls
{
    public class Poll : IPoll
    {
        private static readonly string _workingDir = Directory.GetCurrentDirectory() + "/polls/";

        public string PollName { get; set; }
        public int QuestionAmount { get; set; }
        public List<Question> Questions { get; set; }
        public Statistics Statistics { get; set; }
        
        public void AddQuestions(Poll poll)
        {
            Console.WriteLine("Enter Question:");
            var name = Console.ReadLine();
            var dictionaryOfAnswers = new Dictionary<char, string>();
            Console.WriteLine("How many variants of answers? (if one - just write 1");
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
                                var key = Convert.ToChar(input[0]); //todo:refactoring
                                var value = input.Substring(2, input.Length - 2).Trim();
                                dictionaryOfAnswers.Add(key,value);
                            }
            }
            Console.WriteLine("Please enter right answer: ");
            var rightAnswer = Console.ReadLine();
            UpdateBin(new Question(name, dictionaryOfAnswers, rightAnswer), poll);
        }

        public void DeleteQuestion(int questionId, Poll poll)
        {
            var deserializeFile = JsonSerializer.Deserialize<Poll>(File.ReadAllText(_workingDir + poll.PollName+".bin"));
            if (deserializeFile?.Questions != null)
            {
                var toDelete = deserializeFile.Questions[questionId];
                deserializeFile.Questions.Remove(toDelete);
                deserializeFile.QuestionAmount = deserializeFile.Questions.Count;
            }

            var serialize = JsonSerializer.Serialize(deserializeFile);
            File.WriteAllText(_workingDir+poll.PollName+".bin",serialize);
        }

        /*public void ShowStatistics()
        {
            var deserializeFile = JsonSerializer.Deserialize<Poll>(File.ReadAllText(_workingDir + _fileName)); //todo:remove
            Console.WriteLine(deserializeFile.Statistics.ToString());
        }*/
        public void ShowStatistics(Poll poll)
        {
            //var deserializeFile = JsonSerializer.Deserialize<Poll>(File.ReadAllText(_workingDir + _fileName));
            Console.WriteLine(poll.Statistics.ToString());
        }

        private void UpdateBin(Question question, IPoll poll)
        {
            //var deserializeFile = JsonSerializer.Deserialize<Poll>(File.ReadAllText(_workingDir + _fileName));
            if (poll.Questions == null)
            {
                Questions = new List<Question> {question};
            }
            else
            {
                Questions = poll.Questions;
                Questions.Add(question);
            }
            QuestionAmount = Questions.Count;
            var serialize = JsonSerializer.Serialize(this);
            File.WriteAllText(_workingDir+poll.PollName+".bin",serialize);
        }

        public void NewPoll(string fileName)
        {
            Questions = new List<Question>();
            Statistics = new Statistics();
            //_fileName = fileName + ".bin";
            PollName = fileName;
            if (!Directory.Exists(_workingDir))
            {
                Directory.CreateDirectory(_workingDir);
                using var fs = File.Create(_workingDir+PollName+".bin");
                fs.Close();
            }
            if(File.Exists(_workingDir+PollName+".bin"))
                return;
            var serialize = JsonSerializer.Serialize(this);
            File.WriteAllText(_workingDir+PollName+".bin",serialize);
        }

        public void GetPoll(string fileName)
        {
            fileName += ".bin";
            if (!File.Exists(_workingDir+fileName))
            {
                throw new NotFoundException(fileName);
            }
            //Not used!
        }

        public void TestPoll(Poll poll)
        {
            //var deserialize = JsonSerializer.Deserialize<Poll>(File.ReadAllText(_workingDir + _fileName));
            var questionNumber = 1;
            var rightAnswers = 0;
            foreach (var question in poll.Questions)
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
                if (input != null && input.Equals(question.RightAnswer))
                {
                    rightAnswers++;
                    Console.WriteLine($"Answer {input} is right!\n");
                }
                else
                {
                    Console.WriteLine($"Right answer is {question.RightAnswer}\n");
                }
            }
            poll.Statistics.Tries ++;
            poll.Statistics.AverageRight =
                (poll.Statistics.AverageRight + rightAnswers) / poll.Statistics.Tries;
            var serialize = JsonSerializer.Serialize(poll);
            File.WriteAllText(_workingDir+poll.PollName+".bin",serialize);
            Console.WriteLine($"Your answered on {rightAnswers} questions right.\nThis test was passed {poll.Statistics.Tries} times\nAverage right answers:{poll.Statistics.AverageRight}");
        }
        public static Dictionary<int,Poll> ListPolls()
        {
            var pollList = new Dictionary<int, Poll>();
            foreach (var pollFile in Directory.GetFiles(_workingDir))
            {
                var json = JsonSerializer.Deserialize<Poll>(File.ReadAllText(pollFile));
                pollList.Add(pollList.Count+1,json);
            }
            return pollList;
        }

        public static void PrintAllPolls(Dictionary<int, Poll> polls)
        {
            foreach (var (key, value) in polls)
            {
                Console.WriteLine($"{key}. {value.PollName}");
            }
        }
    }
}