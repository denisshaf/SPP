namespace FakerLab.Generators.NumericGenerators.Integers
{
    internal class GeneratorLong : IGenerator<long>
    {
        private readonly Random _random = new();

        public long GetValue() => _random.NextInt64();
    }
}
