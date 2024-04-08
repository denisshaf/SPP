namespace FakerProj.Generators
{
    public interface IGenerator<T>
    {
        public T GetValue();
    }
}
