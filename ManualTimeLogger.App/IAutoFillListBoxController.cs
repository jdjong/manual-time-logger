using System.Windows.Forms;

namespace ManualTimeLogger.App
{
    public interface IAutoFillListBoxController
    {
        void Init(IHandleAutoFill parentForm, ListBox autoFillListBox);
        void DoAutoFillLabels();
        void DoAutoFillActivities();
    }
}