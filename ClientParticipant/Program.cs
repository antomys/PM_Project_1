using System;
using System.Linq;
using PollLibrary;
using PollLibrary.Polls;

namespace Participant
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            ChangeAccount();
        }

        private static void ChangeAccount()
        {
            var chosenAccount = Login();
            if (chosenAccount.Role.Equals(Roles.Participant))
            {
                ParticipantMenu(chosenAccount);
            }
            else
            {
                ManagerMenu(chosenAccount);
            }
        }

        private static Account Login()
        {
            while (true)
            {
                Console.WriteLine("Please auth to proceed:");
                Console.WriteLine("1. Sign up");
                Console.WriteLine("2. Log in");
                Console.WriteLine("3. Exit");
                Int32.TryParse(Console.ReadLine(), out var input);
                switch (input)
                {
                    case 1:
                        var signUp = SignUp();
                        if (signUp != null) return signUp;
                        continue;
                    case 2:
                        var logIn = LogIn();
                        if (logIn != null) return logIn;
                        continue;
                    case 3:
                        Environment.Exit(0);
                        break;
                    default:
                        continue;
                }

                return null;
            }
        }

        private static Account SignUp()
        {
            string name, password;
            int role;
            do
            {
                Console.Write("Please enter your Nick name: ");
                name = Console.ReadLine()?.Trim();
                Console.Write("Please enter your password: ");
                password = Console.ReadLine()?.Trim();
            } while (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(password));
            do
            {
                Console.WriteLine("Please select your role: ");
                Console.WriteLine("1. Manager");
                Console.WriteLine("2. Participant");
                Int32.TryParse(Console.ReadLine(), out role);
                Account newAccount;
                switch (role)
                {
                    case 1:
                        try
                        {
                            newAccount = new Account(Guid.NewGuid(), name, password, Roles.Manager);
                            newAccount.AddAccountToBin();
                            return newAccount;
                        }
                        catch (Exception exception)
                        {
                            Console.WriteLine(exception.Message);
                            continue;
                        }
                    case 2:
                        try
                        {
                            newAccount = new Account(Guid.NewGuid(), name, password, Roles.Participant);
                            newAccount.AddAccountToBin();
                            return newAccount;
                        }
                        catch (Exception exception)
                        {
                            Console.WriteLine(exception.Message);
                            continue;
                        }
                }
            } while (role < 1 || role > 2);

            return null;
        }
        private static Account LogIn()
        {
            string input, password;
            do
            {
                Console.Write("\nPlease enter Name: ");
                input = Console.ReadLine()?.Trim();
                Console.Write("\nPlease enter password: ");
                password = Console.ReadLine();
            } while (string.IsNullOrEmpty(input) || string.IsNullOrEmpty(password));

            try
            {
                var allAccounts = Account.GetAccounts();
                var result = allAccounts.FirstOrDefault(account =>
                    account.Name.ToLower().Equals(input) && account.Password.ToLower().Equals(password));
                if (result != null) return result;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }
            Console.WriteLine("\nPlease try again. This account is not found!\n");
            return null;
        }

        private static void ManagerMenu(IAccount account)
        {
            Console.WriteLine($"Hello,{account.Name}!");
            Console.WriteLine($"Your role is {account.Role}");
            while (true)
            {
                Console.WriteLine("1. List Polls");
                Console.WriteLine("2. Add question to poll");
                Console.WriteLine("3. Remove question from poll");
                Console.WriteLine("4. Show statistics by poll");
                Console.WriteLine("5. Change Account");
                Console.WriteLine("6. Exit");
                var polls = Poll.ListPolls();
                Int32.TryParse(Console.ReadLine(), out var selected);
                switch (selected)
                {
                    case 1:
                        Console.WriteLine("\nList: ");
                        Poll.PrintAllPolls(polls);
                        Console.WriteLine('\n');
                        break;
                    case 2:
                        Poll.PrintAllPolls(polls);
                        Console.WriteLine('\n');
                        Console.Write("\nPlease select poll : ");
                        Int32.TryParse(Console.ReadLine(), out var pollId);
                        try
                        {
                            var poll = new Poll().GetPollById(polls,pollId);
                            poll.AddQuestions();
                            break;
                        }
                        catch (Exception exception)
                        {
                            Console.WriteLine(exception.Message);
                            continue;
                        }
                    case 3:
                        Poll.PrintAllPolls(polls);
                        Console.WriteLine('\n');
                        Console.Write("\nPlease select poll : ");
                        Int32.TryParse(Console.ReadLine(), out var plId);
                        try
                        {
                            var poll = new Poll().GetPollById(polls,plId);
                            poll.DeleteQuestion();
                            break;
                        }
                        catch (Exception exception)
                        {
                            Console.WriteLine(exception.Message);
                            continue;
                        }
                    //todo:Add features
                    case 4:
                        Console.WriteLine("\nList: ");
                        Poll.PrintAllPolls(polls);
                        Console.WriteLine('\n');
                        Poll.SelectPollToStatistics(polls);
                        break;
                    case 5:
                        ChangeAccount();
                        break;
                    case 6:
                        Environment.Exit(0);
                        break;
                    default:
                        continue;
                }
            }
        }
        private static void ParticipantMenu(IAccount account)
        {
            Console.WriteLine($"Hello,{account.Name}!");
            Console.WriteLine($"Your role is {account.Role}");
            Console.WriteLine("\nMenu:");
            while (true)
            {
                Console.WriteLine("1. List all available polls");
                Console.WriteLine("2. Select poll from list");
                Console.WriteLine("3. Change Account");
                Console.WriteLine("4. Exit");
                Int32.TryParse(Console.ReadLine(), out var selected);
                var polls = Poll.ListPolls();
                while (true)
                {
                    switch (selected)
                    {
                        case 1:
                            Console.WriteLine("\nList: ");
                            Poll.PrintAllPolls(polls);
                            Console.WriteLine('\n');
                            break;
                        case 2:
                            Console.WriteLine("\nList: ");
                            Poll.PrintAllPolls(polls);
                            Console.WriteLine('\n');
                            Poll.SelectPollToTest(polls);
                            break;
                        case 3:
                            ChangeAccount();
                            break;
                        case 4:
                            Environment.Exit(0);
                            break;
                        default:
                            continue;
                    }
                }
            }
        }
    }
}