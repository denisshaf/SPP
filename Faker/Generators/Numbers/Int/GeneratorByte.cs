using FakerProj.Generators;

namespace FakerProj.Generators.Numbers.Integers
{
    public class GeneratorByte : IGenerator<byte>
    {
        private readonly Random _random = new();

        public byte GetValue() => (byte)_random.Next(byte.MaxValue);
    }
}
