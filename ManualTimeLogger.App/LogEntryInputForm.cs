using System;
using System.Drawing;
using System.Windows.Forms;
using ManualTimeLogger.Domain;
using ManualTimeLogger.Persistence;

namespace ManualTimeLogger.App
{
    public partial class LogEntryInputForm : Form
    {
        private readonly LogEntryInputParser _inputParser;
        private readonly IRepository _repository;
        private readonly TextBox _logEntryTextBox;

        public LogEntryInputForm(LogEntryInputParser inputParser, IRepository repository)
        {
            _inputParser = inputParser;
            _repository = repository;

            InitializeComponent();
            // Working area is task-bar exclusive. Correct layout is guaranteed only for bottom task-bars only.
            Location = new Point(Screen.PrimaryScreen.WorkingArea.Width - ClientSize.Width, Screen.PrimaryScreen.WorkingArea.Height - ClientSize.Height - GetTitleBarHeight());

            _logEntryTextBox = Controls["logEntryTextBox"] as TextBox;
            if (_logEntryTextBox != null)
            {
                _logEntryTextBox.KeyDown += TextBoxKeyDown;
                _logEntryTextBox.KeyUp += TextBoxKeyUp;
            }
            else
            {
                throw new ApplicationException("Text box logEntryTextBox is not a control or a text box");
            }
        }

        private int GetTitleBarHeight()
        {
            var screenRectangle=RectangleToScreen(ClientRectangle);
            return screenRectangle.Top - Top;
        }

        private void TextBoxKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter) return;

            if ((e.KeyCode == Keys.Enter) || (e.KeyCode == Keys.Tab))
            {
                e.Handled = e.SuppressKeyPress = true;
            }
        }

        private void TextBoxKeyUp(object sender, KeyEventArgs e)
        {
            DetermineTextColor();
            if (e.KeyCode != Keys.Enter) return;

            var input = _logEntryTextBox.Text;
            if (_inputParser.TryParse(input, out var logEntry))
            {
                _repository.SaveLogEntry(logEntry);
                _logEntryTextBox.Text = string.Empty;
                _logEntryTextBox.ForeColor = Color.Red;
            }
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
