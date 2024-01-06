using Rhino;
using Rhino.Commands;
using Rhino.DocObjects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

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

        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            try
            {
                bool permitted = Permit.Verify();
                if (!permitted)
                {
                    RhinoApp.WriteLine("No valid license found, please visit: https://www.patreon.com/Megidd/shop");
                    return Result.Failure;
                }

                string PathStl = Path.GetTempPath() + "input.stl"; // Input object to be saved as STL.
                RhinoObject obj = Helper.GetInputStl(doc.ModelUnitSystem, PathStl);
                if (obj == null)
                {
                    return Result.Failure;
                }

                // Material props are all based on mm, so double-check that STL would be saved by mm.
                if (Helper.unitOfStlFile != UnitSystem.Millimeters)
                {
                    RhinoApp.WriteLine("Unit of STL file must be set to mm but it is {0}", Helper.unitOfStlFile.ToString().ToLower());
                    throw new Exception("unit of STL file must be set to mm");
                }

                // Resin properties:
                // https://3dprinting.stackexchange.com/a/21439/11091
                // Units of measurement:
                // https://engineering.stackexchange.com/q/54454/15178
                double MassDensity = 1130 * Math.Pow(10, -12); // (N*s2/mm4) // Assumed: 1.13 g/cm3
                double YoungModulus = 1.6 * 1000; // MPa (N/mm2)
                double PoissonRatio = 0.3;
                double TensileStrength = 38; // MPa (N/mm2)

                uint materialProps = Helper.GetUint32FromUser("3D print resin material type? Very soft=1, Soft=2, Medium=3, Hard=4, Very hard=5", 3, 1, 5);
                switch (materialProps)
                {
                    case 1:
                        // TODO: Adjust value.
                        MassDensity = 1130 * Math.Pow(10, -12); // (N*s2/mm4)
                        YoungModulus = 1.6 * 1000; // MPa (N/mm2)
                        PoissonRatio = 0.3;
                        TensileStrength = 38; // MPa (N/mm2)
                        break;
                    case 2:
                        // TODO: Adjust value.
                        MassDensity = 1130 * Math.Pow(10, -12); // (N*s2/mm4)
                        YoungModulus = 1.6 * 1000; // MPa (N/mm2)
                        PoissonRatio = 0.3;
                        TensileStrength = 38; // MPa (N/mm2)
                        break;
                    case 3:
                        // TODO: Adjust value.
                        MassDensity = 1130 * Math.Pow(10, -12); // (N*s2/mm4)
                        YoungModulus = 1.6 * 1000; // MPa (N/mm2)
                        PoissonRatio = 0.3;
                        TensileStrength = 38; // MPa (N/mm2)
                        break;
                    case 4:
                        // TODO: Adjust value.
                        MassDensity = 1130 * Math.Pow(10, -12); // (N*s2/mm4)
                        YoungModulus = 1.6 * 1000; // MPa (N/mm2)
                        PoissonRatio = 0.3;
                        TensileStrength = 38; // MPa (N/mm2)
                        break;
                    case 5:
                        // TODO: Adjust value.
                        MassDensity = 1130 * Math.Pow(10, -12); // (N*s2/mm4)
                        YoungModulus = 1.6 * 1000; // MPa (N/mm2)
                        PoissonRatio = 0.3;
                        TensileStrength = 38; // MPa (N/mm2)
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

                // Load is empty, since the gravity is the main load while 3D printing.
                // Restraint is empty, since first voxel layer on Z axis will be in contact with 3D print floor.

                RhinoApp.WriteLine("First voxel layer on Z axis is considered restraint i.e. in contact with 3D print floor.");

                Dictionary<string, dynamic> specs = new Dictionary<string, dynamic>();
                specs.Add("PathStl", PathStl);
                specs.Add("MassDensity", MassDensity); // (N*s2/mm4)
                specs.Add("YoungModulus", YoungModulus); // MPa (N/mm2)
                specs.Add("PoissonRatio", PoissonRatio);
                specs.Add("TensileStrength", TensileStrength); // MPa (N/mm2)
                specs.Add("GravityDirectionX", 0);
                specs.Add("GravityDirectionY", 0);
                specs.Add("GravityDirectionZ", +1); // 3D printing by SLA technology is done upside-down.
                specs.Add("GravityMagnitude", Unit.Convert(9.810f, UnitSystem.Meters, Helper.unitOfStlFile));
                specs.Add("Resolution", resolution);
                specs.Add("NonlinearConsidered", false);
                specs.Add("ExactSurfaceConsidered", true);
                specs.Add("ModelUnitSystem", doc.ModelUnitSystem.ToString());
                specs.Add("ModelUnitSystemOfSavedStlFile", Helper.unitOfStlFile.ToString());

                string specsPth = Path.GetTempPath() + "specs.json";
                string specsJson = JsonSerializer.Serialize(specs);
                File.WriteAllText(specsPth, specsJson);

                // Prepare arguments as text fields.
                string args = "";
                args += "printable";
                args += " ";
                args += specsPth;

                Helper.RunLogic("Cotton.exe", args, PostProcess);

                RhinoApp.WriteLine("Process started. Please wait...");

                return Result.Success;
            }

            catch (Exception ex)
            {
                RhinoApp.WriteLine("Error on run command: {0}", ex.Message);
                return Result.Failure;
            }
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
