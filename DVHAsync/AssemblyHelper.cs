using System.IO;

namespace DVHAsyncCompare
{
    public static class AssemblyHelper
    {
        public static string GetAssemblyDirectory()
        {
            return Path.GetDirectoryName(GetAssemblyPath());
        }

        public static string GetAssemblyPath()
        {
            return System.Reflection.Assembly.GetExecutingAssembly().Location;
        }
    }
}
