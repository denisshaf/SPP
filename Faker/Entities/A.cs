namespace FakerProj.Entities
{
    public class A
    {
        public int Id;

        public B? B;
        private object? obj;

        public override string ToString() =>
            $"""
            Type: {GetType}
            Id: {Id}
            obj: {obj}
            B: {B}
            """;
    }
}
