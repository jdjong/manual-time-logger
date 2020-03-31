using System;
using System.Drawing;
using System.Timers;
using System.Windows.Forms;
using ManualTimeLogger.Domain;
using ManualTimeLogger.Persistence;

namespace ManualTimeLogger.App
{
    public partial class LogEntryInputForm : Form, ITimeLoggedHandler
    {
        private readonly IRepository _repository;
        private TimeSpan _openTime;
        private readonly TimeSpan _timerTime;

        public LogEntryInputForm(IRepository repository, LogEntryTextBoxController logEntryTextBoxController)
        {
            _timerTime = TimeSpan.FromMinutes(1);
            _openTime = TimeSpan.Zero;
            
            _repository = repository;

            InitializeComponent();

            logEntryTextBoxController.Init(this, logEntryTextBox, autoFillListBox);

            // Working area is area without the task-bar. Correct layout is guaranteed for bottom task-bars only.
            Location = new Point(Screen.PrimaryScreen.WorkingArea.Width - ClientSize.Width, Screen.PrimaryScreen.WorkingArea.Height - ClientSize.Height - GetTitleBarHeight());

            UpdateTitle();
            StartTimer();
        }

        private void StartTimer()
        {
            var timer = new System.Timers.Timer(_timerTime.TotalMilliseconds);
            timer.Elapsed += OnTimerElapsed;
            timer.Start();
        }

        private void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            _openTime += _timerTime;
            UpdateTitle();
        }

        private int GetTitleBarHeight()
        {
            var screenRectangle=RectangleToScreen(ClientRectangle);
            return screenRectangle.Top - Top;
        }

        private void UpdateTitle()
        {
            // ReSharper disable once LocalizableElement
            Text = $"log {_repository.GetTotalLoggedHoursForDate(DateTime.Today):0.00} open {_openTime.TotalHours:0.00}";
        }

        public void HandleTimeLogged(LogEntry logEntry)
        {
            _repository.SaveLogEntry(logEntry);

            _openTime -= TimeSpan.FromHours(logEntry.Duration);
            if (_openTime < TimeSpan.Zero)
            {
                _openTime = TimeSpan.Zero;
            }

            UpdateTitle();
        }
    }
}
