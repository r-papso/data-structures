using System;
using System.IO;

namespace Structures.Helper
{
    internal static class StaticFields
    {
        public static string AppData = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Structures");

        public static string ExtendibleHashingData = Path.Combine(AppData, "Data", "data.bin");

        public static string ExtendibleHashingHeader = Path.Combine(AppData, "Data", "header.bin");

        public static string OverflowFile = Path.Combine(AppData, "Data", "overflow.bin");

        public static string LogFile = Path.Combine(AppData, "Log", "log.txt");
    }
}
