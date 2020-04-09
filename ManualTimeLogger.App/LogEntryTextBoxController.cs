using System.Drawing;
using System.Windows.Forms;
using ManualTimeLogger.App.AutoFill;
using ManualTimeLogger.App.HotKeys;
using ManualTimeLogger.Domain;

namespace ManualTimeLogger.App
{
    public class LogEntryTextBoxController : IAutoFillHandler
    {
        private readonly LogEntryInputParser _inputParser;
        private readonly IAutoFillListBoxController _autoFillListBoxController;
        private readonly IHotKeyState _initialHotKeyState;
        private IHotKeyState _currentHotKeyState;
        private ITimeLoggedHandler _timeLoggedHandler;
        private TextBox _logEntryTextBox;

        public LogEntryTextBoxController(LogEntryInputParser inputParser, IAutoFillListBoxController autoFillListBoxController, IHotKeyState initialHotKeyState)
        {
            _inputParser = inputParser;
            _autoFillListBoxController = autoFillListBoxController;
            _initialHotKeyState = initialHotKeyState;
            _currentHotKeyState = initialHotKeyState;
        }

        public void Init(ITimeLoggedHandler timeLoggedHandler, TextBox logEntryTextBox, ListBox autoFillListBox)
        {
            _timeLoggedHandler = timeLoggedHandler;
            _logEntryTextBox = logEntryTextBox;
            _autoFillListBoxController.Init(this, autoFillListBox);

            _logEntryTextBox.KeyDown += TextBoxKeyDown;
            _logEntryTextBox.KeyUp += TextBoxKeyUp;
        }

        private void TextBoxKeyDown(object sender, KeyEventArgs e)
        {
            var hotKeyResult = _currentHotKeyState.GetHotKeyResult(_logEntryTextBox.Text, e);
            _currentHotKeyState = hotKeyResult.NewHotKeyState;
            if (!string.IsNullOrEmpty(hotKeyResult.TextResult))
            {
                _logEntryTextBox.Text += hotKeyResult.TextResult;
                MoveTextBoxCursorToEndOfText();
                e.Handled = e.SuppressKeyPress = true;
                return;
            }

            // Key @ is pressed. REMINDER! Duplicate knowledge, because @ is also hardcoded in LogEntryInputParser as special char for label.
            // An alternative is unknown to me how to make the below key configuration configurable.
            if (e.Shift && e.KeyCode == Keys.D2)
            {
                _autoFillListBoxController.DoAutoFillLabels();
                DetermineTextColor();
                return;
            }

            // Key ! is pressed. REMINDER! Duplicate knowledge, because ! is also hardcoded in LogEntryInputParser as special char for activity.
            // An alternative is unknown to me how to make the below key configuration configurable.
            if (e.Shift && e.KeyCode == Keys.D1)
            {
                _autoFillListBoxController.DoAutoFillActivities();
                DetermineTextColor();
                return;
            }

            if (e.KeyCode != Keys.Enter) return;

            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Tab)
            {
                e.Handled = e.SuppressKeyPress = true;
            }
        }

        private void TextBoxKeyUp(object sender, KeyEventArgs e)
        {
            DetermineTextColor();
            if (string.IsNullOrEmpty(_logEntryTextBox.Text))
            {
                ResetHotKeyState();
            }

            if (e.KeyCode != Keys.Enter) return;

            var input = _logEntryTextBox.Text;
            if (_inputParser.TryParse(input, out var logEntry))
            {
                _timeLoggedHandler.HandleTimeLogged(logEntry);

                ClearTextBox();
                ResetHotKeyState();
            }
        }

        private void ClearTextBox()
        {
            _logEntryTextBox.Text = string.Empty;
            _logEntryTextBox.ForeColor = Color.Red;
        }

        private void ResetHotKeyState()
        {
            _currentHotKeyState = _initialHotKeyState;
        }

        // Cursor to end of the text in the text box
        public void HandleAutoFillFinished(string autoFilledText = null)
        {
            if (!string.IsNullOrEmpty(autoFilledText))
            {
                _logEntryTextBox.Text += autoFilledText;
            }

            MoveTextBoxCursorToEndOfText();
            DetermineTextColor();
            _logEntryTextBox.Focus();
        }

        private void MoveTextBoxCursorToEndOfText()
        {
            _logEntryTextBox.SelectionStart = _logEntryTextBox.Text.Length;
            _logEntryTextBox.SelectionLength = 0;
        }

        private void DetermineTextColor()
        {
            var input = _logEntryTextBox.Text;
            _logEntryTextBox.ForeColor = _inputParser.TryParse(input, out _)
                ? Color.Green
                : Color.Red;
        }
    }
}