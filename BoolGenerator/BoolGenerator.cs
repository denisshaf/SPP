using FakerProj.Generators;

namespace BoolGenerator
{
    public class BoolGenerator : IGenerator<bool>
    {
        protected readonly Random Random = new();
        public bool GetValue() => Random.Next(2) == 1;
    }
}
