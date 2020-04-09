using System.Collections.Generic;
using System.Windows.Forms;

namespace ManualTimeLogger.App
{
    public class HotKeyManager
    {
        private readonly List<string> _accounts;

        public HotKeyManager(List<string> accounts)
        {
            _accounts = accounts;
        }

        /// <summary>
        /// If text box is empty, then 1 to 5 can be used to insert account quickly.
        /// The hot key is automatically bound to the order of the account presets in the app.config.
        /// </summary>
        /// <param name="currentTextBoxText"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        public string GetHotKeyResult(string currentTextBoxText, KeyEventArgs e)
        {
            if (string.IsNullOrEmpty(currentTextBoxText) && !e.Shift && (e.KeyCode == Keys.D1 || e.KeyCode == Keys.NumPad1) && _accounts.Count >= 1)
            {
                return _accounts[0];
            }
            if (string.IsNullOrEmpty(currentTextBoxText) && !e.Shift && (e.KeyCode == Keys.D2 || e.KeyCode == Keys.NumPad2) && _accounts.Count >= 2)
            {
                return _accounts[1];
            }
            if (string.IsNullOrEmpty(currentTextBoxText) && !e.Shift && (e.KeyCode == Keys.D3 || e.KeyCode == Keys.NumPad3) && _accounts.Count >= 3)
            {
                return _accounts[2];
            }
            if (string.IsNullOrEmpty(currentTextBoxText) && !e.Shift && (e.KeyCode == Keys.D4 || e.KeyCode == Keys.NumPad4) && _accounts.Count >= 4)
            {
                return _accounts[3];
            }
            if (string.IsNullOrEmpty(currentTextBoxText) && !e.Shift && (e.KeyCode == Keys.D5 || e.KeyCode == Keys.NumPad5) && _accounts.Count >= 5)
            {
                return _accounts[4];
            }

            return null;
        }
    }
}