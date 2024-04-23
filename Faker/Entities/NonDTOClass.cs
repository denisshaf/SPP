namespace FakerProj.Entities
{
    public class NonDTOClass
    {
        public Uri? Uri { get; set; }
        public override string ToString() =>
            $"""
            Type: {GetType()}
            Uri: {Uri}
            """;
    }
}
