using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using AlteryxGuiToolkit.Plugins;
using System.Xml;


namespace ContinuumProcessRunner
{
    public partial class ProcessRunnerUserControl : UserControl, IPluginConfiguration
    {
        public static string TIP_EXEPATH = "[REQUIRED]\r\nSelect the field holding the full path to the executable to run\r\neg: C:/Program Files/Python/python3.exe";
        public static string TIP_STDOUT = "[REQUIRED]\r\nSet a name for the field to record the redirected Standard Output from the executable (usually what the Executable prints)";
        public static string TIP_RETCODE = "[REQUIRED]\r\nSet a name for the field to record the exit code from the executable\r\n(usually 0=Success, and other values are error codes)";
        public static string TIP_EXCEPTIONS = "[REQUIRED]\r\nSet a name for the field to record any exceptions (errors) that the executable throws";
        public static string TIP_SELECTEDCOLS = "Checked columns will be converted to string and passed as command line arguments.\r\nThe ExePath column will not be passed, it will be ignored regardless of check state.";
        public static string TIP_SELECTEDCOLS_ALL = "Set all columns as Checked.";
        public static string TIP_SELECTEDCOLS_NONE = "Reset all columns to Unchecked.";
        public static string TIP_SELECTEDCOLS_INVERT = "Reverse the checks for all columns.";
        public static string TIP_SELECTEDCOLS_FORGET = "Remove columns that are marked as missing.";


        public static string MISSING = " (Missing)";

        private static string diags = Constants.DEFAULTDIAGS;
        private static string autoEscape = Constants.DEFAULTAUTOESCAPE;

        public ProcessRunnerUserControl()
        {
            InitializeComponent();
            setToolTips();
            checkedListBoxSelectedCols.ItemCheck += new ItemCheckEventHandler(checkedListBoxSelectedCols_ItemCheck);
        }


        private void setToolTips()
        {
            toolTip1.SetToolTip(labelExePath, TIP_EXEPATH);
            toolTip1.SetToolTip(comboBoxExePathField, TIP_EXEPATH);

            toolTip1.SetToolTip(labelStdOutField, TIP_STDOUT);
            toolTip1.SetToolTip(textBoxStdOutField, TIP_STDOUT);

            toolTip1.SetToolTip(labelRetCodeField, TIP_RETCODE);
            toolTip1.SetToolTip(textBoxRetCodeField, TIP_RETCODE);

            toolTip1.SetToolTip(labelExceptionField, TIP_EXCEPTIONS);
            toolTip1.SetToolTip(textBoxExceptionField, TIP_EXCEPTIONS);

            toolTip1.SetToolTip(labelSelectedCols, TIP_SELECTEDCOLS);
            toolTip1.SetToolTip(checkedListBoxSelectedCols, TIP_SELECTEDCOLS);

            toolTip1.SetToolTip(linkLabelAll, TIP_SELECTEDCOLS_ALL);
            toolTip1.SetToolTip(linkLabelNone, TIP_SELECTEDCOLS_NONE);
            toolTip1.SetToolTip(linkLabelInvert, TIP_SELECTEDCOLS_INVERT);
            toolTip1.SetToolTip(linkLabelForget, TIP_SELECTEDCOLS_FORGET);
        }

        public Control GetConfigurationControl(
            AlteryxGuiToolkit.Document.Properties docProperties,
            XmlElement eConfig,
            XmlElement[] eIncomingMetaInfo,
            int nToolId,
            string strToolName)
        {
            XmlInputConfiguration xmlConfig = XmlInputConfiguration.LoadFromConfiguration(eConfig);

            if (xmlConfig == null)
                return this;

            ///////////////////////////////////////////////////////////////////
            // Populate GUI Controls with saved config information
            //


            ///////////////////////
            // FIELD COMBOX BOXES
            //

            setComboBox(comboBoxExePathField, xmlConfig, Constants.EXEPATHFIELDKEY, eIncomingMetaInfo);


            /////////////////////////
            // SELECTED COLUMNS BOX
            //

            setCheckedListBox(checkedListBoxSelectedCols, xmlConfig, Constants.SELECTEDCOLSKEY, eIncomingMetaInfo);
            

            ///////////////////////////////////////////////////////////////////
            // Output Fields
            //

            textBoxStdOutField.Text = xmlConfig.StdOutField;
            textBoxRetCodeField.Text = xmlConfig.RetCodeField;
            textBoxExceptionField.Text = xmlConfig.ExceptionField;


            // Secrets
            diags = xmlConfig.Diags;
            autoEscape = xmlConfig.AutoEscape;

            return this;
        }

