using System;
using System.IO;
using Rhino;
using Rhino.Commands;

namespace Feather
{
    public class FeatherHollow : Command
    {
        public FeatherHollow()
        {
            Instance = this;
        }

        ///<summary>The only instance of the MyCommand command.</summary>
        public static FeatherHollow Instance { get; private set; }

        public override string EnglishName => "FeatherHollow";

        private static string outPath = "output.stl";

        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            string inPath = "input.stl";

            if (Helper.GetInputStl(inPath) == Result.Failure)
            {
                return Result.Failure;
            }

            bool infill = Helper.GetYesNoFromUser("Do you want infill for hollowed mesh?");

            float thickness = Helper.GetFloatFromUser(1.8, 0.0, 100.0, "Enter wall thickness for hollowing.");

            uint precision = Helper.GetUint32FromUser("Enter precision: Low=1, Medium=2, High=3", 2, 1, 3);
            switch (precision)
            {
                case 1:
                case 2:
                case 3:
                    break;
                default:
                    RhinoApp.WriteLine("Precision must be 1, 2, or 3 i.e. Low=1, Medium=2, High=3");
                    return Result.Failure;
            }

            // Prepare arguments as text fields.
            string args = inPath;
            args += " ";
            args += infill ? "true" : "false";
            args += " ";
            args += thickness.ToString();
            args += " ";
            args += precision.ToString();
            args += " ";
            args += outPath;

            Helper.RunLogic(args, PostProcess);

            RhinoApp.WriteLine("Process is started. Please wait...");

            return Result.Success;
        }

        private static void PostProcess(object sender, EventArgs e)
        {
            try
            {
                RhinoApp.WriteLine("Post processing of output path {0} is started.", outPath);
                String ext = Path.GetExtension(outPath);
                if (null == ext || !ext.ToLower().Equals(".stl", StringComparison.OrdinalIgnoreCase))
                {
                    RhinoApp.WriteLine("Post process: file type must be STL:", outPath);
                    return;
                }

                String script = String.Format("_-Import \"{0}\" _Enter", outPath);
                bool good = RhinoApp.RunScript(script, false);
                if (!good)
                {
                    RhinoApp.WriteLine("Post process: output file cannot be imported: {0}", outPath);
                }
            }

            catch (Exception ex)
            {
                RhinoApp.WriteLine("Error on post process: {0}", ex.Message);
            }
        }
    }
}