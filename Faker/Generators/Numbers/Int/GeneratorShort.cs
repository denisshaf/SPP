namespace FakerLab.Generators.NumericGenerators.Integers
{
    public class GeneratorShort : IGenerator<short>
    {
        private readonly Random _random = new();

        public short GetValue() => (short)_random.Next(short.MaxValue);
    }
}
