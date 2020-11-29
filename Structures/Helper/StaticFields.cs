using System;
using System.IO;

namespace Structures.Helper
{
    internal static class StaticFields
    {
        public static string AppData = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Structures");

        public static string LogFile = Path.Combine(AppData, "Log", "log.txt");
    }
}
