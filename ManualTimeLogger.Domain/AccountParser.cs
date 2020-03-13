using System.Collections.Generic;

namespace ManualTimeLogger.Domain
{
    public class AccountParser
    {
        private readonly List<string> _validAccounts;

        public AccountParser(List<string> validAccounts)
        {
            _validAccounts = validAccounts;
        }

        public ParseResult<string> Parse(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return new ParseResult<string>(false, null);
            }

            bool isSuccess = false;

            string account = null;

            foreach (var validAccount in _validAccounts)
            {
                isSuccess = input.StartsWith(validAccount);
                if (isSuccess)
                {
                    account = validAccount;

                    break;
                }
            }
            return new ParseResult<string>(isSuccess, account);
        }
    }
}
