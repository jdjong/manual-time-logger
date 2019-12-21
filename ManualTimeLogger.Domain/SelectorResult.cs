using System;

namespace ManualTimeLogger.Domain
{
    public class InputPartSelectorResult
    {
        public bool IsSuccess { get; }
        public string InputPart { get; }
        public string FailedMessage { get; }

        private InputPartSelectorResult(bool isSuccess, string inputPart, string failedMessage)
        {
            if (!isSuccess && string.IsNullOrEmpty(failedMessage))
            {
                throw new ArgumentException("Parsing is not a success, so provide a reason why", nameof(failedMessage));
            }
            
            IsSuccess = isSuccess;
            InputPart = inputPart;
            FailedMessage = failedMessage;
        }

        public static InputPartSelectorResult Success(string result)
        {
            return new InputPartSelectorResult(true, result, null);
        }

        public static InputPartSelectorResult Failed(string failedMessage)
        {
            return  new InputPartSelectorResult(false, null, failedMessage);
        }
    }
}
