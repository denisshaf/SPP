using DependencyInjectionContainer.Container.Exceptions;
using DependencyInjectionContainer.Container.Interface;
using DependencyInjectionContainer.Container.Model;
using DependencyInjectionContainer.Utils;

namespace DependencyInjectionContainer.Container
{
    public class DependencyProvider(IDependencyConfiguration configuration) : IDependencyProvider
    {
        public object Resolve(Type type, string? name = null)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>))
            {
                return ResolveAsEnumerable(type, name);
            }

            return ResolveAsObject(type, name);
        }

        public Interface Resolve<Interface>(string? name = null) where Interface : class
        {
            var type = typeof(Interface);

            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>))
            {
                var list = ResolveAsEnumerable(type, name).ToList();
                var elem = type.GetGenericArguments().First()
                    ?? throw new DependencyProviderException("Failed to get generic parameter");

                return GenericConverter.ConvertToTypedEnumerable(list, elem) as Interface
                    ?? throw new DependencyProviderException($"Unable to resolve {type}"); ;
            }

            return ResolveAsObject(type, name) as Interface
                ?? throw new DependencyProviderException($"Unable to resolve {type}");
        }

        private IEnumerable<object> ResolveAsEnumerable(Type type, string? name = null)
        {
            IEnumerable<Dependency> dependencies = configuration.Container[type.GetGenericArguments().First()]
                ?? throw new DependencyProviderException($"Type for {type} not found");

            if (name is not null)
            {
                dependencies = dependencies.Where(d => d.Name is not null && d.Name.Equals(name));
            }

            foreach (var dependency in dependencies)
            {
                yield return dependency.GetInstance(configuration);
            }
        }

        private object ResolveAsObject(Type type, string? name = null)
        {
            if (configuration.Container.TryGetValue(type, out var dependencies))
            {
                Dependency dependency;
                if (name is null)
                {
                    dependency = dependencies.FirstOrDefault()
                        ?? throw new DependencyProviderException($"Type for {type} not found");
                }
                else
                {
                    dependency = dependencies.Where(d => d.Name is not null && d.Name.Equals(name)).FirstOrDefault()
                        ?? throw new DependencyProviderException($"Type for {type} not found");
                }

                return dependency.GetInstance(configuration);
            }

            Dependency nonParametrizedDependency;
            if (name is null)
            {
                nonParametrizedDependency = configuration.Container[type.GetGenericTypeDefinition()].FirstOrDefault()
                    ?? throw new DependencyProviderException($"Type for {type} not found");
            }
            else
            {
                nonParametrizedDependency = configuration.Container[type.GetGenericTypeDefinition()]
                    .Where(d => d.Name is not null && d.Name.Equals(name))
                    .FirstOrDefault()
                    ?? throw new DependencyProviderException($"Type for {type} not found");
            }

            var parametrizedType = nonParametrizedDependency.Type.MakeGenericType(type.GetGenericArguments());
            var parametrizedDependency = new Dependency(parametrizedType, nonParametrizedDependency.LifeCycle);

            return parametrizedDependency.GetInstance(configuration);
        }
    }
}
