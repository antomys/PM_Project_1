using System.Collections.Generic;

namespace PollLibrary
{
    public class AccountManager
    {
        private List<Account> _accounts;

        public AccountManager(List<Account> accounts)
        {
            _accounts = accounts;
        }
    }
}