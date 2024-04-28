using System.Security;
using Xunit;
using Moq;
using Another;

namespace Tests
{
    public class Test_InnerClass
    {
        private InnerClass innerclass;
        public Test_InnerClass()
        {
        }

        [Fact]
        public void Test_InnerMethod()
        {
            var actual = innerclass.InnerMethod();
            var expected = default(string);
            Assert.Equal(expected, actual);
            Assert.Fail("autogenerated");
        }
    }
}