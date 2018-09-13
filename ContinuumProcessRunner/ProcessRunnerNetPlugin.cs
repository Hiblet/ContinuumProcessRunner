﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Xml;
using AlteryxRecordInfoNet;
using System.IO;
using System.Diagnostics;
using CSharpTest.Net.Processes;
using CSharpTest.Net.Utils;

namespace ContinuumProcessRunner
{
    public class ProcessRunnerNetPlugin : INetPlugin, IIncomingConnectionInterface
    {
        private int _toolID; // Integer identifier provided by Alteryx, this tools tool number.
        private EngineInterface _engineInterface; // Reference provided by Alteryx so that we can talk to the engine.
        private XmlElement _xmlProperties; // Xml configuration for this custom tool

        private PluginOutputConnectionHelper _outputHelper;

        private RecordInfo _recordInfoIn;
        private RecordInfo _recordInfoOut;

        // App Specific Variables
        private string _exePathField = Constants.DEFAULTEXEPATHFIELD;
        private string _stdOutField = Constants.DEFAULTSTDOUTFIELD;
        private string _retCodeField = Constants.DEFAULTRETCODEFIELD;
        private string _exceptionsField = Constants.DEFAULTEXCEPTIONFIELD;
        private string _diagsField = Constants.DEFAULTDIAGSFIELD;
        private string _selectedCols = Constants.DEFAULTSELECTEDCOLS;

        private string _cmdLine = "";
        private string _diags = "N";
        private string _autoEscape = "Y";
        private string _retCode = "";        

        private StringBuilder _sbStdOut = new StringBuilder();
        private string _sbStdOutLock = "sbStdOutLock"; // Multi-threading lock for StringBuilder object

        private StringBuilder _sbExceptions = new StringBuilder();
        private string _sbExceptionsLock = "sbExceptionsLock"; // Lock object




        public void PI_Init(int nToolID, EngineInterface engineInterface, XmlElement pXmlProperties)
        {
            _toolID = nToolID;
            _engineInterface = engineInterface;
            _xmlProperties = pXmlProperties;

            // Use the information in the pXmlProperties parameter to get the user control info

            XmlElement configElement = XmlHelpers.GetFirstChildByName(_xmlProperties, "Configuration", true);
            if (configElement != null)
            {
                getConfigSetting(configElement, Constants.EXEPATHFIELDKEY, ref _exePathField);

                getConfigSetting(configElement, Constants.STDOUTFIELDKEY, ref _stdOutField);
                getConfigSetting(configElement, Constants.RETCODEFIELDKEY, ref _retCodeField);
                getConfigSetting(configElement, Constants.EXCEPTIONFIELDKEY, ref _exceptionsField);

                getConfigSetting(configElement, Constants.DIAGSKEY, ref _diags);
                getConfigSetting(configElement, Constants.AUTOESCAPEKEY, ref _autoEscape);

                getConfigSetting(configElement, Constants.SELECTEDCOLSKEY, ref _selectedCols);
            }

            _outputHelper = new AlteryxRecordInfoNet.PluginOutputConnectionHelper(_toolID, _engineInterface);
        }


        public IIncomingConnectionInterface PI_AddIncomingConnection(string pIncomingConnectionType, string pIncomingConnectionName)
        {
            return this;
        }

        public bool PI_AddOutgoingConnection(string pOutgoingConnectionName, OutgoingConnection outgoingConnection)
        {
            _outputHelper.AddOutgoingConnection(outgoingConnection);
            return true;
        }

        public bool PI_PushAllRecords(long nRecordLimit)
        {
            return true;
        }

        public void PI_Close(bool bHasErrors)
        {
            _outputHelper.Close();
        }

        public bool ShowDebugMessages()
        {
            return true;
        }


        public XmlElement II_GetPresortXml(XmlElement pXmlProperties)
        {
            return null;
        }

        public bool II_Init(RecordInfo recordInfo)
        {
            _recordInfoIn = recordInfo;
            prepRecordInfoOut(); // This allows zero record run to succeed and fixes problem with downstream tool complaining about a stream not being initialized.
            return true;
        }

        public void II_Close()
        { }

        public void II_UpdateProgress(double dPercent)
        {
            // Since our progress is directly proportional to he progress of the
            // upstream tool, we can simply output it's percentage as our own.
            if (_engineInterface.OutputToolProgress(_toolID, dPercent) != 0)
            {
                // If this returns anything but 0, then the user has canceled the operation.
                throw new AlteryxRecordInfoNet.UserCanceledException();
            }

            // Have the PluginOutputConnectionHelper ask the downstream tools to update their progress.
            _outputHelper.UpdateProgress(dPercent);
        }


