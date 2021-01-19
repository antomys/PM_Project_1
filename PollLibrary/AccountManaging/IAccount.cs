using System;

namespace PollLibrary
{
    public enum Roles
    {
        Manager,
        Participant
    }

    public interface IAccount
    {
        Guid Id { get;  }
        string Name { get; }
        string Password { get; }
        Roles Role { get; }
        
        
    }
}