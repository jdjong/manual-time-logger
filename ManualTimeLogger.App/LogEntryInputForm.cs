using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using ManualTimeLogger.Domain;
using ManualTimeLogger.Persistence;

namespace ManualTimeLogger.App
{
    public partial class LogEntryInputForm : Form
    {
        private readonly bool _isExistingLabelListBoxDisplayFeatureEnabled = false;
        
        private readonly LogEntryInputParser _inputParser;
        private readonly IRepository _repository;

        public LogEntryInputForm(LogEntryInputParser inputParser, IRepository repository)
        {
            _inputParser = inputParser;
            _repository = repository;

            InitializeComponent();

            // Working area is area without the task-bar. Correct layout is guaranteed for bottom task-bars only.
            Location = new Point(Screen.PrimaryScreen.WorkingArea.Width - ClientSize.Width, Screen.PrimaryScreen.WorkingArea.Height - ClientSize.Height - GetTitleBarHeight());

            logEntryTextBox.KeyDown += TextBoxKeyDown;
            logEntryTextBox.KeyUp += TextBoxKeyUp;

            labelsListBox.KeyUp += ListBoxKeyUp;

            DisplayHoursToday();
        }

        private int GetTitleBarHeight()
        {
            var screenRectangle=RectangleToScreen(ClientRectangle);
            return screenRectangle.Top - Top;
        }

        private void TextBoxKeyDown(object sender, KeyEventArgs e)
        {
            // TODO, is @. Duplicate knowledge, because @ is also hardcoded in LogEntryInputParser as special char for label
            if (_isExistingLabelListBoxDisplayFeatureEnabled && e.Shift && e.KeyCode == Keys.D2)
            {
                labelsListBox.Visible = true;
                labelsListBox.Items.Clear();
                _repository
                    .GetExistingLabels()
                    .ToList()
                    .ForEach(label => labelsListBox.Items.Add(label));

                labelsListBox.MultiColumn = false;
                labelsListBox.Sorted = true;
                labelsListBox.SetSelected(0, true);
                labelsListBox.Focus();

                return;
            }
            
            if (e.KeyCode != Keys.Enter) return;

            if ((e.KeyCode == Keys.Enter) || (e.KeyCode == Keys.Tab))
            {
                e.Handled = e.SuppressKeyPress = true;
            }
        }

        private void ListBoxKeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Escape:
                    HideLabelsListBox();
                    SetTextBoxCursorToEndOfText();
                    break;
                case Keys.Enter:
                    // Add selected label
                    logEntryTextBox.Text += labelsListBox.SelectedItem as string;

                    HideLabelsListBox();
                    SetTextBoxCursorToEndOfText();
                    break;
            }

            if ((e.KeyCode == Keys.Enter) || (e.KeyCode == Keys.Tab))
            {
                e.Handled = e.SuppressKeyPress = true;
            }
        }

        // Cursor to end of the text in the text box
        private void SetTextBoxCursorToEndOfText()
        {
            logEntryTextBox.SelectionStart = logEntryTextBox.Text.Length;
            logEntryTextBox.SelectionLength = 0;
        }

        // Hide labels list box and bring focus to text box again
        private void HideLabelsListBox()
        {
            labelsListBox.Visible = false;
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
            
            DisplayHoursToday();
        }

        private void DisplayHoursToday()
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
