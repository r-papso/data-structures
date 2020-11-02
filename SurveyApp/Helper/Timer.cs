using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace SurveyApp.Helper
{
    public class Timer : INotifyPropertyChanged
    {
        private static object _lock = new object();
        private static volatile Timer _instance;

        private Stopwatch _stopwatch = new Stopwatch();

        protected Timer() { }

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

        public long ElapsedMilliseconds => _stopwatch.ElapsedMilliseconds;

        public event PropertyChangedEventHandler PropertyChanged;

        public void Start()
        {
            _stopwatch.Reset();
            OnPropertyChanged(nameof(ElapsedMilliseconds));
            _stopwatch.Start();

        }

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
