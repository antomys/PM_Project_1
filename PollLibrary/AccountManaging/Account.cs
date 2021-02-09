using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using PollLibrary.Exceptions;

namespace PollLibrary.AccountManaging
{
    
    public class Account : IAccount
    {
        private const string AccountBin = "Accounts.bin";
        
        public Account(Guid id, string name, string password, Role role)
        {
            Id = id;
            if (string.IsNullOrEmpty(name.Trim()))
                throw new InvalidNameException(Name);
            Name = name.Trim();
            Password = password;
            Role = role;
        }
        [JsonPropertyName("id")]
        public Guid Id { get; }
        [JsonPropertyName("name")]
        public string Name { get; }
        [JsonPropertyName("password")]
        public string Password { get;}
        [JsonPropertyName("role")]
        public Role Role { get; }

        public void AddAccountToBin()
        {
            var option = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            CreateOrCheckFile();
            if (File.ReadAllText(AccountBin).Length == 0)
            {
                var list = new List<IAccount> {this};

                if (!CheckAccountExists(list))
                {
                    throw new AlreadyExistsException(ToString());
                }
                
                var json = JsonSerializer.Serialize(list,option);
                File.WriteAllText(AccountBin, json);
            }
            else
            {
                var data = JsonSerializer.Deserialize<List<Account>>(File.ReadAllText(AccountBin));
                
                //if (data!.Any(x=>x.Name.Equals(Name) && x.Password.Equals(Password)))
                //{
                //    throw new AlreadyExistsException(ToString());
                //}

                if (CheckAccountExists(data))
                {
                    throw new AlreadyExistsException(ToString());
                }
                
                data?.Add(this);
                var json = JsonSerializer.Serialize(data,option);
                File.WriteAllText(AccountBin,json);
            }
        }

        public void DeleteAccountById(Guid guid)
        {
            CreateOrCheckFile();
            var option = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            var accounts = JsonSerializer.Deserialize<List<Account>>(File.ReadAllText(AccountBin));
            accounts?.RemoveAt(accounts.FindIndex(x => x.Id.Equals(guid)));
            var json = JsonSerializer.Serialize(accounts, option);
            File.WriteAllText(AccountBin,json);

        }
        private static void CreateOrCheckFile()
        {
            if (File.Exists(AccountBin)) return;
            //Console.WriteLine("File Account.bin not found. Creating...");
            using var fs = File.Create(AccountBin);
            fs.Close();
        }

        private bool CheckAccountExists(IEnumerable<IAccount> accounts)
        {
            return accounts.All(account => account.Name.ToLower() != Name.ToLower());
        }

        public static IEnumerable<Account> GetAccounts()
        {
            if (!File.Exists(AccountBin) || File.ReadAllText(AccountBin).Length==0) 
            {
                throw new NotFoundException(AccountBin);
            }
            var deserialized = JsonSerializer.Deserialize<List<Account>>(File.ReadAllText(AccountBin));
            var accounts = deserialized!.ToList();
            return accounts;
        }

        public override string ToString()
        {
            return $"ID: {Id}\n" +
                   $"Name: {Name}\n" +
                   $"Role: {Role}";
        }
    }
}