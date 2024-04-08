namespace FakerLab.Generators.NumericGenerators.Floats
{
    public class GeneratorDouble : IGenerator<double>
    {
        private readonly Random _random = new();

        public double GetValue() => _random.NextDouble();
    }
}
