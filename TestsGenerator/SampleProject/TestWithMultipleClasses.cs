using System.Security;

namespace SampleProject
{
    public class TestWithMultipleClasses
    {
        public TestWithMultipleClasses(IEnumerable<int> a, int integer)
        {

        }

        public int DoSomething(int testParam, object a)
        {
            var abc = default(object);
            throw new NotImplementedException();
        }

        public void DoAnother()
        {

        }
    }

    public struct CoolStruct
    {

    }

    public abstract class AbstrctClass
    {
        public void AbstractStaff()
        {

        }
    }

    public interface IInterface
    {
        void InterfaceMethod();
    }

    public static class StaticClass
    {
        public static void StaticMethod()
        {
            throw new NotImplementedException();
        }
    }

    public class AnotherClass(TestWithMultipleClasses test)
    {
        public List<int> anotherThing()
        {
            throw new NotImplementedException();
        }
    }
}

namespace Another
{
    internal class InnerClass
    {
        public string InnerMethod()
        {
            throw new NotImplementedException();
        }
    }
}
