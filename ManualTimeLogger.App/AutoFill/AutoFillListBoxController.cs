using System;
using System.Linq;
using System.Windows.Forms;

namespace ManualTimeLogger.App.AutoFill
{
    public class AutoFillListBoxController : IAutoFillListBoxController
    {
        private IAutoFillHandler _autoFillHandler;
        private ListBox _autoFillListBox;
        private readonly string[] _labelPresetList;
        private readonly string[] _activityPresetList;

        public AutoFillListBoxController(string[] labelPresetList, string[] activityPresetList)
        {
            _labelPresetList = labelPresetList;
            _activityPresetList = activityPresetList;
        }

        public void Init(IAutoFillHandler autoFillHandler, ListBox autoFillListBox)
        {
            _autoFillHandler = autoFillHandler;
            _autoFillListBox = autoFillListBox;

            autoFillListBox.PreviewKeyDown += AutoFillListBoxPreviewKeyHandler;
            autoFillListBox.KeyUp += AutoFillListBoxKeyUpHandler;
            autoFillListBox.LostFocus += LostFocusHandler;
        }

        public void DoAutoFillLabels()
        {
            FillAutoFillListBoxWith(_labelPresetList);
            ShowAutoFillListBox();
        }

        public void DoAutoFillActivities()
        {
            FillAutoFillListBoxWith(_activityPresetList);
            ShowAutoFillListBox();
        }

        private void FillAutoFillListBoxWith(string[] items)
        {
            _autoFillListBox.Items.Clear();
            items.ToList().ForEach(item => { _autoFillListBox.Items.Add(item); });
        }

        private void ShowAutoFillListBox()
        {
            _autoFillListBox.Visible = true;
            _autoFillListBox.MultiColumn = false;
            _autoFillListBox.Sorted = true;
            _autoFillListBox.SetSelected(0, true);
            _autoFillListBox.Focus();
        }

        private void AutoFillListBoxPreviewKeyHandler(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Tab)
                e.IsInputKey = true;
        }

        private void AutoFillListBoxKeyUpHandler(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Escape:
                    HideAutoFillListBox();
                    _autoFillHandler.HandleAutoFillFinished();
                    break;
                case Keys.Enter:
                    HideAutoFillListBox();
                    _autoFillHandler.HandleAutoFillFinished(_autoFillListBox.SelectedItem as string);
                    break;
                case Keys.Tab:
                    HideAutoFillListBox();
                    _autoFillHandler.HandleAutoFillFinished(_autoFillListBox.SelectedItem as string);
                    break;
            }

            if ((e.KeyCode == Keys.Enter) || (e.KeyCode == Keys.Tab))
            {
                e.Handled = e.SuppressKeyPress = true;
            }
        }

        private void LostFocusHandler(object sender, EventArgs e)
        {
            HideAutoFillListBox();
        }

        private void HideAutoFillListBox()
        {
            _autoFillListBox.Visible = false;
        }
    }
}
