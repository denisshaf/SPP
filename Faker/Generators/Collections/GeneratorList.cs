namespace FakerProj.Generators.CollectionGenerators
{
    public class GeneratorList<T, TGenerator> : IGenerator<List<T>>
        where TGenerator : IGenerator<T>, new()
    {
        public List<T> GetValue()
        {
            var baseGenerator = new GeneratorIEnumerable<T, TGenerator>();

            return baseGenerator.GetValue().ToList();
        }
    }
}
