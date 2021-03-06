﻿using System;
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
        public static string DIAGNOSTICFIELDKEY = "DiagnosticField";
        public static string AUTOESCAPEKEY = "AutoEscape";
        public static string SELECTEDCOLSKEY = "SelectedCols";

        // Default Values
        public static string DEFAULTEXEPATHFIELD = "ProcessRunnerExePath";
        public static string DEFAULTSTDOUTFIELD = "ProcessRunnerStdOut";
        public static string DEFAULTRETCODEFIELD = "ProcessRunnerReturnCode";
        public static string DEFAULTEXCEPTIONFIELD = "ProcessRunnerException";
        public static string DEFAULTDIAGNOSTICFIELD = "ProcessRunnerDiagnostics";
        public static string DEFAULTAUTOESCAPE = "Y";
        public static string DEFAULTSELECTEDCOLS = "##ALLCOLUMNS##";
        public static string ZEROSELECTEDCOLS = "##NOCOLUMNS##";

        public static int LARGEOUTPUTFIELDSIZE = Int32.MaxValue;
        public static int SMALLOUTPUTFIELDSIZE = 512;

        
    }
}
