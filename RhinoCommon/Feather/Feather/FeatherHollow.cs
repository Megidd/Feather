using System;
using System.IO;
using Rhino;
using Rhino.Commands;
using Rhino.DocObjects;
using Rhino.Geometry;

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

        private static RhinoDoc docCurrent; // Accessed by async post-process code.
        private static RhinoObject inObj = null; // Input object.
        private static string inPath = Path.GetTempPath() + "input.stl"; // Input object to be saved as STL.
        private static string outPath = Path.GetTempPath() + "output.stl"; // Output STL saved by logic.

        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            docCurrent = doc; // Accessed by async post-process code.
            inObj = Helper.GetInputStl(inPath);
            if (inObj == null)
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
            string args = "";
            args += "hollow";
            args += " ";
            args += inPath;
            args += " ";
            args += infill ? "true" : "false";
            args += " ";
            args += thickness.ToString();
            args += " ";
            args += precision.ToString();
            args += " ";
            args += outPath;

            Helper.RunLogic("Cotton.exe", args, PostProcess);

            RhinoApp.WriteLine("Process is started. Please wait...");

            return Result.Success;
        }

        private static void PostProcess(object sender, EventArgs e)
        {
            try
            {
                RhinoApp.WriteLine("Post process started for {0}", outPath);
                Mesh meshOut = Helper.LoadStlAsMesh(outPath);

                // Run the CheckValidity method on the mesh.
                MeshCheckParameters parameters = new MeshCheckParameters();
                // Enable all the checks:
                parameters.CheckForBadNormals = true;
                parameters.CheckForDegenerateFaces = true;
                parameters.CheckForDisjointMeshes = true;
                parameters.CheckForDuplicateFaces = true;
                parameters.CheckForExtremelyShortEdges = true;
                parameters.CheckForInvalidNgons = true;
                parameters.CheckForNakedEdges = true;
                parameters.CheckForNonManifoldEdges = true;
                parameters.CheckForRandomFaceNormals = true;
                parameters.CheckForSelfIntersection = true;
                parameters.CheckForUnusedVertices = true;
                Rhino.FileIO.TextLog log = new Rhino.FileIO.TextLog(Path.GetTempPath() + "mesh-checks.txt");
                bool isValid = meshOut.Check(log, ref parameters);
                RhinoApp.WriteLine("Is output mesh valid? {0}", isValid);
                bool hasInvalidVertexIndices = Helper.HasInvalidVertexIndices(meshOut);
                RhinoApp.WriteLine("Does output mesh have invalid vertex indices? {0}", hasInvalidVertexIndices);
                if (!isValid || hasInvalidVertexIndices)
                {
                    RhinoApp.WriteLine("Total cound of disjoint meshes: {0}", parameters.DisjointMeshCount);
                    RhinoApp.WriteLine("Output mesh cannot be added to scene due to being invalid");
                }
                else
                {
                    // If the mesh is valid, add it to the document.

                    // Create a new object attributes with the desired name
                    ObjectAttributes attributes = new ObjectAttributes();
                    attributes.Name = "Hollowed: " + inObj.Attributes.Name;

                    // Add the mesh to the document with the specified attributes
                    docCurrent.Objects.AddMesh(meshOut, attributes);

                    // Redraw the viewports to update the display
                    docCurrent.Views.Redraw();

                    if (docCurrent.Objects.Delete(inObj.Id, true))
                    {
                        // Good.
                    }
                    else
                    {
                        RhinoApp.WriteLine("Post process couldn't delete the original object.");
                    }
                }

                RhinoApp.WriteLine("Post process finished.");
            }

            catch (Exception ex)
            {
                RhinoApp.WriteLine("Error on post process: {0}", ex.Message);
            }
        }
    }
}