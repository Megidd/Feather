using System;
using Rhino;
using Rhino.Commands;

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
            // TODO: complete command.
            return Result.Success;
        }
    }
}