namespace Feather
{
    internal class Permit
    {
        public static bool Verify()
        {
            // Prepare arguments as text fields.
            string args = "";
            args += "permit";

            int exitCode = Helper.RunLogicWithLogAndWait(Paths.cotton, args);
            return exitCode == 0;
        }
    }
}
