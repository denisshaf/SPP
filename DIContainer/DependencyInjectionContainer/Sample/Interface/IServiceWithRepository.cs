namespace DependencyInjectionContainer.Sample.Interface
{
    public interface IServiceWithRepository<TRepository> where TRepository : IRepository
    {
    }
}
