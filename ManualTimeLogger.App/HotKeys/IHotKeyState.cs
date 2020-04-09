using System.Windows.Forms;

namespace ManualTimeLogger.App.HotKeys
{
    public interface IHotKeyState
    {
        HotKeyResult GetHotKeyResult(string currentTextBoxText, KeyEventArgs e);
    }
}