using Serilog;
using System;
using System.IO;

namespace Structures.Hepler
{
    internal static class Logging
    {
        private static object _lock = new object();
        private static volatile ILogger _instance;

        public static ILogger Logger
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Structures", "Log", "log.txt");
                            _instance = new LoggerConfiguration().WriteTo.File(path, rollingInterval: RollingInterval.Day).CreateLogger();
                        }
                    }
                }
                return _instance;
            }
        }
    }
}
