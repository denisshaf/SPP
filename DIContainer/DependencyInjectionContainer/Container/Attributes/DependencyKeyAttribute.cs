namespace DependencyInjectionContainer.Container.Attributes
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public class DependencyKeyAttribute(string name) : Attribute
    {
        public string Name = name;
    }
}
