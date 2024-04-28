using TestsGeneratorLib;

await TestGenerator.Generate(
    [
        @"..\..\..\..\SampleProject\TestWithMultipleClasses.cs",
        @"..\..\..\..\SampleProject\AnotherCoolTest.cs"
    ],
    @"..\..\..\..\Samples",
    10
);
