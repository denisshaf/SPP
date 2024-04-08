namespace FakerLab.Generators.CollectionGenerators
{
    public class GeneratorIEnumerable<T, TGenerator> : IGenerator<IEnumerable<T>> 
        where TGenerator : IGenerator<T>, new()
    {
        private readonly Random _random = new();

        public IEnumerable<T> GetValue()
        {
            const int maxCapacity = 16;
            var capacity = _random.Next(maxCapacity);

            var generator = new TGenerator();
            var instance = new List<T>(_random.Next(capacity));

            for (int i = 0; i < capacity; i++)
            {
                instance.Add(generator.GetValue());
            }

            return instance;
        }
    }
}
