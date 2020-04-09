using System.Windows.Forms;

namespace ManualTimeLogger.App.HotKeys
{
    public class HourPartHotKeyState : IHotKeyState
    {
        public HotKeyResult GetHotKeyResult(string currentTextBoxText, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.D0 || e.KeyCode == Keys.NumPad0)
            {
                return new HotKeyResult(new NoHotKeyAvailableState(), ".0$");
            }
            if (e.KeyCode == Keys.D1 || e.KeyCode == Keys.NumPad1)
            {
                return new HotKeyResult(new NoHotKeyAvailableState(), ".25$");
            }
            if (e.KeyCode == Keys.D2 || e.KeyCode == Keys.NumPad2)
            {
                return new HotKeyResult(new NoHotKeyAvailableState(), ".5$");
            }
            if (e.KeyCode == Keys.D3 || e.KeyCode == Keys.NumPad3)
            {
                return new HotKeyResult(new NoHotKeyAvailableState(), ".75$");
            }

            return new HotKeyResult(new NoHotKeyAvailableState(), null);
        }
    }
}
