using System;
using System.Linq;
using PollLibrary.AccountManaging;
using PollLibrary.Polls;

namespace Participant
{
    internal static class Program
    {
        private static void Main()
        {
            ChangeAccount();
        }

        private static void ChangeAccount()
        {
            var chosenAccount = Login();
            if (chosenAccount.Role.Equals(Role.Participant))
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
                int.TryParse(Console.ReadLine(), out var input);
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
                            newAccount = new Account(Guid.NewGuid(), name, password, Role.Manager);
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
                            newAccount = new Account(Guid.NewGuid(), name, password, Role.Participant);
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
                Console.WriteLine("2. Create New Poll");
                Console.WriteLine("3. Add question to poll");
                Console.WriteLine("4. Remove question from poll");
                Console.WriteLine("5. Show statistics by poll");
                Console.WriteLine("6. Change Account");
                Console.WriteLine("7. Exit");
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
                        Console.Write("Enter PollName: ");
                        try
                        {
                            new Poll().NewPoll(Console.ReadLine()?.Trim());
                            break;
                        }
                        catch (Exception exception)
                        {
                            Console.WriteLine(exception.Message);
                            continue;
                        }
                    case 3:
                        if(!Poll.PrintAllPolls(polls))
                            continue;
                        Console.WriteLine('\n');
                        Console.Write("\nPlease select poll : ");
                        var input = Console.ReadLine()?.Trim();
                        Int32.TryParse(input, out var pollId);
                        try
                        {
                            var poll = Poll.GetPollById(polls,pollId);
                            poll.AddQuestions();
                            break;
                        }
                        catch (Exception exception)
                        {
                            Console.WriteLine(exception.Message);
                            continue;
                        }
                    case 4:
                        if(!Poll.PrintAllPolls(polls))
                            continue;
                        Console.WriteLine('\n');
                        Console.Write("\nPlease select poll : ");
                        Int32.TryParse(Console.ReadLine(), out var plId);
                        try
                        {
                            var poll = Poll.GetPollById(polls,plId);
                            poll.DeleteQuestion();
                            break;
                        }
                        catch (Exception exception)
                        {
                            Console.WriteLine(exception.Message);
                            continue;
                        }
                    //todo:Add features
                    case 5:
                        Console.WriteLine("\nList: ");
                        if(!Poll.PrintAllPolls(polls))
                            continue;
                        Console.WriteLine('\n');
                        Poll.SelectPollToStatistics(polls);
                        break;
                    case 6:
                        ChangeAccount();
                        break;
                    case 7:
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
                Console.WriteLine("2. Select poll from list to test");
                Console.WriteLine("3. Change Account");
                Console.WriteLine("4. Exit");
                Int32.TryParse(Console.ReadLine(), out var selected);
                var polls = Poll.ListPolls();
                switch (selected)
                    {
                        case 1:
                            Console.WriteLine("\nList: ");
                            if(!Poll.PrintAllPolls(polls))
                                continue;
                            Console.WriteLine('\n');
                            break;
                        case 2:
                            Console.WriteLine("\nList: ");
                            if(!Poll.PrintAllPolls(polls))
                                continue;
                            Console.WriteLine('\n');
                            try
                            {
                                Poll.SelectPollToTest(polls);
                                break;
                            }
                            catch (Exception exception)
                            {
                                Console.WriteLine(exception.Message);
                                continue;
                            }
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