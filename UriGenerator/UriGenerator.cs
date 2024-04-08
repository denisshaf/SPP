using FakerProj.Generators;
using FakerProj.Generators.StringGenerators;

namespace UriGenerator
{
    public class UriGenerator : IGenerator<Uri>
    {
        public Uri GetValue()
        {
            var generator = new GeneratorString();

            var builder = new UriBuilder
            {
                Scheme = "http",
                Host = "localhost",
                Path = generator.GetValue()
            };

            return builder.Uri;
        }
    }
}
