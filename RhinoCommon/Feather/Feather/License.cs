using Rhino;
using Rhino.Commands;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Feather
{
    internal class License
    {
        public static void Verify()
        {
            try
            {
                // Prepare arguments as text fields.
                string args = "";
                args += "license";

                Helper.RunLogic("Cotton.exe", args, PostProcess);
            }

            catch (Exception ex)
            {
                RhinoApp.WriteLine("Error on run command: {0}", ex.Message);
            }
        }

        private static void PostProcess(object sender, EventArgs e)
        {
            try
            {
                Process process = sender as Process;
                if (process != null)
                {
                    int test = process.ExitCode;
                    if (test == -1)
                    {
                        throw new Exception("No valid license found, please visit: https://www.patreon.com/Megidd/shop");
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
