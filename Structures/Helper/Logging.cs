using Serilog;

namespace Structures.Helper
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
                            _instance = new LoggerConfiguration().WriteTo.File(StaticFields.LogFile, rollingInterval: RollingInterval.Day).CreateLogger();
                        }
                    }
                }
                return _instance;
            }
        }
    }
}
