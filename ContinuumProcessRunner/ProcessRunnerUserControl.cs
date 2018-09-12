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

        private static string diags = Constants.DEFAULTDIAGS;
        private static string autoEscape = Constants.DEFAULTAUTOESCAPE;

        public ProcessRunnerUserControl()
        {
            InitializeComponent();
            setToolTips();
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

        public void SaveResultsToXml(XmlElement eConfig, out string strDefaultAnnotation)
        {
            saveSubForComboBox(comboBoxExePathField, eConfig, Constants.EXEPATHFIELDKEY, Constants.DEFAULTEXEPATHFIELD);

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

    }
}
