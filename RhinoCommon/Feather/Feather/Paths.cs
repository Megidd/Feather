using System;
using System.IO;
using System.Reflection;

namespace Feather
{
    internal class Paths
    {
        public static string AssemblyDirectory
        {
            get
            {
                string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }

        // Input object to be saved as STL.
        // Material props are all based on mm, so STL unit would be converted to mm.
        public static string stl = Path.GetTempPath() + "input.stl";

        public static string specs = Path.GetTempPath() + "specs.json";

        public static string cotton = Path.Combine(AssemblyDirectory, "Cotton.exe");
    }
}
