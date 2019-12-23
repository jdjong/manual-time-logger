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
        }

        private int GetTitleBarHeight()
        {
            System.Drawing.Rectangle screenRectangle=RectangleToScreen(this.ClientRectangle);
            return screenRectangle.Top - this.Top;
        }
    }
}
