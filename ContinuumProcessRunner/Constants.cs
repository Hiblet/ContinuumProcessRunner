using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContinuumProcessRunner
{
    public class Constants
    {
        // Keys for XML data storage: 
        // THESE MUST MATCH THE ACCESSOR NAMES IN THE XmlInputConfiguration.cs FILE
        public static string EXEPATHFIELDKEY = "ExePathField";
        public static string STDOUTFIELDKEY = "StdOutField";
        public static string RETCODEFIELDKEY = "RetCodeField";
        public static string EXCEPTIONFIELDKEY = "ExceptionField";
        public static string DIAGSKEY = "Diags";
        public static string AUTOESCAPEKEY = "AutoEscape";

        // Default Values
        public static string DEFAULTEXEPATHFIELD = "ProcessRunnerExePath";
        public static string DEFAULTSTDOUTFIELD = "ProcessRunnerStdOut";
        public static string DEFAULTRETCODEFIELD = "ProcessRunnerReturnCode";
        public static string DEFAULTEXCEPTIONFIELD = "ProcessRunnerException";
        public static string DEFAULTDIAGSFIELD = "ProcessRunnerDiagnostics";
        public static string DEFAULTDIAGS = "N";
        public static string DEFAULTAUTOESCAPE = "Y";

        public static int LARGEOUTPUTFIELDSIZE = Int32.MaxValue;
        public static int SMALLOUTPUTFIELDSIZE = 512;
    }
}
