using FakerProj.Generators.Numbers.Integers;

namespace FakerProj.Generators.Numbers.Floats
{
    public class GeneratorDecimal : IGenerator<decimal>
    {
        public decimal GetValue()
        {
            var intGenerator = new GeneratorInt();

            int lo = intGenerator.GetValue();
            int mid = intGenerator.GetValue();
            int hi = intGenerator.GetValue();

            return new decimal(lo, mid, hi, false, 0);
        }
    }
}
