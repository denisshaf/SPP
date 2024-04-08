namespace FakerProj.Generators.StringGenerators
{
    public class GeneratorChar : IGenerator<char>
    {
        private readonly Random _random = new();

        public char GetValue() => (char)_random.Next(byte.MaxValue);
    }
}
