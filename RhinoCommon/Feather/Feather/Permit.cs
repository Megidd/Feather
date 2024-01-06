namespace Feather
{
    internal class Permit
    {
        public static bool Verify()
        {
            // Prepare arguments as text fields.
            string args = "";
            args += "permit";

            int exitCode = Helper.RunLogicWithLogAndWait("Cotton.exe", args);
            return exitCode == 0;
        }
    }
}
