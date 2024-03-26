using Tracer;
using Tracer.Models;
using Tracer.Serialization;

namespace ConsoleApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ITracer tracer = new Tracer.Tracer();
            var foo = new Foo(tracer);
            int threadsAmount = 10;
            var locker = new object();
            var tasks = new Task[threadsAmount];
            for (int i = 1; i <= threadsAmount; i++)
            {
                tasks[i - 1] = new Task(() =>
                {
                    foo.RunMethodForTrace(50);
                });
            }
            Parallel.ForEach(tasks, (t) => { t.Start(); });
            Task.WaitAll(tasks);

            foo.PrintTraceResults();
        }
    }
}
public class Foo
{
    private Bar _bar;
    private ITracer _tracer;
    public TraceResult TraceResults => _tracer.GetTraceResult();

    public void RunMethodForTrace(int time)
    {
        _tracer.StartTrace();
        Thread.Sleep(time);
        _tracer.StopTrace();
    }

    internal Foo(ITracer tracer)
    {
        _tracer = tracer;
        _bar = new Bar(_tracer);
    }

    public void MyMethod()
    {
        _tracer.StartTrace();
        Thread.Sleep(100);
        _bar.InnerMethod();
        _tracer.StopTrace();
    }

    public void Serialize(string path)
    {
        if (path.Contains('.'))
            path = path.Substring(0, path.IndexOf('.'));
    }

    public void PrintTraceResults()
    {
        var result = _tracer.GetTraceResult();
        Console.WriteLine("Thread -> {0}", Environment.CurrentManagedThreadId);
        foreach (var t in result.Threads)
        {
            Console.WriteLine(t.ToString());
        }
        var s = new SerializeLoader();
        s.Serialize(result, "TraceResults");
    }
}

public class Bar
{
    private ITracer _tracer;

    internal Bar(ITracer tracer)
    {
        _tracer = tracer;
    }

    public void InnerMethod()
    {
        _tracer.StartTrace();
        Thread.Sleep(200);
        Foo1();
        Foo2();
        _tracer.StopTrace();
    }

    private void Foo1()
    {
        _tracer.StartTrace();
        Thread.Sleep(300);
        _tracer.StopTrace();
    }

    public void Foo2()
    {
        _tracer.StartTrace();
        Thread.Sleep(400);
        _tracer.StopTrace();
    }
}
