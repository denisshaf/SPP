namespace FakerProj.Entities
{
    public class A
    {
        public int Id;

        public B? B;

        public override string ToString() =>
            $"""
            Type: {GetType}
            Id: {Id}
            B: {B}
            """;
    }
}
