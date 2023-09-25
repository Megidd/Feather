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
            // Prepare arguments as text fields.
            string args = "";
            args += "license";

            Helper.RunLogic("Cotton.exe", args, PostProcess);
        }

        private static void PostProcess(object sender, EventArgs e)
        {
            Process process = sender as Process;
            if (process != null)
            {
                // https://stackoverflow.com/a/26002230/3405291
                int test = process.ExitCode;
                if (test == -1)
                {
                    // https://stackoverflow.com/a/4542090/3405291
                    throw new Exception("No valid license found, please visit: https://www.patreon.com/Megidd/shop");
                }
            }
        }
    }
}
