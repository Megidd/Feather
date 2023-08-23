using Rhino;
using Rhino.Commands;
using Rhino.DocObjects;

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

        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            // Behavior of the command.

            string inputStl = "input.stl";

            RhinoObject obj = Helper.GetInputStl(inputStl);
            if (obj == null)
            {
                return Result.Failure;
            }

            return Result.Success;
        }
    }
}