        private void saveSubForComboBox(ComboBox cbox, XmlElement eConfig, string key, string valueDefault)
        {
            XmlElement xe = XmlHelpers.GetOrCreateChildNode(eConfig, key);
            var selectedItem = cbox.SelectedItem;
            xe.InnerText = selectedItem == null ? valueDefault : selectedItem.ToString();
        }

        private void saveSubForCheckedListBox(CheckedListBox box, XmlElement eConfig, string key)
        {
            XmlElement xe = XmlHelpers.GetOrCreateChildNode(eConfig, key);

            List<string> selectedCols = new List<string>();

            foreach (var item in box.CheckedItems)
            {
                string selectedCol = item.ToString();

                // Drop the " (Missing)" suffix if it is appended
                if (selectedCol.EndsWith(MISSING))                
                    selectedCol = selectedCol.Substring(0, selectedCol.Length - MISSING.Length);

                selectedCols.Add(selectedCol);
            }
            
            if (selectedCols.Count() == 0)
                xe.InnerText = Constants.ZEROSELECTEDCOLS;
            else
                xe.InnerText = string.Join(",", selectedCols);
        }

        public void SaveResultsToXml(XmlElement eConfig, out string strDefaultAnnotation)
        {
            saveSubForComboBox(comboBoxExePathField, eConfig, Constants.EXEPATHFIELDKEY, Constants.DEFAULTEXEPATHFIELD);

            saveSubForCheckedListBox(checkedListBoxSelectedCols, eConfig, Constants.SELECTEDCOLSKEY);

            // Output Fields
            XmlElement xe = XmlHelpers.GetOrCreateChildNode(eConfig, Constants.STDOUTFIELDKEY);
            string stdOutField = textBoxStdOutField.Text;
            xe.InnerText = string.IsNullOrWhiteSpace(stdOutField) ? Constants.DEFAULTSTDOUTFIELD : stdOutField;

            xe = XmlHelpers.GetOrCreateChildNode(eConfig, Constants.RETCODEFIELDKEY);
            string retCodeField = textBoxRetCodeField.Text;
            xe.InnerText = string.IsNullOrWhiteSpace(retCodeField) ? Constants.DEFAULTRETCODEFIELD : retCodeField;

            xe = XmlHelpers.GetOrCreateChildNode(eConfig, Constants.EXCEPTIONFIELDKEY);
            string exceptionField = textBoxExceptionField.Text;
            xe.InnerText = string.IsNullOrWhiteSpace(exceptionField) ? Constants.DEFAULTEXCEPTIONFIELD : exceptionField;

            // Set the default annotation.
            strDefaultAnnotation = "ProcessRunner";

            // Save the secret flags
            xe = XmlHelpers.GetOrCreateChildNode(eConfig, Constants.DIAGSKEY);            
            xe.InnerText = string.IsNullOrWhiteSpace(diags) ? Constants.DEFAULTDIAGS : diags;

            xe = XmlHelpers.GetOrCreateChildNode(eConfig, Constants.AUTOESCAPEKEY);
            xe.InnerText = string.IsNullOrWhiteSpace(autoEscape) ? Constants.DEFAULTAUTOESCAPE : autoEscape;
        }


