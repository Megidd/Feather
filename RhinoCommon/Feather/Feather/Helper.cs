using Rhino;
using Rhino.Commands;
using Rhino.DocObjects;
using Rhino.Geometry;
using Rhino.Input;
using Rhino.Input.Custom;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace Feather
{
    internal class Helper
    {
        public static Result GetInputStl(string filename= "mesh.stl")
        {
            RhinoObject obj = GetSingle();
            if (null == obj || obj.ObjectType != ObjectType.Mesh)
            {
                RhinoApp.WriteLine("Mesh is not valid.");
                return Result.Failure;
            }
            Mesh mesh = obj.Geometry as Mesh;
            if (mesh == null)
            {
                RhinoApp.WriteLine("Mesh is not valid.");
                return Result.Failure;
            }

            RhinoApp.WriteLine("Number of mesh vertices: {0}", mesh.Vertices.Count);
            RhinoApp.WriteLine("Number of mesh triangles: {0}", mesh.Faces.Count);

            SaveAsStl(mesh, filename);

            return Result.Success;
        }

        /// <summary>
        /// A simple method to prompt the user to select a single mesh.
        /// </summary>
        /// <returns>Selected object that should be of type mesh.</returns>
        public static RhinoObject GetSingle(String message = "Select a single mesh")
        {
            GetObject go = new GetObject();
            go.SetCommandPrompt(message);
            go.GeometryFilter = ObjectType.Mesh;
            go.Get();
            if (go.CommandResult() != Result.Success) return null;
            if (go.ObjectCount != 1) return null;
            RhinoObject obj = go.Object(0).Object();
            if (obj.ObjectType != ObjectType.Mesh) return null;
            return obj;
        }

        public static void SaveAsStl(Mesh mesh, string fileName)
        {
            // Extract vertex buffer and index buffer.
            float[] vertexBuffer;
            int[] indexBuffer;
            GetBuffers(mesh, out vertexBuffer, out indexBuffer);

            SaveBuffersAsStl(vertexBuffer, indexBuffer, fileName);
        }

        public static void GetBuffers(Mesh mesh, out float[] vertexBuffer, out int[] indexBuffer)
        {
            // Convert quads to triangles
            bool converted = mesh.Faces.ConvertQuadsToTriangles();
            if (converted)
            {
                RhinoApp.WriteLine("Mesh contains quads. They are converted to triangles.");
            }

            // Get vertex buffer
            Point3f[] vertices = mesh.Vertices.ToPoint3fArray();
            vertexBuffer = new float[vertices.Length * 3];
            for (int i = 0; i < vertices.Length; i++)
            {
                vertexBuffer[i * 3] = vertices[i].X;
                vertexBuffer[i * 3 + 1] = vertices[i].Y;
                vertexBuffer[i * 3 + 2] = vertices[i].Z;
            }

            // Get index buffer
            MeshFace[] faces = mesh.Faces.ToArray();
            indexBuffer = new int[faces.Length * 3];
            for (int i = 0; i < faces.Length; i++)
            {
                MeshFace face = faces[i];
                indexBuffer[i * 3] = face.A;
                indexBuffer[i * 3 + 1] = face.B;
                indexBuffer[i * 3 + 2] = face.C;
            }
        }

        public static void SaveBuffersAsStl(float[] vertexBuffer, int[] indexBuffer, string fileName)
        {
            // Open the file for writing
            using (FileStream fileStream = new FileStream(fileName, FileMode.Create))
            {
                // Write the STL header
                byte[] header = new byte[80];
                fileStream.Write(header, 0, header.Length);

                // Write the number of triangles
                int triangleCount = indexBuffer.Length / 3;
                byte[] triangleCountBytes = BitConverter.GetBytes(triangleCount);
                fileStream.Write(triangleCountBytes, 0, 4);

                // Write the triangles
                for (int i = 0; i < indexBuffer.Length; i += 3)
                {
                    // Get vertices for the current triangle
                    float x1 = vertexBuffer[indexBuffer[i] * 3];
                    float y1 = vertexBuffer[indexBuffer[i] * 3 + 1];
                    float z1 = vertexBuffer[indexBuffer[i] * 3 + 2];
                    float x2 = vertexBuffer[indexBuffer[i + 1] * 3];
                    float y2 = vertexBuffer[indexBuffer[i + 1] * 3 + 1];
                    float z2 = vertexBuffer[indexBuffer[i + 1] * 3 + 2];
                    float x3 = vertexBuffer[indexBuffer[i + 2] * 3];
                    float y3 = vertexBuffer[indexBuffer[i + 2] * 3 + 1];
                    float z3 = vertexBuffer[indexBuffer[i + 2] * 3 + 2];

                    // Compute the normal vector of the triangle
                    float nx = (y2 - y1) * (z3 - z1) - (z2 - z1) * (y3 - y1);
                    float ny = (z2 - z1) * (x3 - x1) - (x2 - x1) * (z3 - z1);
                    float nz = (x2 - x1) * (y3 - y1) - (y2 - y1) * (x3 - x1);
                    float length = (float)Math.Sqrt(nx * nx + ny * ny + nz * nz);
                    nx /= length;
                    ny /= length;
                    nz /= length;

                    // Write the normal vector
                    byte[] normal = new byte[12];
                    BitConverter.GetBytes(nx).CopyTo(normal, 0);
                    BitConverter.GetBytes(ny).CopyTo(normal, 4);
                    BitConverter.GetBytes(nz).CopyTo(normal, 8);
                    fileStream.Write(normal, 0, normal.Length);

                    // Write the vertices in counter-clockwise order
                    byte[] triangle = new byte[36];
                    BitConverter.GetBytes(x1).CopyTo(triangle, 12);
                    BitConverter.GetBytes(y1).CopyTo(triangle, 16);
                    BitConverter.GetBytes(z1).CopyTo(triangle, 20);
                    BitConverter.GetBytes(x3).CopyTo(triangle, 0);
                    BitConverter.GetBytes(y3).CopyTo(triangle, 4);
                    BitConverter.GetBytes(z3).CopyTo(triangle, 8);
                    BitConverter.GetBytes(x2).CopyTo(triangle, 24);
                    BitConverter.GetBytes(y2).CopyTo(triangle, 28);
                    BitConverter.GetBytes(z2).CopyTo(triangle, 32);
                    fileStream.Write(triangle, 0, triangle.Length);

                    // Write the triangle attribute (zero)
                    byte[] attribute = new byte[2];
                    fileStream.Write(attribute, 0, attribute.Length);
                }
            }
        }


        public static float GetFloatFromUser(double defaultValue, double lowerLimit, double upperLimit, string message)
        {
            // Create a GetNumber object
            GetNumber numberGetter = new GetNumber();
            numberGetter.SetLowerLimit(lowerLimit, false);
            numberGetter.SetUpperLimit(upperLimit, false);
            numberGetter.SetDefaultNumber(defaultValue);
            numberGetter.SetCommandPrompt(message);

            // Prompt the user to enter a number
            GetResult result = numberGetter.Get();

            // Check if the user entered a number
            switch (result)
            {
                case GetResult.Number:
                    break;
                default:
                    return Convert.ToSingle(defaultValue);
            }

            // Get the number entered by the user
            double number = numberGetter.Number();

            return Convert.ToSingle(number);
        }


        public static uint GetUint32FromUser(string prompt, uint defaultValue, uint lowerLimit, uint upperLimit)
        {
            double doubleResult = defaultValue;
            uint result = defaultValue;
            while (true)
            {
                var getNumberResult = RhinoGet.GetNumber(prompt, false, ref doubleResult, lowerLimit, upperLimit);
                if (getNumberResult == Result.Cancel)
                {
                    RhinoApp.WriteLine("Canceled by user.");
                    return defaultValue;
                }
                else if (getNumberResult == Result.Success)
                {
                    result = (uint)doubleResult;
                    if (result < lowerLimit || result > upperLimit)
                        RhinoApp.WriteLine("Input out of range.");
                    else
                        return result;
                }
                else
                    RhinoApp.WriteLine("Invalid input.");
            }
        }

        public static bool GetYesNoFromUser(string prompt)
        {
            bool boolResult = false;
            while (true)
            {
                var getBoolResult = RhinoGet.GetBool(prompt, true, "No", "Yes", ref boolResult);
                if (getBoolResult == Result.Cancel)
                    RhinoApp.WriteLine("Canceled by user.");
                else if (getBoolResult == Result.Success)
                    return boolResult;
                else
                    RhinoApp.WriteLine("Invalid input.");
            }
        }

        private static StringBuilder output;
        private static Process cmd;

        public static void RunLogic(string args)
        {
        output = new StringBuilder();
        cmd = new Process();

            try
            {
            cmd.StartInfo.FileName = "Cotton.exe";
            cmd.StartInfo.Arguments = args;
            cmd.StartInfo.UseShellExecute = false;
            cmd.StartInfo.CreateNoWindow = true;
            cmd.StartInfo.RedirectStandardOutput = true;
            cmd.StartInfo.RedirectStandardInput = true;

            cmd.EnableRaisingEvents = true;
            cmd.OutputDataReceived +=
               new DataReceivedEventHandler(cmd_OutputDataReceived);
            cmd.Exited += new EventHandler(cmd_Exited);

            cmd.Start();
            }

            catch (Exception ex)
            {
                RhinoApp.WriteLine("Error on process start: " + ex.Message);
            }
        }

        private static void cmd_Exited(object sender, EventArgs e)
        {
            RhinoApp.WriteLine("Process output: {0}", output.ToString());
            cmd.Dispose();
        }

        private static void cmd_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (!String.IsNullOrEmpty(e.Data))
            {
                output.Append(e.Data + Environment.NewLine);
            }
        }
    }
}
