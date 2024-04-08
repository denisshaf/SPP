namespace FakerProj.Entities
{
    public class B(int id)
    {
        public readonly int Id = id;
        public readonly string Name;

        public C? C;

        public override string ToString() =>
            $"""
            Type: {GetType()} 
            Id: {Id} 
            C: {C}
            """;
    }
}
