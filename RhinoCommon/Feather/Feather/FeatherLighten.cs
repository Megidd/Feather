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

        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            docCurrent = doc; // Accessed by async post-process code.
            inObj = Helper.GetInputStl(inPath);
            if (inObj == null)
            {
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

            // Load magnitude will be estimated by the 3D model weight and impact velocity.
            //Double loadMagnitude = Helper.GetDoubleFromUser();

            RhinoApp.WriteLine("Load/force points count: {0}", loadPoints.Count);

            List<Point3d> restraintPoints = Helper.GetPointOnMesh(inObj, "Select restraint points on mesh (Esc to cancel)");
            if (restraintPoints == null || restraintPoints.Count < 1)
            {
                RhinoApp.WriteLine("No points are selected");
                return Result.Failure;
            }

            RhinoApp.WriteLine("Restraint points count: {0}", restraintPoints.Count);

            var specs = new Dictionary<string, double>
            {
                ["massDensity"] = 7.85e-9,
                ["youngModulus"] = 210000,
                ["poissonRatio"] = 0.3,
                ["gravityConsidered"] = 1, // 0 or 1
                ["gravityDirectionX"] = 0,
                ["gravityDirectionY"] = 0,
                ["gravityDirectionZ"] = -1,
                ["gravityMagnitude"] = 9810,
                ["resolution"] = 50,
                ["layerStart"] = -1, // Negative means all layers.
                ["layerEnd"] = -1, // Negative means all layers.
                ["Tet4"] = 0,
                ["Tet10"] = 0,
                ["Hex8"] = 0,
                ["Hex20"] = 0,
                ["Hex8Tet4"] = 1,
                ["Hex20Tet10"] = 0,
            };

            string loadPth = Path.GetTempPath() + "loadPoints.json";
            string loadJson = JsonSerializer.Serialize(loadPoints);
            File.WriteAllText(loadPth, loadJson);

            string loadNormalsPth = Path.GetTempPath() + "loadNormals.json";
            string loadNormalsJson = JsonSerializer.Serialize(loadNormals);
            File.WriteAllText(loadNormalsPth, loadNormalsJson);

            string restraintPth = Path.GetTempPath() + "restraintPoints.json";
            string restraintJson = JsonSerializer.Serialize(restraintPoints);
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
            args += loadPth;
            args += " ";
            args += loadNormalsJson;
            args += " ";
            args += restraintPth;
            args += " ";
            args += specsPth;

            Helper.RunLogic(args, PostProcess);

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
