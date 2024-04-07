namespace FakerProj.Entities
{
    public class C
    {
        public int CId;
        public B? B;

        public override string ToString() =>
            $"""
            Type: {GetType()}
            Id: {CId}
            B: {B}
            """;
    }
}
