using System;
using System.Collections.Generic;
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

            string outPath = "output.stl";

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

            Helper.RunLogic(args);

            RhinoApp.WriteLine("{0} process is started. Please wait...", EnglishName);

            return Result.Success;
        }
    }
}