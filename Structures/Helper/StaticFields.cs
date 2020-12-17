using System;
using System.IO;

namespace Structures.Helper
{
    internal static class StaticFields
    {
        public static readonly string AppData = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Structures");

        public static readonly string LogFile = Path.Combine(AppData, "Log", "log.txt");

        public static readonly string DirectoryFileName = "directory.csv";

        public static readonly string DataFileName = "primary_file_data.bin";

        public static readonly string DataHeaderName = "primary_file_header.bin";

        public static readonly string OverflowFileName = "overflow_file_data.bin";

        public static readonly string OverflowHeaderName = "overflow_file_header.bin";

        public static readonly string HashingProtKey = "extendible_hashing_prototype";
    }
}
