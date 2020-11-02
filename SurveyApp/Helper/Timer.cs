using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace SurveyApp.Helper
{
    /// <summary>
    /// Provides functionality used for methods execution time measurement
    /// </summary>
    public class Timer : INotifyPropertyChanged
    {
        private static object _lock = new object();
        private static volatile Timer _instance;

        private Stopwatch _stopwatch = new Stopwatch();

        protected Timer() { }

        /// <summary>
        /// Instance of the <see cref="Timer"/> class
        /// </summary>
        public static Timer Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new Timer();
                        }
                    }
                }
                return _instance;
            }
        }

        /// <summary>
        /// Execution time of last measured execution
        /// </summary>
        public long ElapsedMilliseconds => _stopwatch.ElapsedMilliseconds;

        /// <summary>
        /// Invoked when <see cref="ElapsedMilliseconds"/> property changes
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Starts new execution measurement
        /// </summary>
        public void Start()
        {
            _stopwatch.Reset();
            OnPropertyChanged(nameof(ElapsedMilliseconds));
            _stopwatch.Start();

        }

        /// <summary>
        /// Stops execution measurement
        /// </summary>
        public void Stop()
        {
            _stopwatch.Stop();
            OnPropertyChanged(nameof(ElapsedMilliseconds));
        }

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
