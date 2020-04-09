using System.Windows.Forms;

namespace ManualTimeLogger.App.HotKeys
{
    public class NoHotKeyAvailableState : IHotKeyState
    {
        public HotKeyResult GetHotKeyResult(string currentTextBoxText, KeyEventArgs e)
        {
            return new HotKeyResult(this, null);
        }
    }
}
