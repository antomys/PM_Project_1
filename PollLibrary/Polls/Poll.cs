using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Microsoft.VisualBasic;
using PollLibrary.Exceptions;

namespace PollLibrary.Polls
{
    public class Poll : IPoll
    {
        private static readonly string WorkingDir = Directory.GetCurrentDirectory() + "/polls/";

        public string PollName { get; set; }
        public int QuestionAmount { get; set; }
        public List<Question> Questions { get; set; }
        public Statistics Statistics { get; set; }
        
        public void AddQuestions()
        {
            Console.Write("Enter Question: ");
            var name = Console.ReadLine();
            var dictionaryOfAnswers = new Dictionary<char, string>();
            Console.Write ("How many variants of answers? (if one - just write 1): ");
            Int32.TryParse(Console.ReadLine(), out var amount);
            if (amount == 1)
            {
                dictionaryOfAnswers = null;
            }
            else
            {
                var key = '1';
                Console.WriteLine("Enter each variant of answer like: answer. Numbers will be automatically added");
                            for (var i = 0; i < amount; i++)
                            {
                                Console.Write($"{key}. ");
                                var value = Console.ReadLine()?.Trim();
                                dictionaryOfAnswers.Add(key,value);
                                key++;
                            }
            }

            string rightAnswer;
            char answer;
            if (dictionaryOfAnswers != null)
            {
                do
                {
                    Console.Write("Please number of right answer: ");
                    answer = Convert.ToChar(Console.ReadLine().Trim());
                    rightAnswer = answer.ToString();
                } while (!dictionaryOfAnswers.Keys.Any(x => x.Equals(answer)));
                
            }
            else
            {
                Console.Write("Please enter right answer: ");
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
            File.WriteAllText(WorkingDir+PollName+".bin",serialize);
        }

        /*public void ShowStatistics()
        {
            var deserializeFile = JsonSerializer.Deserialize<Poll>(File.ReadAllText(_workingDir + _fileName)); //todo:remove
            Console.WriteLine(deserializeFile.Statistics.ToString());
        }*/
        private static void ShowStatistics(IPoll poll)
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
            File.WriteAllText(WorkingDir+poll.PollName+".bin",serialize);
        }

        public void NewPoll(string fileName)
        {
            Questions = new List<Question>();
            Statistics = new Statistics();
            //_fileName = fileName + ".bin";
            PollName = fileName;
            if (!Directory.Exists(WorkingDir))
            {
                Directory.CreateDirectory(WorkingDir);
                using var fs = File.Create(WorkingDir+PollName+".bin");
                fs.Close();
            }
            if(File.Exists(WorkingDir+PollName+".bin"))
                return;
            var serialize = JsonSerializer.Serialize(this);
            File.WriteAllText(WorkingDir+PollName+".bin",serialize);
        }

        public Poll GetPollById(Dictionary<int,Poll> polls, int input)
        {
            if (input > polls.Values.Count || input < polls.Values.Count || polls==null)
            {
                throw new PollNotFoundException(input.ToString());
            }
            return polls[input];
        }

        public void TestPoll()
        {
            //var deserialize = JsonSerializer.Deserialize<Poll>(File.ReadAllText(_workingDir + _fileName));
            var questionNumber = 1;
            var rightAnswers = 0;
            foreach (var question in Questions)
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
            Statistics.Tries ++;
            if (Statistics.AverageRight == 0)
            {
                Statistics.AverageRight = rightAnswers / questionNumber * 100;
            }
            else
            {
                var thisAverage = rightAnswers / questionNumber * 100;
                Statistics.AverageRight =
                    (Statistics.AverageRight + thisAverage) / 2;
            }
            
            var serialize = JsonSerializer.Serialize(this);
            File.WriteAllText(WorkingDir+PollName+".bin",serialize);
            Console.WriteLine($"Your answered on {rightAnswers} questions right.\nThis test was passed {Statistics.Tries} times\nAverage right answers:{Statistics.AverageRight}%");
        }
        public static Dictionary<int,Poll> ListPolls()
        {
            var pollList = new Dictionary<int, Poll>();
            foreach (var pollFile in Directory.GetFiles(WorkingDir))
            {
                if (File.ReadAllText(pollFile).Length == 0)
                {
                    return null;
                }
                var json = JsonSerializer.Deserialize<Poll>(File.ReadAllText(pollFile));
                pollList.Add(pollList.Count+1,json);
            }
            return pollList;
        }

        public static bool PrintAllPolls(Dictionary<int, Poll> polls)
        {
            if (polls == null || polls.Count==0)
            {
                Console.WriteLine("\tNo polls found!");
                return false;
            }
            else
            {
                foreach (var (key, value) in polls)
                {
                    Console.WriteLine($"\t{key}. {value.PollName}");
                }

                return true;
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
            polls[input].TestPoll();


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
            ShowStatistics(polls[input]);
        }
    }
}