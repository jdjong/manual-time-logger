using System;
using System.Drawing;
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

        // TODO, introduce log entry text box controller

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
            #region hotkeyfunctionality

            // TODO, how to fix all this hardcoded duplication knowledge below
            // Key 1 is pressed. REMINDER! Duplicate knowledge, because nb is also hardcoded in LogEntryInputParser as special char for label.
            // An alternative is unknown to me how to make the below key configuration configurable.
            if (string.IsNullOrEmpty(logEntryTextBox.Text) && !e.Shift &&(e.KeyCode == Keys.D1 || e.KeyCode == Keys.NumPad1))
            {
                logEntryTextBox.Text = "nb";
                MoveTextBoxCursorToEndOfText();
                e.Handled = e.SuppressKeyPress = true;
                return;
            }

            if (string.IsNullOrEmpty(logEntryTextBox.Text) && !e.Shift && (e.KeyCode == Keys.D2 || e.KeyCode == Keys.NumPad2))
            {
                logEntryTextBox.Text = "norma";
                MoveTextBoxCursorToEndOfText();
                e.Handled = e.SuppressKeyPress = true;
                return;
            }

            if (string.IsNullOrEmpty(logEntryTextBox.Text) && !e.Shift && (e.KeyCode == Keys.D3 || e.KeyCode == Keys.NumPad3))
            {
                logEntryTextBox.Text = "nwb";
                MoveTextBoxCursorToEndOfText();
                e.Handled = e.SuppressKeyPress = true;
                return;
            }

            if (string.IsNullOrEmpty(logEntryTextBox.Text) && !e.Shift && (e.KeyCode == Keys.D4 || e.KeyCode == Keys.NumPad4))
            {
                logEntryTextBox.Text = "roi";
                MoveTextBoxCursorToEndOfText();
                e.Handled = e.SuppressKeyPress = true;
                return;
            }

            if (string.IsNullOrEmpty(logEntryTextBox.Text) && !e.Shift && (e.KeyCode == Keys.D5 || e.KeyCode == Keys.NumPad5))
            {
                logEntryTextBox.Text = "sogyo";
                MoveTextBoxCursorToEndOfText();
                e.Handled = e.SuppressKeyPress = true;
                return;
            }

            #endregion

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

            MoveTextBoxCursorToEndOfText();
            DetermineTextColor();
            logEntryTextBox.Focus();
        }

        private void MoveTextBoxCursorToEndOfText()
        {
            logEntryTextBox.SelectionStart = logEntryTextBox.Text.Length;
            logEntryTextBox.SelectionLength = 0;
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
