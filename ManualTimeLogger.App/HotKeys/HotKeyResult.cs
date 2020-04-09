namespace ManualTimeLogger.App.HotKeys
{
    public class HotKeyResult
    {
        public IHotKeyState NewHotKeyState { get; }
        public string TextResult { get; }

        public HotKeyResult(IHotKeyState newHotKeyState, string textResult)
        {
            NewHotKeyState = newHotKeyState;
            TextResult = textResult;
        }
    }
}
