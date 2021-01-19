using System;
using System.Linq;
using PollLibrary;
using PollLibrary.Polls;

namespace ClientParticipant
{
    internal static class Program
    {
        private static void Main(string[] args)
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
            int role = 0;
            do
            {
                Console.Write("Please enter your Nick name: ");
                name = Console.ReadLine().Trim();
                Console.Write("Please enter your password: ");
                password = Console.ReadLine().Trim();
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
                input = Console.ReadLine().Trim();
                Console.Write("\nPlease enter password: ");
                password = Console.ReadLine();
            } while (string.IsNullOrEmpty(input) || string.IsNullOrEmpty(password));
            var allAccounts = Account.GetAccounts();
            var result = allAccounts.FirstOrDefault(account =>
                account.Name.ToLower().Equals(input) && account.Password.ToLower().Equals(password));
            if (result != null) return result;
            Console.WriteLine("\nPlease try again. This account is not found!\n");
            return null;
        }

        private static void ManagerMenu(IAccount account)
        {
            Console.WriteLine($"Hello,{account.Name}!");
            Console.WriteLine($"Your role is {account.Role}");
        }

        private static void ParticipantMenu(IAccount account)
        {
            Console.WriteLine($"Hello,{account.Name}!");
            Console.WriteLine($"Your role is {account.Role}");
            Console.WriteLine("Menu:");
            while (true)
            {
                
            }
        }
    }
}