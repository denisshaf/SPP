namespace DependencyInjectionContainer.Container.Exceptions
{
    public class InstanceCreationException : Exception
    {
        public InstanceCreationException() { }

        public InstanceCreationException(string message) : base(message) { }

        public InstanceCreationException(string message, Exception e) : base(message, e) { }
    }
}
