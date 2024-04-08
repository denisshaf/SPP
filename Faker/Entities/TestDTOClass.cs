namespace FakerProj.Entities
{
    public class TestDTOClass
    {
        public string? String { get; set; }
        public int Int { get; set; }
        public float Float { get; set; }
        public double Double;
        public decimal Decimal;
        public A? A;
        public IEnumerable<int>? Enumerable;
        public List<string>? List;
        public Dictionary<int, int>? Dictionary;
        public DateTime DateTime { get; set; }

        public TestDTOClass(double dbl, A a)
        {
            Double = dbl;
            A = a;
        }

        public override string ToString() =>
            $"""
            Type: {GetType()}
            string: {String}
            int: {Int}
            float: {Float}
            double: {Double}
            decimal: {Decimal}
            enumerable: {string.Join(", ", Enumerable?.Select(item => $"{item}") ?? [])}
            list: {string.Join(", ", List?.Select(item => $"{item}") ?? [])}
            dictionary: {Dictionary}
            datetime: {DateTime}
            a: {A}
            """;
    }
}
