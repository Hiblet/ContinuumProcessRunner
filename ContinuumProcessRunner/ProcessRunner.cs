using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AlteryxGuiToolkit.Plugins;

namespace ContinuumProcessRunner
{
    public class ProcessRunner : IPlugin
    {

        private System.Drawing.Bitmap _icon;
        private string _iconResource = "ContinuumProcessRunner.Images.ProcessRunner_171.png";

        public IPluginConfiguration GetConfigurationGui()
        {
            return new ProcessRunnerUserControl();
        }

        public EntryPoint GetEngineEntryPoint()
        {
            var entryPoint = new AlteryxGuiToolkit.Plugins.EntryPoint("ContinuumProcessRunner.dll", "ContinuumProcessRunner.ProcessRunnerNetPlugin", true);
            return entryPoint;
        }

        public System.Drawing.Image GetIcon()
        {
            // DIAG
            // To see the actual name of the embedded resource, as the assembly sees it.
            var arrResources = typeof(ProcessRunner).Assembly.GetManifestResourceNames();
            // END DIAG

            if (_icon == null)
            {
                System.IO.Stream s = typeof(ProcessRunner).Assembly.GetManifestResourceStream(_iconResource);
                if (s == null)
                {
                    throw new ArgumentNullException("Could not find local resource [" + _iconResource + "]");
                }

                _icon = (System.Drawing.Bitmap)System.Drawing.Bitmap.FromStream(s);
                _icon.MakeTransparent();
            }

            return _icon;
        }

        public Connection[] GetInputConnections()
        {
            return new AlteryxGuiToolkit.Plugins.Connection[] { new AlteryxGuiToolkit.Plugins.Connection("Input") };
        }

        public Connection[] GetOutputConnections()
        {
            return new AlteryxGuiToolkit.Plugins.Connection[] { new AlteryxGuiToolkit.Plugins.Connection("Output") };
        }


    }
}
