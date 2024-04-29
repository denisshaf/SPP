using DependencyInjectionContainer.Container.Attributes;
using DependencyInjectionContainer.Container.Exceptions;
using DependencyInjectionContainer.Container.Interface;

namespace DependencyInjectionContainer.Container.Model
{
    public class Dependency(Type type, LifeCycle lifeCycle, string? name = null)
    {
        public Type Type { get; } = type;
        public LifeCycle LifeCycle { get; } = lifeCycle;
        public string? Name { get; } = name;

        private object? _instance;
        private readonly object _lockObject = new();

        public object GetInstance(IDependencyConfiguration configuration) =>
            LifeCycle == LifeCycle.Singleton ? GetSingleton(configuration) : GetInstancePerDependency(configuration);

        private object GetSingleton(IDependencyConfiguration configuration)
        {
            if (_instance is null)
            {
                lock (_lockObject)
                {
                    if (_instance is null)
                    {
                        _instance = GetInstancePerDependency(configuration);
                    }
                }
            }

            return _instance;
        }

        private object GetInstancePerDependency(IDependencyConfiguration configuration)
        {
            var constructor = Type.GetConstructors().MinBy(c => c.GetParameters().Length);

            if (constructor is null)
            {
                return Activator.CreateInstance(Type)
                    ?? throw new InstanceCreationException($"Unable to call constructor for {Type}");
            }

            var parametersInfo = constructor.GetParameters();
            var parameters = new object?[parametersInfo.Length];

            for (int i = 0; i < parametersInfo.Length; i++)
            {
                var parameterType = parametersInfo[i].ParameterType;

                if (parameterType.IsInterface && configuration.Container.TryGetValue(parameterType, out var value))
                {
                    // parameter has attribute
                    if (parametersInfo[i].IsDefined(typeof(DependencyKeyAttribute), false))
                    {
                        var attribute = parametersInfo[i]
                            .GetCustomAttributes(typeof(DependencyKeyAttribute), false)
                            .FirstOrDefault()
                            as DependencyKeyAttribute;
                        parameters[i] = value.Where(d => d.Name is not null && d.Name.Equals(attribute!.Name)).FirstOrDefault()?.GetInstance(configuration);
                    }
                    else
                    {
                        parameters[i] = value.FirstOrDefault()?.GetInstance(configuration)
                            ?? throw new DependencyProviderException($"Type for {parameterType} not found");
                    }
                }
                else
                {
                    parameters[i] = default;
                }
            }

            return constructor.Invoke(parameters);
        }

        public override bool Equals(object? obj)
        {
            return obj is Dependency dependency && dependency.Type.Equals(Type);
        }

        public override int GetHashCode()
        {
            return Type.GetHashCode();
        }
    }
}
