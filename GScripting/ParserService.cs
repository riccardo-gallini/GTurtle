using System;
using System.Threading.Tasks;

namespace GScripting
{
    public class ParserService
    {
        public Func<string> GetSource;
        public Action<ParseInfo> ParseFinished;
        private Engine _engine;

        private System.Threading.Timer timer;
        private Task lastParseRun;
        private string _sourceSnapshot = "";
                
        public int ParseFrequency { get; set; }
        
        internal ParserService(Engine Engine)
        {
            _engine = Engine;
            ParseFrequency = 2500; //default
        }

        private bool _isRunning = false;
        public bool IsRunning
        {
            get
            {
                return _isRunning;
            }
        }
        
        public void Start()
        {
            if (ParseFinished==null || GetSource==null) { return; }

            if (!_isRunning)
            {
                timer = new System.Threading.Timer(timer_tick, null, ParseFrequency, ParseFrequency);
                _isRunning = true;
            }
        }

        public void Stop()
        {
            if (_isRunning)
            {
                timer.Change(System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);
                _isRunning = false;
            }
        }

        private void timer_tick(object state)
        {
            if (lastParseRun != null)
            {
                // don't start another parse run if the last one is still running
                if (!lastParseRun.IsCompleted)
                    return;
            }
            lastParseRun = null;

            var source = GetSource();

            if (source != _sourceSnapshot)
            {
                _sourceSnapshot = source;

                lastParseRun = Task.Run(() => _engine.Parse(_sourceSnapshot))
                                   .ContinueWith((bgTask) => ParseFinished(bgTask.Result));
            }
        }

    }
}
