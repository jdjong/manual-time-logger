using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using ManualTimeLogger.Domain;
using ManualTimeLogger.Persistence;

namespace ManualTimeLogger.App
{
    public partial class LogEntryInputForm : Form, IHandleAutoFill
    {
        private readonly LogEntryInputParser _inputParser;
        private readonly IRepository _repository;
        private readonly AutoFillListBoxController _autoFillListBoxController;

        public LogEntryInputForm(LogEntryInputParser inputParser, IRepository repository, AutoFillListBoxController autoFillListBoxController)
        {
            _inputParser = inputParser;
            _repository = repository;
            
            InitializeComponent();

            _autoFillListBoxController = autoFillListBoxController;
            _autoFillListBoxController.Init(this, autoFillListBox);

            // Working area is area without the task-bar. Correct layout is guaranteed for bottom task-bars only.
            Location = new Point(Screen.PrimaryScreen.WorkingArea.Width - ClientSize.Width, Screen.PrimaryScreen.WorkingArea.Height - ClientSize.Height - GetTitleBarHeight());

            logEntryTextBox.KeyDown += TextBoxKeyDown;
            logEntryTextBox.KeyUp += TextBoxKeyUp;

            DisplayHoursTodaySoFar();
        }

        private int GetTitleBarHeight()
        {
            var screenRectangle=RectangleToScreen(ClientRectangle);
            return screenRectangle.Top - Top;
        }

        private void TextBoxKeyDown(object sender, KeyEventArgs e)
        {
            // Key @ is pressed. REMINDER! Duplicate knowledge, because @ is also hardcoded in LogEntryInputParser as special char for label.
            // An alternative is unknown to me how to make the below key configuration configurable.
            if (e.Shift && e.KeyCode == Keys.D2)
            {
                _autoFillListBoxController.DoAutoFillLabels();
                return;
            }

            // Key ! is pressed. REMINDER! Duplicate knowledge, because ! is also hardcoded in LogEntryInputParser as special char for activity.
            // An alternative is unknown to me how to make the below key configuration configurable.
            if (e.Shift && e.KeyCode == Keys.D1)
            {
                _autoFillListBoxController.DoAutoFillActivities();
                return;
            }

            if (e.KeyCode != Keys.Enter) return;

            if ((e.KeyCode == Keys.Enter) || (e.KeyCode == Keys.Tab))
            {
                e.Handled = e.SuppressKeyPress = true;
            }
        }

        // Cursor to end of the text in the text box
        public void HandleAutoFillFinished(string autoFilledText = null)
        {
            if (!string.IsNullOrEmpty(autoFilledText))
            {
                logEntryTextBox.Text += autoFilledText;
            }
            logEntryTextBox.SelectionStart = logEntryTextBox.Text.Length;
            logEntryTextBox.SelectionLength = 0;
            logEntryTextBox.Focus();
        }

        private void TextBoxKeyUp(object sender, KeyEventArgs e)
        {
            DetermineTextColor();
            if (e.KeyCode != Keys.Enter) return;

            var input = logEntryTextBox.Text;
            if (_inputParser.TryParse(input, out var logEntry))
            {
                _repository.SaveLogEntry(logEntry);

                // Clear text box
                logEntryTextBox.Text = string.Empty;
                logEntryTextBox.ForeColor = Color.Red;
            }
            
            DisplayHoursTodaySoFar();
        }

        private void DisplayHoursTodaySoFar()
        {
            Text = $"Log time ({_repository.GetTotalLoggedHoursForDate(DateTime.Today).ToString("0.00")})";
        }

        private void DetermineTextColor()
        {
            var input = logEntryTextBox.Text;
            logEntryTextBox.ForeColor = _inputParser.TryParse(input, out _)
                ? Color.Green
                : Color.Red;
        }
    }
}
