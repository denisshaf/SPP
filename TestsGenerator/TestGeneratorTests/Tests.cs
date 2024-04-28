using FluentAssertions;
using TestsGeneratorLib;

namespace TestGeneratorTests
{
    public class Tests
    {
        [Fact]
        public void Test_NoClasses()
        {
            var res = TestGenerator.ParseTestFile("", "Samples").ToList();

            res.Should().BeEmpty();
        }

        [Fact]
        public async Task Test_NoThreads()
        {
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () =>
            {
                await TestGenerator.Generate(
                    [
                        @"..\..\..\..\SampleProject\TestWithMultipleClasses.cs",
                        @"..\..\..\..\SampleProject\AnotherCoolTest.cs"
                    ],
                    @"..\..\..\..\Samples",
                    0);
            });
        }

        [Fact]
        public async Task Test_DirectoryDoesNotExist()
        {
            if (Directory.Exists(@"..\..\..\..\DoesntExist"))
            {
                Directory.Delete(@"..\..\..\..\DoesntExist", true);
            }

            await TestGenerator.Generate(
                [
                    @"..\..\..\..\SampleProject\TestWithMultipleClasses.cs",
                    @"..\..\..\..\SampleProject\AnotherCoolTest.cs"
                ],
                @"..\..\..\..\DoesntExist",
                10);

            Directory.Delete(@"..\..\..\..\DoesntExist", true);
        }

        [Fact]
        public void Test_MultipleClasses()
        {
            var res = 
                TestGenerator.ParseTestFile(File.ReadAllText(
                    @"..\..\..\..\SampleProject\TestWithMultipleClasses.cs"), "Samples").ToList();

            res.Should().HaveCount(4);
        }

        [Fact]
        public void Test_MultipleNamespaces()
        {
            var res =
                TestGenerator.ParseTestFile(File.ReadAllText(
                    @"..\..\..\..\SampleProject\TestWithMultipleClasses.cs"), "Samples").ToList();

            res.Should().HaveCount(4);
        }
    }
}