        public bool II_PushRecord(RecordData recordDataIn)
        {
            prepRecordInfoOut();

            // The same object is used in each call, so reset the result fields before using them.
            _retCode = "";
            _cmdLine = "";
            lock (_sbStdOutLock) { _sbStdOut.Clear(); }
            lock (_sbExceptionsLock) { _sbExceptions.Clear(); }



            // Check the paths to Executable and Script...
            string exePath = getFieldBaseStringData(_exePathField, recordDataIn);
            checkValidPath(exePath, _exePathField);

            // Do the magic
            shell(exePath, getArguments(recordDataIn));

            // Get the output
            string stdOut = "";
            lock (_sbStdOutLock)
            { stdOut = _sbStdOut.ToString(); }

            // Get the exceptions
            string exceptions = "";
            lock (_sbExceptionsLock)
            { exceptions = _sbExceptions.ToString(); }


            // Prepare the output
            AlteryxRecordInfoNet.Record recordOut = _recordInfoOut.CreateRecord();
            recordOut.Reset();

            // Transfer existing data
            copyRecordData(recordDataIn, recordOut);


            // Set Output Fields
            var numInputFields = (int)_recordInfoIn.NumFields();

            AlteryxRecordInfoNet.FieldBase fbStdOut = _recordInfoOut[numInputFields];
            fbStdOut.SetFromString(recordOut, stdOut);

            AlteryxRecordInfoNet.FieldBase fbRetCode = _recordInfoOut[numInputFields + 1];
            fbRetCode.SetFromString(recordOut, _retCode);

            AlteryxRecordInfoNet.FieldBase fbExceptions = _recordInfoOut[numInputFields + 2];
            fbExceptions.SetFromString(recordOut, exceptions);

            if (isTrueString(_diags))
            {
                AlteryxRecordInfoNet.FieldBase fbDiags = _recordInfoOut[numInputFields + 3];
                fbDiags.SetFromString(recordOut, _cmdLine);
            }

            // Output
            _outputHelper.PushRecord(recordOut.GetRecord());


            // Clear the accumulated strings for next record
            _retCode = "";
            _cmdLine = "";
            lock (_sbStdOutLock) { _sbStdOut.Clear(); }
            lock (_sbExceptionsLock) { _sbExceptions.Clear(); }

            return true;
        }





        /////////////////////////////////////////////////////////////////////// 
        // HELPERS
        //

        private void getConfigSetting(XmlElement configElement, string key, ref string memberToSet)
        {
            XmlElement xe = XmlHelpers.GetFirstChildByName(configElement, key, false);
            if (xe != null)
            {
                if (!string.IsNullOrWhiteSpace(xe.InnerText))
                    memberToSet = xe.InnerText;
            }
        }

        private void prepRecordInfoOut()
        {
            if (_recordInfoOut == null)
            {
                // Prep the output once
                populateRecordInfoOut();
                _outputHelper.Init(_recordInfoOut, "Output", null, _xmlProperties);
            }
        }

        private void populateRecordInfoOut()
        {
            _recordInfoOut = new AlteryxRecordInfoNet.RecordInfo();

            // Copy the fieldbase structure of the incoming record
            uint countFields = _recordInfoIn.NumFields();
            for (int i = 0; i < countFields; ++i)
            {
                FieldBase fbIn = _recordInfoIn[i];
                _recordInfoOut.AddField(fbIn.GetFieldName(), fbIn.FieldType, (int)fbIn.Size, fbIn.Scale, fbIn.GetSource(), fbIn.GetDescription());
            }

            // Add the output columns at the end
            _recordInfoOut.AddField(_stdOutField, FieldType.E_FT_V_WString, Constants.LARGEOUTPUTFIELDSIZE, 0, "", "");
            _recordInfoOut.AddField(_retCodeField, FieldType.E_FT_V_WString, Constants.SMALLOUTPUTFIELDSIZE, 0, "", "");
            _recordInfoOut.AddField(_exceptionsField, FieldType.E_FT_V_WString, Constants.LARGEOUTPUTFIELDSIZE, 0, "", "");

            if (isTrueString(_diags))            
                _recordInfoOut.AddField(_diagsField, FieldType.E_FT_V_WString, Constants.LARGEOUTPUTFIELDSIZE, 0, "", "");            
        }

