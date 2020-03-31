using System.Windows.Forms;

namespace ManualTimeLogger.App
{
    public class DoNothingAutoFillListBoxController : IAutoFillListBoxController
    {
        public void Init(IHandleAutoFill parentForm, ListBox autoFillListBox)
        {
        }

        public void DoAutoFillLabels()
        {
        }

        public void DoAutoFillActivities()
        {
        }
    }
}
