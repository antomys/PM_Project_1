using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.WebSockets;
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
        
        public void AddQuestions()
        {
            Console.WriteLine("Enter Question:");
            var name = Console.ReadLine();
            var dictionaryOfAnswers = new Dictionary<char, string>();
            Console.WriteLine("How many variants of answers? (if one - just write 1)");
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

            string rightAnswer;
            char answer;
            if (dictionaryOfAnswers != null)
            {
                do
                {
                    Console.WriteLine("Please enter right answer: ");
                    answer = Convert.ToChar(Console.ReadLine().Trim());
                    rightAnswer = answer.ToString();
                } while (!dictionaryOfAnswers.Keys.Any(x => x.Equals(answer)));
            }
            else
            {
                Console.WriteLine("Please enter right answer: ");
                rightAnswer = Console.ReadLine();
            }
            UpdateBin(new Question(name, dictionaryOfAnswers, rightAnswer), this);
            
        }

        public void DeleteQuestion()
        {
            Console.WriteLine('\n');
            for (var i = 0; i < Questions.Count; i++)
            {
                Console.WriteLine($"{i+1}. {Questions[i]}");
            }
            int questionId;
            do
            {
                Console.Write("Please enter Question index to delete: ");
                Int32.TryParse(Console.ReadLine(), out questionId);
                questionId -= 1;
            } while (questionId < 0 || questionId > Questions.Count);
            if (Questions != null)
            {
                var toDelete = Questions[questionId];
                Questions.Remove(toDelete);
                QuestionAmount = Questions.Count;
            }

            var serialize = JsonSerializer.Serialize(this);
            Console.WriteLine("Success!\n");
            File.WriteAllText(_workingDir+PollName+".bin",serialize);
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

        public Poll GetPollById(Dictionary<int,Poll> polls, int input)
        {
            if (input > polls.Values.Count || input < polls.Values.Count)
            {
                throw new PollNotFoundException(input.ToString());
            }
            return polls[input];
            Console.WriteLine('\n');
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
            if (polls == null || polls.Count==0)
            {
                Console.WriteLine("No polls found!");
            }
            else
            {
                foreach (var (key, value) in polls)
                {
                    Console.WriteLine($"{key}. {value.PollName}");
                } 
            }
        }
        public static void SelectPollToTest(Dictionary<int,Poll> polls)
        {
            if (polls == null || polls.Count==0) return;
            int input;
            do
            {            
                Console.Write("Please select poll to test: ");
                Int32.TryParse(Console.ReadLine(), out input);
            } while (input > polls.Values.Count || input < polls.Values.Count);
            Console.WriteLine('\n');
            new Poll().TestPoll(polls[input]);


        }
        public static void SelectPollToStatistics(Dictionary<int, Poll> polls)
        {
            if (polls == null || polls.Count==0) return;
            int input;
            do
            {            
                Console.Write("Please select poll to see stats: ");
                Int32.TryParse(Console.ReadLine(), out input);
            } while (input > polls.Values.Count || input < polls.Values.Count);

            Console.WriteLine('\n');
            new Poll().ShowStatistics(polls[input]);
        }
    }
}