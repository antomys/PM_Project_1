using System;
using System.Collections.Generic;
using PollLibrary;
using PollLibrary.Polls;

namespace ClientParticipant
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            var poll = new Poll();
            //poll.NewPoll("test");
            poll.GetPoll("test"); 
            poll.ShowStatistics();
            //poll.AddQuestions();
        }
    }
}