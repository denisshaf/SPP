using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tracer.Models
{
    internal class ThreadTrace
    {
        internal int Id;
        internal long TotalTime { get; private set; }
        internal int Depth => StackFrames.Count;
        internal Stack<ReadOnlyStackFrame> StackFrames;
        private Stack<long> _savedTime = new Stack<long>();
        private Stopwatch _stopwatch;

        private List<Queue<MethodInfo>> _methodsList = new List<Queue<MethodInfo>>();
        private Queue<MethodInfo> _traceQueue = new Queue<MethodInfo>();

        public ThreadTrace(int id)
        {
            Id = id;
            StackFrames = new Stack<ReadOnlyStackFrame>();
            _stopwatch = new Stopwatch();
        }

        public IEnumerable<Queue<MethodInfo>> GetListOfTracedMethods()
        {
            return _methodsList;
        }

        internal void SaveMethodInfo(MethodInfo method, ref int lastDepth)
        {
            if (Depth < lastDepth)
            {
                method.ChildMethods = _traceQueue;
                _traceQueue = new Queue<MethodInfo>();
            }

            _traceQueue.Enqueue(method);
            lastDepth = Depth;

            if (Depth == 0)
            {
                _methodsList.Add(_traceQueue);
                _traceQueue = new Queue<MethodInfo>();
            }
        }

        internal void AddStackFrame(ReadOnlyStackFrame traceDiagnostic)
        {
            StackFrames.Push(traceDiagnostic);
        }

        internal void TryToStartTimer()
        {
            if (!_stopwatch.IsRunning)
                _stopwatch.Start();
        }

        internal void StopwatchMark()
        {
            _savedTime.Push(_stopwatch.ElapsedMilliseconds);
        }

        internal void TryToStopTimer()
        {
            if (_stopwatch.IsRunning)
                _stopwatch.Stop();
        }

        internal long GetLastMethodTime()
        {
            TotalTime = Math.Max(TotalTime, _stopwatch.ElapsedMilliseconds);
            return _savedTime.Any() ? _stopwatch.ElapsedMilliseconds - _savedTime.Pop() : 0;
        }
    }
}
