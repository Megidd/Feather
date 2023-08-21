using System;
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
            string inputStl = "input.stl";

            if (Helper.GetInputStl(inputStl) == Result.Failure)
            {
                return Result.Failure;
            }

            return Result.Success;
        }
    }
}