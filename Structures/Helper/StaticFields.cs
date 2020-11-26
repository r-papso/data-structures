using System;
using System.IO;

namespace Structures.Helper
{
    internal static class StaticFields
    {
        public static string AppData = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Structures");

        public static string ExtendibleHashingData = Path.Combine(AppData, "Data", "extendible_hashing_data.bin");

        public static string ExtendibleHashingHeader = Path.Combine(AppData, "Data", "extendible_hashing_header.bin");

        public static string OverflowFileData = Path.Combine(AppData, "Data", "overflow_data.bin");

        public static string OverflowFileHeader = Path.Combine(AppData, "Data", "overflow_header.bin");

        public static string LogFile = Path.Combine(AppData, "Log", "log.txt");
    }
}
