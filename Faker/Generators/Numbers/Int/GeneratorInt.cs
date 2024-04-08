namespace FakerLab.Generators.NumericGenerators.Integers
{
    public class GeneratorInt : IGenerator<int>
    {
        private readonly Random _random = new();

        public int GetValue() => _random.Next();
    }
}
