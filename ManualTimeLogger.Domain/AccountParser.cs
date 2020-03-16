using System;
using System.Collections.Generic;
using System.Linq;

namespace ManualTimeLogger.Domain
{
    public class AccountParser
    {
        private readonly List<string> _validAccounts;
        private readonly char[] _allSectionMarkers;

        public AccountParser(List<string> validAccounts, string allSectionMarkers)
        {
            _validAccounts = validAccounts;
            if (string.IsNullOrEmpty(allSectionMarkers)) throw new ArgumentNullException(nameof(allSectionMarkers));

            _allSectionMarkers = (allSectionMarkers + " ").ToCharArray();
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
                string inputMinusAccount = null;
                if (input.Length > validAccount.Length)
                {
                    inputMinusAccount = input.Substring(validAccount.Length); 
                }

                isSuccess = !string.IsNullOrEmpty(inputMinusAccount) &&
                            input.StartsWith(validAccount) &&
                            _allSectionMarkers.Contains(inputMinusAccount.ToCharArray()[0]);

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
