using Rhino.Commands;
using Rhino.DocObjects;
using Rhino.Input.Custom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Feather
{
    internal class Helper
    {
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

    }
}
