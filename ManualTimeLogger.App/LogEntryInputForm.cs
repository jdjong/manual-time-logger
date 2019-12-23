using System.Windows.Forms;
using ManualTimeLogger.Domain;
using ManualTimeLogger.Persistence;

namespace ManualTimeLogger.App
{
    public partial class LogEntryInputForm : Form
    {
        private readonly LogEntryInputParser _inputParser;
        private readonly IRepository _repository;

        public LogEntryInputForm(LogEntryInputParser inputParser, IRepository repository)
        {
            _inputParser = inputParser;
            _repository = repository;

            InitializeComponent();
            // Working area is task-bar exclusive. Correct layout is guaranteed only for bottom task-bars only.
            Location = new System.Drawing.Point(Screen.PrimaryScreen.WorkingArea.Width - ClientSize.Width, Screen.PrimaryScreen.WorkingArea.Height - ClientSize.Height - GetTitleBarHeight());
        }

        private int GetTitleBarHeight()
        {
            System.Drawing.Rectangle screenRectangle=RectangleToScreen(ClientRectangle);
            return screenRectangle.Top - Top;
        }
    }
}
