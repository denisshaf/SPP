using FakerLab.Generators.Numbers.Floats;
using FakerLab.Generators.TimeGenerators;
using FakerProj.Generators.CollectionGenerators;
using FakerProj.Generators.Numbers.Floats;
using FakerProj.Generators.Numbers.Integers;
using FakerProj.Generators.NumericGenerators.Integers;
using FakerProj.Generators.StringGenerators;
using FakerProj.Generators.TimeGenerators;
using System;
using System.Reflection;

namespace FakerProj.FakerLib
{
    public class Faker
    {
        private readonly Dictionary<Type, Type> _generators = new()
        {
            { typeof(int), typeof(GeneratorInt) },
            { typeof(IEnumerable<>), typeof(GeneratorIEnumerable<,>) },
            { typeof(string), typeof(GeneratorString) },
            { typeof(List<>), typeof(GeneratorList<,>) },
            { typeof(decimal), typeof(GeneratorDecimal) },
            { typeof(double), typeof(GeneratorDouble) },
            { typeof(float), typeof(GeneratorFloat) },
            { typeof(byte), typeof(GeneratorByte) },
            { typeof(long), typeof(GeneratorLong) },
            { typeof(short), typeof(GeneratorShort) },
            { typeof(char), typeof(GeneratorChar) },
            { typeof(DateTime), typeof(GeneratorDateTime) },
            { typeof(TimeSpan), typeof(GeneratorTimeSpan) }
        };

        private readonly HashSet<Type> _types = [];
        private readonly FakerConfig _config;

        public Faker()
        {
            _config = new();
        }

        public Faker(FakerConfig config)
        {
            _config = config;
        }
        public T? Create<T>()
        {
            var type = typeof(T);

            return (T?)Create(type);
        }
        public object? Create(Type type)
        {
            if (_types.Remove(type))
            {
                return null;
            }

            _types.Add(type);

            var instance = GetInstance(type);
            FillFields(instance);
            FillProperties(instance);

            _types.Remove(type);

            return instance;
        }

        private void FillProperties(object? instance)
        {
            if (instance is null)
            {
                return;
            }

            var type = instance.GetType();
            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var property in properties)
            {
                var value = property.GetValue(instance);

                if (HasPublicSetter(property) && (value is null || IsDefaultValue(value)))
                {
                    var generated = _config.GetGeneratedValue(type, property.Name.ToLower())
                        ?? GetGeneratedValue(property.PropertyType);
                    property.SetValue(instance, generated);
                }
            }
        }

        private void FillFields(object? instance)
        {
            if (instance is null)
            {
                return;
            }

            var type = instance.GetType();
            var fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);

            foreach (var field in fields)
            {
                var value = field.GetValue(instance);
                if (value is null || IsDefaultValue(value))
                {
                    var generated = _config.GetGeneratedValue(type, field.Name.ToLower())
                        ?? GetGeneratedValue(field.FieldType);
                    field.SetValue(instance, generated);
                }
            }
        }

        private static bool IsDefaultValue(object value)
        {
            var valueType = value.GetType();
            var localInstance = Activator.CreateInstance(valueType);

            return valueType.IsValueType && value.Equals(localInstance);
        }

        private static bool HasPublicSetter(PropertyInfo property)
        {
            var setter = property.GetSetMethod();

            return setter is not null && setter.IsPublic;
        }

        private object? GetInstance(Type type)
        {
            if (type.IsAbstract)
            {
                return null;
            }

            var constructors = type.GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            var constructor = constructors.OrderByDescending(c => c.GetParameters().Length).FirstOrDefault();

            if (constructor is null)
            {
                return Activator.CreateInstance(type)!;
            }

            var parameters = constructor.GetParameters();
            var constructorArgs = new object?[parameters.Length];

            for (int i = 0; i < parameters.Length; i++)
            {
                var parameter = parameters[i];
                var parameterType = parameter.ParameterType;

                constructorArgs[i] = GetGeneratedValue(parameterType);
            }

            try
            {
                return constructor.Invoke(constructorArgs);
            }
            catch (TargetInvocationException)
            {
                return null;
            }
        }

        private object? GetGeneratedValue(Type type)
        {
            var baseType = type;

            if (type.IsGenericType)
            {
                type = type.GetGenericTypeDefinition();
            }

            if (_generators.TryGetValue(type, out Type? generatorType))
            {
                var generator = Activator.CreateInstance(generatorType);
                var generateMethod = generatorType.GetMethod("GetValue")!;

                return generateMethod.Invoke(generator, null);
            }

            return Create(type);
        }
    }
}
