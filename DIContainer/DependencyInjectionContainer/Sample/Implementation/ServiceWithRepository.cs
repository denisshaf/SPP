using DependencyInjectionContainer.Container.Attributes;
using DependencyInjectionContainer.Sample.Interface;

namespace DependencyInjectionContainer.Sample.Implementation
{
    public class ServiceWithRepository<TRepository> : IServiceWithRepository<TRepository> where TRepository : IRepository
    {
        public ServiceWithRepository(TRepository repository)
        {
        }
    }
}
