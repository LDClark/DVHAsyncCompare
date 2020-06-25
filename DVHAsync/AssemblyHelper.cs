using System.IO;

namespace DVHAsync
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
