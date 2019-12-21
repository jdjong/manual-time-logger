namespace ManualTimeLogger.Domain
{
    public interface IInputPartSelector
    {
        InputPartSelectorResult Get(string input);
    }
}
