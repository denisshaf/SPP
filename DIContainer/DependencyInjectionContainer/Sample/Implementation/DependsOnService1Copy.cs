using DependencyInjectionContainer.Container.Attributes;
using DependencyInjectionContainer.Sample.Interface;

namespace DependencyInjectionContainer.Sample.Implementation
{
    public class DependsOnService1Copy : IRepository
    {
        public IService1 Copy { get; set; }

        public DependsOnService1Copy([DependencyKey("C")] IService1 service1)
        {
            Copy = service1;
        }

        public override string ToString()
        {
            return $"{Copy.GetType()}";
        }
    }
}
