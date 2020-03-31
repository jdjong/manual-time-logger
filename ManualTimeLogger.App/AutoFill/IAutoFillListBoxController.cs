using System.Windows.Forms;

namespace ManualTimeLogger.App.AutoFill
{
    public interface IAutoFillListBoxController
    {
        void Init(IAutoFillHandler autoFillHandler, ListBox autoFillListBox);
        void DoAutoFillLabels();
        void DoAutoFillActivities();
    }
}