        // Helper
        private static bool isStringType(string fieldType)
        {
            return string.Equals(fieldType, "string", StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(fieldType, "v_string", StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(fieldType, "wstring", StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(fieldType, "v_wstring", StringComparison.OrdinalIgnoreCase);
        }

        private void setCheckedListBox(CheckedListBox box, XmlInputConfiguration xmlConfig, string key, XmlElement[] eIncomingMetaInfo)
        {
            string csvTargets = (string)xmlConfig[key];

            // Split the csv 
            string[] targets = csvTargets.Split(',');

            bool bCheckAll = false;
            if (targets.Length == 1 && String.Equals(targets[0], Constants.DEFAULTSELECTEDCOLS, StringComparison.OrdinalIgnoreCase))
            {
                targets = new string[0];
                bCheckAll = true;
            }

            // If there are no selected columns, clear the targets list
            if (targets.Length == 1 && String.Equals(targets[0], Constants.ZEROSELECTEDCOLS, StringComparison.OrdinalIgnoreCase))
                targets = new string[0];

            box.Items.Clear();

            if (eIncomingMetaInfo == null || eIncomingMetaInfo[0] == null)
            {
                // No incoming connection; 
                // Cycle through the targets, adding them as missing
                foreach (string target in targets)
                {
                    if (!bCheckAll)
                        box.Items.Add(target + MISSING, true);
                }
            }
            else
            {
                // We have an incoming connection

                var xmlElementMetaInfo = eIncomingMetaInfo[0];
                var xmlElementRecordInfo = xmlElementMetaInfo.FirstChild;
                string fieldName = "";

                foreach (XmlElement elementChild in xmlElementRecordInfo)
                {
                    fieldName = elementChild.GetAttribute("name");
                    bool bSelected = false;

                    foreach (string target in targets)
                    {
                        // If the current field on the inbound stream is selected, set the checked flag
                        bSelected = bSelected || String.Equals(fieldName, target, StringComparison.OrdinalIgnoreCase);
                    }

                    // If bCheckAll is set, select this column anyway
                    bSelected = bSelected || bCheckAll;

                    box.Items.Add(fieldName, bSelected);
                }

                // Add the missing fields
                foreach (string target in targets)
                {
                    bool bExists = false;

                    foreach (XmlElement elementChild in xmlElementRecordInfo)
                    {
                        fieldName = elementChild.GetAttribute("name");

                        // If the current field in the target list exists, set the exists flag
                        bExists = bExists || String.Equals(fieldName, target, StringComparison.OrdinalIgnoreCase);
                    }

                    if (!bExists)
                    {
                        // Target does not exist in the inbound fields list, so it is missing
                        box.Items.Add(target + MISSING, true);
                    }
                }
            }
        }

        private void setComboBox(ComboBox cbox, XmlInputConfiguration xmlConfig, string key, XmlElement[] eIncomingMetaInfo)
        {
            string target = (string)xmlConfig[key];

            cbox.Items.Clear();

            if (eIncomingMetaInfo == null || eIncomingMetaInfo[0] == null)
            {
                // No incoming connection;  Just add the field and select it
                cbox.Items.Add(target);
                cbox.SelectedIndex = 0;
            }
            else
            {
                // We have an incoming connection

                var xmlElementMetaInfo = eIncomingMetaInfo[0];
                var xmlElementRecordInfo = xmlElementMetaInfo.FirstChild;
                foreach (XmlElement elementChild in xmlElementRecordInfo)
                {
                    string fieldName = elementChild.GetAttribute("name");
                    string fieldType = elementChild.GetAttribute("type");

                    if (isStringType(fieldType))
                        cbox.Items.Add(fieldName);
                }

                // If the messageField matches a possible field in the combo box, make it the selected field.
                // If the field does not match, do not select anything and blank the field.
                if (!string.IsNullOrWhiteSpace(target))
                {
                    int selectedIndex = cbox.FindStringExact(target);
                    if (selectedIndex >= 0)
                        cbox.SelectedIndex = selectedIndex; // Found; Select this item                    
                }

            } // end of "if (eIncomingMetaInfo == null || eIncomingMetaInfo[0] == null)"
        }

        private void checkedListBoxSelectedCols_Click(object sender, EventArgs e)
        {
            var dummy = false;
        }

        private void checkedListBoxSelectedCols_ItemCheck(object sender, EventArgs e)
        {
            if (checkedListBoxSelectedCols.SelectedIndex != -1)
                checkedListBoxSelectedCols.SetSelected(checkedListBoxSelectedCols.SelectedIndex, false);
        }

        private void linkLabelAll_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // Check all checkboxes
            int countItems = checkedListBoxSelectedCols.Items.Count;
            for (var i = 0; i < countItems; ++i)
            {
                checkedListBoxSelectedCols.SetItemCheckState(i, System.Windows.Forms.CheckState.Checked);
            }
        }

        private void linkLabelNone_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // Uncheck all checkboxes
            int countItems = checkedListBoxSelectedCols.Items.Count;
            for (var i = 0; i < countItems; ++i)
            {
                checkedListBoxSelectedCols.SetItemCheckState(i, CheckState.Unchecked);
            }
        }

        private void linkLabelInvert_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // Invert check for all checkboxes
            int countItems = checkedListBoxSelectedCols.Items.Count;
            for (var i = 0; i < countItems; ++i)
            {
                var state = checkedListBoxSelectedCols.GetItemCheckState(i);
                
                if (state == CheckState.Checked)
                    checkedListBoxSelectedCols.SetItemCheckState(i, CheckState.Unchecked);
                else
                    checkedListBoxSelectedCols.SetItemCheckState(i, CheckState.Checked);
            }
        }

        private void linkLabelForget_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // Remove items that are missing
            var itemsToRemove = new List<String>();

            int countItems = checkedListBoxSelectedCols.Items.Count;
            for (var i = 0; i < countItems; ++i)
            {
                string itemText = checkedListBoxSelectedCols.Items[i].ToString();
                if (itemText.EndsWith(MISSING))
                {
                    itemsToRemove.Add(itemText);
                }
            }

            foreach (var itemToRemove in itemsToRemove)
            {
                checkedListBoxSelectedCols.Items.Remove(itemToRemove);
            }
        }
    }
}
