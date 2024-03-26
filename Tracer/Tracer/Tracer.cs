using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tracer.Models;

namespace Tracer
{
    public class Tracer : ITracer
    {
        Dictionary<int, ThreadTrace> Threads = new Dictionary<int, ThreadTrace>();

        private object _lockObject = new object();
        private int _lastDepth = int.MinValue;

        private int _currentThreadId => System.Environment.CurrentManagedThreadId;

        public TraceResult GetTraceResult()
        {
            var dtoList = new List<ThreadTraceDTO>();
            foreach (var thread in Threads.Values)
            {
                dtoList.Add(new(thread.Id, thread.TotalTime, thread.GetListOfTracedMethods()));
            }
            return new TraceResult() { Threads = dtoList };
        }

        public void StartTrace()
        {
            lock (_lockObject)
            {
                if (!Threads.ContainsKey(_currentThreadId))
                    Threads.Add(_currentThreadId, new ThreadTrace(_currentThreadId));

                var frame = new StackTrace(true).GetFrame(1);
                if (frame == null)
                    throw new NullReferenceException($"There are no frames in thread: {_currentThreadId}");

                var thread = Threads[_currentThreadId];
                thread.AddStackFrame(new ReadOnlyStackFrame(frame));
                thread.StopwatchMark();
                thread.TryToStartTimer();
            }
        }

        public void StopTrace()
        {
            lock (_lockObject)
            {
                if (!Threads.ContainsKey(_currentThreadId))
                    throw new InvalidOperationException(
                      $"Attemp to stop trace for thread (id {_currentThreadId}) without starting tracing"
                    );

                var thread = Threads[_currentThreadId];
                thread.TryToStopTimer();
                if (thread.StackFrames.TryPop(out var diagnostic))
                {
                    var methodInfo = diagnostic.Frame.GetMethod();
                    var sb = new StringBuilder();
                    if (methodInfo != null)
                    {
                        thread.SaveMethodInfo(new MethodInfo(
                            methodInfo.Name,
                            methodInfo.ReflectedType!.Name,
                            thread.GetLastMethodTime()
                          ),
                          ref _lastDepth
                        );
                    }
                }
                thread.TryToStartTimer();
            }
        }
    }
}
