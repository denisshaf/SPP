using AssemblyBrowserLibrary;
using AssemblyBrowserLibrary.Elements;
using System.Reflection;
using TestClasses;

namespace TestProject
{
    public class Tests
    {       

        [Test]
        public void NotPossibleLoad_Test()
        {
            Browser browser = new Browser();

            var tree = browser.GetTreeHead("jujudfu");

            Assert.Multiple(() =>
            {
                Assert.That(tree.Name, Is.EqualTo("Error while loading assembly"));
                Assert.That(tree.Childs, Is.EqualTo(null));
            });
        }

        [Test]
        public void NameSpaceCount_Test()
        {
            Browser browser = new Browser();

            var tree = browser.GetTreeHead("TestClasses.dll");
            Assert.That(tree.Childs.Count, Is.EqualTo(2));
        }

        [Test]
        public void FieldInfo_Test()
        {
            Type a = typeof(Animal);


            var b = a.GetField("Age");
            var result = new FieldElement(b);

            Assert.Multiple(() =>
            {
                Assert.That(result.Name, Is.EqualTo(b.Name));
                Assert.That(result.FieldTypeName, Is.EqualTo("int"));
            });
        }

        [Test]
        public void PropertyInfo_Test()
        {
            Type a = typeof(Animal);


            var b = a.GetProperty("Name");
            var result = new PropertyElement(b);

            Assert.Multiple(() =>
            {
                Assert.That(result.Name, Is.EqualTo(b.Name));
                Assert.That(result.PropTypeName, Is.EqualTo("string"));
            });
        }

        [Test]
        public void MethodInfo_Test()
        {
            Type a = typeof(Animal);


            var b = a.GetMethod("Func");
            var result = new MethodElement(b);

            Assert.Multiple(() =>
            {
                Assert.That(result.Name, Is.EqualTo(b.Name));
                Assert.That(result.ReturnTypeName, Is.EqualTo("string"));
                Assert.That(result.Parameters.Length, Is.EqualTo(1));
            });
        }

        [Test]
        public void TypeInfo_Test()
        {
            Type a = typeof(Animal);

            var temp = new List<MethodInfo>();
            var result = new TypeElement(a,ref temp);

            Assert.Multiple(() =>
            {
                Assert.That(result.Name, Is.EqualTo(a.Name));
                Assert.That(result.Childs.Count, Is.EqualTo(10));
            });
        }
        [Test]
        public void Extensions_Test()
        {
            Browser browser = new Browser();

            var tree = browser.GetTreeHead("TestClasses.dll");

            Assert.Multiple(() =>
            {
                Assert.That(tree.Childs[0].Childs[0].Childs.Count, Is.EqualTo(11));
            });
        }

        [Test]
        public void ExternalExtensions_Test()
        {
            Browser browser = new Browser();

            var tree = browser.GetTreeHead("TestClasses.dll");

            Assert.Multiple(() =>
            {
                Assert.That(tree.Childs.Count, Is.EqualTo(2));
                Assert.That(tree.Childs[1].Childs[0].Childs.Count, Is.EqualTo(1));
            });
        }
    }
}