        private void copyRecordData(RecordData recordDataIn, Record recordOut)
        {
            uint countFields = _recordInfoIn.NumFields();
            for (int i = 0; i < countFields; ++i)
            {
                FieldBase fbIn = _recordInfoIn[i];
                FieldBase fbOut = _recordInfoOut[i];

                // Point a fieldbase reference to the record out item.
                string fbData = "";
                try
                {
                    fbData = fbIn.GetAsString(recordDataIn) ?? "";
                }
                catch (NullReferenceException)
                {
                    // If there is no data, catch and write out an empty string
                    fbData = "";
                }
                finally
                {
                    fbOut.SetFromString(recordOut, fbData);
                }
            }
        }

        private string getFieldBaseStringData(string fieldName, RecordData recordDataIn)
        {
            FieldBase fb = null;
            try
            {
                fb = _recordInfoIn.GetFieldByName(fieldName, true);
            }
            catch
            {
                throw new Exception($"The field [{fieldName}] was not found.");
            }

            return fb.GetAsString(recordDataIn);
        }

        private string getFieldBaseStringData_Optional(string fieldName, RecordData recordDataIn)
        {
            FieldBase fb = null;
            try
            {
                fb = _recordInfoIn.GetFieldByName(fieldName, true);
            }
            catch
            {
                // For optional fields, return empty string
                return "";
            }

            return fb.GetAsString(recordDataIn);
        }





        ///////////////////////////////////////////////////////////////////////
        // APP SPECIFICS
        //

        /// <summary>
        /// Check that the path to a file is valid
        /// </summary>
        /// <param name="path">string; Full path to file, including extension;  Relative references are allowed</param>
        /// <param name="columnName">string; Name of column holding path, for error message</param>
        private void checkValidPath(string path, string columnName)
        {
            // Try to create a file info object, and if the path is 
            // invalid, or you don't have privilege, it will exception.
            try
            {
                if (!System.IO.File.Exists(path))
                {
                    throw new Exception("File [" + path + "] in column [" + columnName + "] does not exist or is inaccessible;");
                }
            }
            catch (Exception)
            {
                throw;
            }
        }




        private List<string> getArguments(RecordData recordDataIn)
        {
            string[] arrSelectedCols = new string[0];

            bool bAllColsSelected = 
                String.Equals(_selectedCols, Constants.DEFAULTSELECTEDCOLS, StringComparison.OrdinalIgnoreCase);

            if (_selectedCols != Constants.ZEROSELECTEDCOLS && !bAllColsSelected)            
                arrSelectedCols = _selectedCols.Split(',');

            List<string> items = new List<string>();

            // loop through the inbound records, build a string
            uint countFields = _recordInfoIn.NumFields();
            for (int i = 0; i < countFields; ++i)
            {
                FieldBase fbIn = _recordInfoIn[i];
                string fbColumnName = fbIn.GetFieldName();
                string fbData = "";

                // Do not include the executable in the arguments
                if (!(String.Equals(fbColumnName, _exePathField, StringComparison.OrdinalIgnoreCase)))
                {
                    // If the fbColumnName matches a selected column name, include it as a parameter
                    if (bAllColsSelected || arrSelectedCols.Contains(fbColumnName))
                    { 
                        try
                        {
                            fbData = fbIn.GetAsString(recordDataIn) ?? "";
                        }
                        catch (NullReferenceException)
                        {
                            // If there is no data, catch and write out an empty string
                            fbData = "";
                        }

                        items.Add(fbData);
                    }
                }
            }

            return items;
        }


        private void shell(string exePath, List<string> args)
        {
            CSharpTest.Net.Processes.ProcessRunner procRunner = 
                new CSharpTest.Net.Processes.ProcessRunner(isTrueString(_autoEscape), exePath, args.ToArray());

            _cmdLine = procRunner.ToString(); // What will be spawned

            procRunner.OutputReceived += new ProcessOutputEventHandler(run_OutputReceived);
            _retCode = procRunner.Run().ToString();
        }


        private void run_OutputReceived(object sender, ProcessOutputEventArgs args)
        {
            if (args.Error)
                lock (_sbExceptionsLock) { _sbExceptions.AppendLine(args.Data); }            
            else
                lock (_sbStdOutLock) { _sbStdOut.AppendLine(args.Data); }            
        }


        public static bool isTrueString(string target)
        {
            string cleanTarget = target.Trim().ToUpper();
            switch(cleanTarget)
            {
                case "Y":
                case "TRUE":
                case "1":
                    return true;
                default:
                    break;
            }
            return false;
        }
    }
}
