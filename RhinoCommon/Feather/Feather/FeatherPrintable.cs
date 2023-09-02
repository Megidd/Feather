using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Rhino;
using Rhino.Commands;
using Rhino.DocObjects;
using Rhino.Geometry;
using static Feather.Helper;

namespace Feather
{
    public class FeatherPrintable : Command
    {
        public FeatherPrintable()
        {
            Instance = this;
        }

        ///<summary>The only instance of the MyCommand command.</summary>
        public static FeatherPrintable Instance { get; private set; }

        public override string EnglishName => "FeatherPrintable";

        private static RhinoDoc docCurrent; // Accessed by async post-process code.
        private static RhinoObject inObj = null; // Input object.
        private static string inPath = Path.GetTempPath() + "input.stl"; // Input object to be saved as STL.
        private static string resultPath = Path.GetTempPath() + "result.inp"; // Consumable by FEA.
        private static string resultInfoPath = Path.GetTempPath() + "result-info.json"; // Info & details.

        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            docCurrent = doc; // Accessed by async post-process code.

            string message = string.Format("Document model unit system is set to {0}. It will affect the result. Is {0} unit acceptable?", doc.ModelUnitSystem.ToString().ToLower());
            bool isUnitAcceptable = Helper.GetYesNoFromUser(message);
            if (!isUnitAcceptable)
            {
                RhinoApp.WriteLine("Please fix the document model unit system and re-run this command.");
                return Result.Failure;
            }

            inObj = Helper.GetInputStl(doc.ModelUnitSystem, inPath);
            if (inObj == null)
            {
                return Result.Failure;
            }

            double MassDensity = 7.85e-9;
            double YoungModulus = 210000;
            double PoissonRatio = 0.3;

            uint materialProps = Helper.GetUint32FromUser("3D print resin material type? Very soft=1, Soft=2, Medium=3, Hard=4, Very hard=5", 3, 1, 5);
            switch (materialProps)
            {
                case 1:
                    MassDensity = 7.85e-9; // TODO: Actual value.
                    YoungModulus = 210000;
                    PoissonRatio = 0.3;
                    break;
                case 2:
                    MassDensity = 7.85e-9;
                    YoungModulus = 210000;
                    PoissonRatio = 0.3;
                    break;
                case 3:
                    MassDensity = 7.85e-9;
                    YoungModulus = 210000;
                    PoissonRatio = 0.3;
                    break;
                case 4:
                    MassDensity = 7.85e-9;
                    YoungModulus = 210000;
                    PoissonRatio = 0.3;
                    break;
                case 5:
                    MassDensity = 7.85e-9;
                    YoungModulus = 210000;
                    PoissonRatio = 0.3;
                    break;
                default:
                    RhinoApp.WriteLine("It's out of range");
                    return Result.Failure;
            }

            // Resolution is voxel (3D pixel) count on longest axis of 3D model AABB.
            // NOTE: It will be further calibrated by the logic. Don't worry about it.
            uint resolution = 30;

            uint precision = Helper.GetUint32FromUser("Enter precision of computation. It highly affects the speed. VeryLow=1, Low=2, Medium=3, High=4, VeryHigh=5", 3, 1, 5);
            switch (precision)
            {
                case 1:
                    resolution = 30;
                    break;
                case 2:
                    resolution = 60;
                    break;
                case 3:
                    resolution = 90;
                    break;
                case 4:
                    resolution = 120;
                    break;
                case 5:
                    resolution = 150;
                    break;
                default:
                    RhinoApp.WriteLine("Precision must be 1, 2, 3, 4, or 5 i.e. VeryLow=1, Low=2, Medium=3, High=4, VeryHigh=5");
                    return Result.Failure;
            }

            List<Point3d> restraintPoints = Helper.GetPointOnMesh(inObj, "Select sample points on mesh that are attached to print floor (Esc/Enter to finish)");
            if (restraintPoints == null || restraintPoints.Count < 1)
            {
                RhinoApp.WriteLine("No points are selected");
                return Result.Failure;
            }

            RhinoApp.WriteLine("Restraint points count: {0}", restraintPoints.Count);

            List<Restraint> restraints = new List<Restraint>();
            for (var i = 0; i < restraintPoints.Count; i++)
            {
                Restraint restraint = new Restraint();
                restraint.LocX = restraintPoints[i].X;
                restraint.LocY = restraintPoints[i].Y;
                restraint.LocZ = restraintPoints[i].Z;
                restraint.IsFixedX = true;
                restraint.IsFixedY = true;
                restraint.IsFixedZ = true;
                restraints.Add(restraint);
            }

            Dictionary<string, dynamic> specs = new Dictionary<string, dynamic>();
            specs.Add("MassDensity", MassDensity);
            specs.Add("YoungModulus", YoungModulus);
            specs.Add("PoissonRatio", PoissonRatio);
            specs.Add("GravityDirectionX", 0);
            specs.Add("GravityDirectionY", 0);
            specs.Add("GravityDirectionZ", +1); // 3D printing by SLA technology is done upside-down.
            specs.Add("GravityMagnitude", UnitConversion.Convert(9.810f, UnitSystem.Meters, Helper.unitOfStlFile));
            specs.Add("Resolution", resolution);
            specs.Add("LayersAllConsidered", true);
            specs.Add("LayerStart", -1);
            specs.Add("LayerEnd", -1);
            specs.Add("NonlinearConsidered", false);
            specs.Add("ExactSurfaceConsidered", true);
            specs.Add("ModelUnitSystem", doc.ModelUnitSystem.ToString());
            specs.Add("ModelUnitSystemOfSavedStlFile", Helper.unitOfStlFile.ToString());

            // Load is empty, since the gravity is the main load while 3D printing.
            List<Load> loads = new List<Load>();
            string loadPth = Path.GetTempPath() + "loads.json";
            string loadJson = JsonSerializer.Serialize(loads);
            File.WriteAllText(loadPth, loadJson);

            string restraintPth = Path.GetTempPath() + "restraints.json";
            string restraintJson = JsonSerializer.Serialize(restraints);
            File.WriteAllText(restraintPth, restraintJson);

            string specsPth = Path.GetTempPath() + "specs.json";
            string specsJson = JsonSerializer.Serialize(specs);
            File.WriteAllText(specsPth, specsJson);

            // Prepare arguments as text fields.
            string args = "";
            args += "printable";
            args += " ";
            args += inPath;
            args += " ";
            args += specsPth;
            args += " ";
            args += loadPth;
            args += " ";
            args += restraintPth;
            args += " ";
            args += resultPath;
            args += " ";
            args += resultInfoPath;

            Helper.RunLogic("Cotton.exe", args, PostProcess);

            RhinoApp.WriteLine("Process started. Please wait...");

            return Result.Success;
        }
        private static void PostProcess(object sender, EventArgs e)
        {
            try
            {
                RhinoApp.WriteLine("Post process started.");
                RhinoApp.WriteLine("Post process finished.");
            }
            catch (Exception ex)
            {
                RhinoApp.WriteLine("Error on post process: {0}", ex.Message);
            }
        }
    }
}