using Rhino;
using Rhino.Commands;
using Rhino.DocObjects;
using System.IO;
using System;
using Rhino.Geometry;
using System.Collections.Generic;
using System.Text.Json;

namespace Feather
{
    public class FeatherLighten : Command
    {
        public FeatherLighten()
        {
            // Rhino only creates one instance of each command class defined in a
            // plug-in, so it is safe to store a refence in a static property.
            Instance = this;
        }

        ///<summary>The only instance of this command.</summary>
        public static FeatherLighten Instance { get; private set; }

        ///<returns>The command name as it appears on the Rhino command line.</returns>
        public override string EnglishName => "FeatherLighten";

        private static RhinoDoc docCurrent; // Accessed by async post-process code.
        private static RhinoObject inObj = null; // Input object.
        private static string inPath = Path.GetTempPath() + "input.stl"; // Input object to be saved as STL.
        private static string resultPath = Path.GetTempPath() + "result.inp"; // Consumable by FEA.

        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            docCurrent = doc; // Accessed by async post-process code.
            inObj = Helper.GetInputStl(inPath);
            if (inObj == null)
            {
                return Result.Failure;
            }

            // TODO: Load magnitude will be estimated by the 3D model weight and impact velocity.
            // Unit of measurement is Newton (N).
            Double loadMagnitude = 0;

            // https://en.wikipedia.org/wiki/List_of_jewellery_types
            uint jewelryType = Helper.GetUint32FromUser("Enter jewelry type. Crown=1, Necklace=2, Bracelet/Armlet/Anklet=3, Ring=4, Earring=5, Belly/waist=6, Piercing=7, Other=8", 3, 1, 8);
            switch (jewelryType)
            {
                case 1:
                    loadMagnitude = 800; // 80Kg or 800N i.e. average adult human weight
                    break;
                case 2:
                    loadMagnitude = 200;
                    break;
                case 3:
                    loadMagnitude = 200;
                    break;
                case 4:
                    loadMagnitude = 200;
                    break;
                case 5:
                    loadMagnitude = 200;
                    break;
                case 6:
                    loadMagnitude = 800;
                    break;
                case 7:
                    loadMagnitude = 200;
                    break;
                case 8:
                    loadMagnitude = 800;
                    break;
                default:
                    RhinoApp.WriteLine("It must be from 1 to 8");
                    return Result.Failure;
            }

            uint precision = Helper.GetUint32FromUser("Enter precision of computation. It highly affects the speed. VeryLow=1, Low=2, Medium=3, High=4, VeryHigh=5", 3, 1, 5);
            switch (precision)
            {
                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                    break;
                default:
                    RhinoApp.WriteLine("Precision must be 1, 2, 3, 4, or 5 i.e. VeryLow=1, Low=2, Medium=3, High=4, VeryHigh=5");
                    return Result.Failure;
            }

            List<Point3d> loadPoints = Helper.GetPointOnMesh(inObj, "Select load/force points on mesh (Esc to cancel)");
            if (loadPoints == null || loadPoints.Count < 1)
            {
                RhinoApp.WriteLine("No points are selected");
                return Result.Failure;
            }

            List<Vector3d> loadNormals = new List<Vector3d>();
            Mesh inMesh = inObj.Geometry as Mesh;
            for (var i = 0; i < loadPoints.Count; i++)
            {
                MeshPoint mp = inMesh.ClosestMeshPoint(loadPoints[i], 0.0);
                Vector3d normal = inMesh.NormalAt(mp);
                loadNormals.Add(normal);
            }

            RhinoApp.WriteLine("Load/force points count: {0}", loadPoints.Count);

            List<Load> loads = new List<Load>();
            for (var i = 0; i < loadPoints.Count; i++)
            {
                Load load = new Load();
                load.LocX = loadPoints[i].X;
                load.LocY = loadPoints[i].Y;
                load.LocZ = loadPoints[i].Z;
                bool good = loadNormals[i].Unitize();
                if (!good) RhinoApp.WriteLine("Warning: cannot normalize the load direction: {0}", loadNormals[i]);
                load.MagX = loadNormals[i].X * loadMagnitude;
                load.MagY = loadNormals[i].Y * loadMagnitude;
                load.MagZ = loadNormals[i].Z * loadMagnitude;
                loads.Add(load);
            }

            List<Point3d> restraintPoints = Helper.GetPointOnMesh(inObj, "Select restraint points on mesh (Esc to cancel)");
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
            specs.Add("MassDensity", 7.85e-9);
            specs.Add("YoungModulus", 210000);
            specs.Add("PoissonRatio", 0.3);
            specs.Add("GravityDirectionX", 0);
            specs.Add("GravityDirectionY", 0);
            specs.Add("GravityDirectionZ", -1);
            specs.Add("GravityMagnitude", 9810);
            specs.Add("Resolution", precision * 30.0); // Resolution is voxels count on longest axis of AABB. TODO: Calibrate?
            specs.Add("LayersAllConsidered", true);
            specs.Add("LayerStart", -1);
            specs.Add("LayerEnd", -1);
            specs.Add("NonlinearConsidered", false);
            specs.Add("ExactSurfaceConsidered", true);

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
            args += "lighten";
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

    public class Restraint
    {
        public double LocX { get; set; }
        public double LocY { get; set; }
        public double LocZ { get; set; }
        public bool IsFixedX { get; set; }
        public bool IsFixedY { get; set; }
        public bool IsFixedZ { get; set; }
    }

    public class Load
    {
        public double LocX { get; set; }
        public double LocY { get; set; }
        public double LocZ { get; set; }
        public double MagX { get; set; }
        public double MagY { get; set; }
        public double MagZ { get; set; }
    }
}
