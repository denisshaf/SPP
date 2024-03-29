using Tracer;

namespace Tests
{
    public class UnitTest1
    {
        const int testTime = 55;
        Foo foo = new Foo();

        [Fact]
        public void CorrectTimeTest()
        {
            foo.RunMethodForTrace(testTime);
            var result = foo.Tracer.GetTraceResult();
            var threadInfo = result.Threads[0];

            var methodInfo = threadInfo.Methods.First().Peek();
            long.TryParse(methodInfo.Time.Substring(0, methodInfo.Time.Length - 2), out var time);
            // 10 - is max stopwatch inaccuracy
            Assert.True(Math.Abs(time - testTime) <= 10 &&
                        methodInfo.Name.Equals("RunMethodForTrace") &&
                        methodInfo.ClassName.Equals("Foo"),
                        $"Test of correct work finished with {methodInfo.Time}");
        }

        [Theory]
        [InlineData(2)]
        [InlineData(5)]
        [InlineData(10)]
        public void CorrectMultithreadingTest(int threadsAmount)
        {
            var tasks = new Task[threadsAmount];
            for (int i = 1; i <= threadsAmount; i++)
                tasks[i - 1] = new Task(() => foo.RunMethodForTrace(testTime * i));
            Parallel.ForEach(tasks, t => t.Start());
            Task.WaitAll(tasks);
            var result = foo.Tracer.GetTraceResult();
            var methodInfoAmount = result.Threads.Select(dto => dto.Methods.Select(m => m.Count()).Sum()).Sum();

            Assert.Equal(threadsAmount, methodInfoAmount);
        }

        [Fact]
        public void CorrectInnerMethodsTest()
        {
            foo.RunMethodWithInnerMethod(testTime);
            var result = foo.Tracer.GetTraceResult();
            var threadInfo = result.Threads[0];

            var innerMethodInfo = threadInfo.Methods.First().Peek().ChildMethods.First();
            int.TryParse(innerMethodInfo.Time, out var time);
            Assert.True(innerMethodInfo.Name.Equals("RunInnerMethodForTrace") &&
                        innerMethodInfo.ClassName.Equals("Foo"),
                        $"Test of correct work of method {innerMethodInfo.Name} ({innerMethodInfo.ClassName}) finished with {innerMethodInfo.Time}");

        }

        private class Foo
        {
            public ITracer Tracer = new Tracer.Tracer();
            public void RunMethodForTrace(int time)
            {
                Tracer.StartTrace();
                Thread.Sleep(time);
                Tracer.StopTrace();
            }

            public void RunMethodWithInnerMethod(int time)
            {
                Tracer.StartTrace();
                Thread.Sleep(time);
                RunInnerMethodForTrace(2 * time);
                Tracer.StopTrace();
            }

            private void RunInnerMethodForTrace(int time = 100)
            {
                Tracer.StartTrace();
                Thread.Sleep(time);
                Tracer.StopTrace();
            }
        }
    }
}