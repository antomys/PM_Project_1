using System;
using System.Security;
using System.Text.Json.Serialization;

namespace PollLibrary
{
    public enum Roles
    {
        Manager,
        Participant
    }

    internal interface IAccount
    { 
        [JsonPropertyName("id")]
        Guid Guid { get; }
        [JsonPropertyName("nickname")]
        string NickName { get; }
        [JsonPropertyName("password")]
        SecureString Password { get; }
        [JsonPropertyName("role")]
        Roles Role { get; }
        
        
    }
}