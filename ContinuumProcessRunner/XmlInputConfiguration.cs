using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Xml;

namespace ContinuumProcessRunner
{
    public class XmlInputConfiguration
    {
        public string ExePathField { get; private set; }
        public string StdOutField { get; private set; }
        public string RetCodeField { get; private set; }
        public string ExceptionField { get; private set; }




        // Note that the constructor is private.  Instances are created through the LoadFromConfigration method.
        private XmlInputConfiguration(
            string exePathField,
            string stdOutField,
            string retCodeField,
            string exceptionField)
        {
            ExePathField = exePathField;
            StdOutField = stdOutField;
            RetCodeField = retCodeField;
            ExceptionField = exceptionField;
        }

        public static string getStringFromConfig(XmlElement eConfig, string key, string valueDefault)
        {
            string sReturn = valueDefault;

            XmlElement xe = eConfig.SelectSingleNode(key) as XmlElement;
            if (xe != null)
            {
                if (!string.IsNullOrEmpty(xe.InnerText))
                    sReturn = xe.InnerText;
            }

            return sReturn;
        }

        public static XmlInputConfiguration LoadFromConfiguration(XmlElement eConfig)
        {
            string exePathField = getStringFromConfig(eConfig, Constants.EXEPATHFIELDKEY, Constants.DEFAULTEXEPATHFIELD);
            string stdOutField = getStringFromConfig(eConfig, Constants.STDOUTFIELDKEY, Constants.DEFAULTSTDOUTFIELD);
            string retCodeField = getStringFromConfig(eConfig, Constants.RETCODEFIELDKEY, Constants.DEFAULTRETCODEFIELD);
            string exceptionField = getStringFromConfig(eConfig, Constants.EXCEPTIONFIELDKEY, Constants.DEFAULTEXCEPTIONFIELD);

            return new XmlInputConfiguration(
                exePathField, 
                stdOutField, 
                retCodeField, 
                exceptionField);
        }

        // Property Name Accessor
        public object this[string propertyName]
        {
            get { return this.GetType().GetProperty(propertyName).GetValue(this, null); }
            set { this.GetType().GetProperty(propertyName).SetValue(this, value, null); }
        }
    }
}
