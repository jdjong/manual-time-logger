using System;
using System.Windows.Forms;

namespace ManualTimeLogger.App.HotKeys
{
    public class HoursHotKeyState : IHotKeyState
    {
        public HotKeyResult GetHotKeyResult(string currentTextBoxText, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.D0 || e.KeyCode == Keys.NumPad0)
            {
                return new HotKeyResult(new HourPartHotKeyState(), "*0");
            }
            if (e.KeyCode == Keys.D1 || e.KeyCode == Keys.NumPad1)
            {
                return new HotKeyResult(new HourPartHotKeyState(), "*1");
            }
            if (e.KeyCode == Keys.D2 || e.KeyCode == Keys.NumPad2)
            {
                return new HotKeyResult(new HourPartHotKeyState(), "*2");
            }
            if (e.KeyCode == Keys.D3 || e.KeyCode == Keys.NumPad3)
            {
                return new HotKeyResult(new HourPartHotKeyState(), "*3");
            }
            if (e.KeyCode == Keys.D4 || e.KeyCode == Keys.NumPad4)
            {
                return new HotKeyResult(new HourPartHotKeyState(), "*4");
            }
            if (e.KeyCode == Keys.D5 || e.KeyCode == Keys.NumPad5)
            {
                return new HotKeyResult(new HourPartHotKeyState(), "*5");
            }
            if (e.KeyCode == Keys.D6 || e.KeyCode == Keys.NumPad6)
            {
                return new HotKeyResult(new HourPartHotKeyState(), "*6");
            }
            if (e.KeyCode == Keys.D7 || e.KeyCode == Keys.NumPad7)
            {
                return new HotKeyResult(new HourPartHotKeyState(), "*7");
            }
            if (e.KeyCode == Keys.D8 || e.KeyCode == Keys.NumPad8)
            {
                return new HotKeyResult(new HourPartHotKeyState(), "*8");
            }
            if (e.KeyCode == Keys.D9 || e.KeyCode == Keys.NumPad9)
            {
                return new HotKeyResult(new HourPartHotKeyState(), "*9");
            }

            return new HotKeyResult(new NoHotKeyAvailableState(), null);
        }
    }
}
