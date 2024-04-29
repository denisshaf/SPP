using DependencyInjectionContainer.Container.Model;

namespace DependencyInjectionContainer.Container.Interface
{
    public interface IDependencyConfiguration
    {
        Dictionary<Type, HashSet<Dependency>> Container { get; }

        void Register<Interface, Implementation>(LifeCycle lifeCycle, string? name) 
            where Interface : class
            where Implementation : class;
        void Register(Type interfaceType, Type implementationType, LifeCycle lifeCycle, string? name);
    }
}
