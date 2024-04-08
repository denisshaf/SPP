using FakerProj.Generators;

namespace FakerProj.Entities
{
    internal class MyGenerator : IGenerator<string>
    {
        public string GetValue() => "my generator's random string";
    }
}
