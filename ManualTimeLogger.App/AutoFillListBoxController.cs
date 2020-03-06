﻿using System.Linq;
using System.Windows.Forms;

namespace ManualTimeLogger.App
{
    public class AutoFillListBoxController
    {
        private IHandleAutoFill _parentForm;
        private ListBox _autoFillListBox;
        private readonly bool _isAutoFillFeatureEnabled;
        private readonly string[] _labelPresetList;
        private readonly string[] _activityPresetList;

        public AutoFillListBoxController(bool isAutoFillFeatureEnabled, string[] labelPresetList, string[] activityPresetList)
        {
            _isAutoFillFeatureEnabled = isAutoFillFeatureEnabled;
            _labelPresetList = labelPresetList;
            _activityPresetList = activityPresetList;
        }

        public void Init(IHandleAutoFill parentForm, ListBox autoFillListBox)
        {
            _parentForm = parentForm;
            _autoFillListBox = autoFillListBox;

            autoFillListBox.KeyUp += AutoFillListBoxKeyUpHandler;
        }

        public void DoAutoFillLabels()
        {
            if (!_isAutoFillFeatureEnabled) return;

            FillAutoFillListBoxWith(_labelPresetList);
            ShowAutoFillListBox();
        }

        public void DoAutoFillActivities()
        {
            if (!_isAutoFillFeatureEnabled) return;
            
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

        private void AutoFillListBoxKeyUpHandler(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Escape:
                    HideAutoFillListBox();
                    _parentForm.HandleAutoFillFinished();
                    break;
                case Keys.Enter:
                    HideAutoFillListBox();
                    _parentForm.HandleAutoFillFinished(_autoFillListBox.SelectedItem as string);
                    break;
            }

            if ((e.KeyCode == Keys.Enter) || (e.KeyCode == Keys.Tab))
            {
                e.Handled = e.SuppressKeyPress = true;
            }
        }

        // Hide labels list box and bring focus to text box again
        private void HideAutoFillListBox()
        {
            _autoFillListBox.Visible = false;
        }
    }
}