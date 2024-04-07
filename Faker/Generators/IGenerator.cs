namespace FakerProj.Generators
{
    internal interface IGenerator<T>
    {
        public T GetValue();
    }
}
