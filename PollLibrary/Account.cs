using System;
using System.Security;

namespace PollLibrary
{
    public class Account : IAccount
    {
        public Guid Guid { get; }
        public string NickName { get; }
        public SecureString Password { get; }
        public Roles Role { get; }

        public Account(Guid guid, string nickName, SecureString password, Roles role)
        {
            Guid = guid;
            NickName = nickName;
            Password = password;
            Role = role;
        } 
    }
}