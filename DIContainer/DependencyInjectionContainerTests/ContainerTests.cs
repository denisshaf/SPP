using DependencyInjectionContainer.Container;
using DependencyInjectionContainer.Container.Model;
using DependencyInjectionContainer.Sample.Implementation;
using DependencyInjectionContainer.Sample.Interface;
using FluentAssertions;

namespace DependencyInjectionContainerTests
{
    public class ContainerTests
    {
        private readonly DependencyConfiguration _configuration;

        public ContainerTests()
        {
            _configuration = new DependencyConfiguration();
        }

        [Fact]
        public void Test_NamedDependency()
        {
            _configuration.Register<IService1, Service1>(LifeCycle.InstancePerDependency, "A");
            _configuration.Register<IService1, Service1Copy>(LifeCycle.InstancePerDependency, "C");

            var provider = new DependencyProvider(_configuration);

            var a = provider.Resolve<IService1>("A");
            var c = provider.Resolve<IService1>("C");

            a.Should().BeOfType<Service1>();
            c.Should().BeOfType<Service1Copy>();
        }

        [Fact]
        public void Test_NamedDependencyWithAnnotation()
        {
            _configuration.Register<IService1, Service1>(LifeCycle.InstancePerDependency, "A");
            _configuration.Register<IService1, Service1Copy>(LifeCycle.InstancePerDependency, "C");
            _configuration.Register<IRepository, DependsOnService1>(LifeCycle.InstancePerDependency);

            var provider = new DependencyProvider(_configuration);

            var expected = provider.Resolve<IRepository>();
            expected.Should().BeOfType(typeof(DependsOnService1));
            (expected as DependsOnService1)!.Service.Should().BeOfType<Service1>();
        }

        [Fact]
        public void Test_EnumerableDependency()
        {
            _configuration.Register<IService1, Service1>(LifeCycle.InstancePerDependency, "A");
            _configuration.Register<IService1, Service1>(LifeCycle.InstancePerDependency, "B");
            _configuration.Register<IService1, Service1Copy>(LifeCycle.InstancePerDependency, "C");

            var provider = new DependencyProvider(_configuration);

            var expected = provider.Resolve<IEnumerable<IService1>>().ToList();
            expected.Should().HaveCount(2);
        }

        [Fact]
        public void Test_SimpleResolving()
        {
            _configuration.Register<IService1, Service1>();

            var provider = new DependencyProvider(_configuration);

            var expected = provider.Resolve<IService1>();
            expected.Should().BeOfType<Service1>();
        }

        [Fact]
        public void Test_RecursiveDependency()
        {
            _configuration.Register<IRepository, Repository>(LifeCycle.InstancePerDependency);
            _configuration.Register<IServiceWithRepository<IRepository>,
                ServiceWithRepository<IRepository>>(LifeCycle.InstancePerDependency);

            var provider = new DependencyProvider(_configuration);
            provider.Should().NotBeNull();
        }

        [Fact]
        public void Test_SingletonLifeTime()
        {
            _configuration.Register<IService1, Service1>(LifeCycle.Singleton);

            var provider = new DependencyProvider(_configuration);

            var expected1 = provider.Resolve<IEnumerable<IService1>>();
            var expected2 = provider.Resolve<IEnumerable<IService1>>();

            expected1.Should().Equal(expected2);
        }

        [Fact]
        public void Test_TransientLifeTime()
        {
            _configuration.Register<IService1, Service1>();

            var provider = new DependencyProvider(_configuration);

            var expected1 = provider.Resolve<IEnumerable<IService1>>();
            var expected2 = provider.Resolve<IEnumerable<IService1>>();

            expected1.Should().NotEqual(expected2);
        }

        [Fact]
        public void Test_OpenGenerics()
        {
            _configuration.Register<IRepository, Repository>(LifeCycle.InstancePerDependency);
            _configuration.Register(typeof(IServiceWithRepository<>), typeof(ServiceWithRepository<>), LifeCycle.InstancePerDependency);

            var provider = new DependencyProvider(_configuration);

            var expected = provider.Resolve<IServiceWithRepository<IRepository>>();
            expected.Should().NotBeNull();
        }
    }
}