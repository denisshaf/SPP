namespace FakerProj.FakerLib
{
    public class FakerConfig
    {
        private readonly Dictionary<Type, Dictionary<string, Func<object?>>> _config = [];

        public object? GetGeneratedValue(Type className, string propertyName)
        {
            if (_config.TryGetValue(className, out Dictionary<string, Func<object?>>? value)
                && value.TryGetValue(propertyName, out Func<object?>? func))
            {
                return func.Invoke();
            }

            return default;
        }
    }
}
