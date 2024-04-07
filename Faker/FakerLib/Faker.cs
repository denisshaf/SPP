using System.Reflection;

namespace FakerProj.FakerLib
{
    public class Faker
    {
        private readonly Dictionary<Type, Type> _generators = new() {};
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
            var instance = GetInstance(type);
            FillFields(instance);
            FillProperties(instance);

            return instance;
        }

        private void FillProperties(object? instance)
        {
            throw new NotImplementedException();
        }

        private void FillFields(object? instance)
        {
            throw new NotImplementedException();
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

        private object? GetGeneratedValue(Type parameterType)
        {
            throw new NotImplementedException();
        }
    }
}
