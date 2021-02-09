using System;

namespace PollLibrary.AccountManaging
{
    public enum Role
    {
        Manager,
        Participant
    }

    public interface IAccount
    {
        Guid Id { get;  }
        string Name { get; }
        string Password { get; }
        Role Role { get; }
        
        
    }
}