namespace DependencyInjectionContainer.Container.Interface
{
    internal interface IDependencyProvider
    {
        Interface Resolve<Interface>(string? name) where Interface : class;
    }
}
