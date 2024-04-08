namespace FakerProj.Entities
{
    public class PrivateConstructorClass
    {
        public int Id;
        private PrivateConstructorClass(int id)
        {
            Id = id;
        }

        public override string ToString() =>
            $"""
            Type: {GetType()}
            int: {Id}
            """;
    }
}
