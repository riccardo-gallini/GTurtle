using System;
using System.Threading.Tasks;

namespace GScripting.SimpleIDE
{
    public class ParserService
    {
        private Func<string> getSource;
        private Action<ParseInfo> parseFinished;
        private Engine _engine;

        private System.Threading.Timer timer;
        private Task lastParseRun;
        private string _sourceSnapshot = "";
                
        public int ParseFrequency { get; set; }
        
        public void RegisterGetSource(Func<string> func)
        {
            getSource = func;
        }

        public void RegisterOnParseFinished(Action<ParseInfo> action)
        {
            parseFinished = action;
        }

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
            if (parseFinished==null || getSource==null) { return; }

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

            var source = getSource();

            if (source != _sourceSnapshot)
            {
                _sourceSnapshot = source;

                lastParseRun = Task.Run(() => _engine.Parse(_sourceSnapshot))
                                   .ContinueWith((bgTask) => parseFinished(bgTask.Result));
            }
        }

    }
}
