namespace ManualTimeLogger.App.AutoFill
{
    public interface IAutoFillHandler
    {
        void HandleAutoFillFinished(string autoFilledText = null);
    }
}