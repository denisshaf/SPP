using FakerProj.Generators;
using System.Linq.Expressions;

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

        public void Add<TTarget, TProperty, TGenerator>(Expression<Func<TTarget, TProperty>> propertySelector)
            where TGenerator : IGenerator<TProperty>, new()
        {
            var targetType = typeof(TTarget);
            var propertyName = GetPropertyName(propertySelector);

            if (!_config.TryGetValue(targetType, out Dictionary<string, Func<object?>>? value))
            {
                value = [];
                _config[targetType] = value;
            }

            value[propertyName] = () => GenerateValue<TProperty, TGenerator>();
        }

        private static string GetPropertyName<TTarget, TProperty>(Expression<Func<TTarget, TProperty>> propertySelector)
        {
            if (propertySelector.Body is MemberExpression memberExpression)
            {
                return memberExpression.Member.Name.ToLower();
            }

            throw new ArgumentException("Invalid property selector expression");
        }

        private static TProperty GenerateValue<TProperty, TGenerator>()
            where TGenerator : IGenerator<TProperty>, new()
        {
            var generator = new TGenerator();

            return generator.GetValue();
        }
    }
}
