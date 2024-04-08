using FakerProj.Generators;

namespace FakerLab.Generators.Numbers.Floats
{
    public class GeneratorFloat : IGenerator<float>
    {
        private readonly Random _random = new();

        public float GetValue() => _random.NextSingle();
    }
}
