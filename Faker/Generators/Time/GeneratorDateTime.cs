using FakerLab.Generators.NumericGenerators.Integers;

namespace FakerLab.Generators.TimeGenerators
{
    public class GeneratorDateTime : IGenerator<DateTime>
    {
        public DateTime GetValue()
        {
            var intGenerator = new GeneratorInt();

            int year = intGenerator.GetValue() % 10000;
            int month = intGenerator.GetValue() % 12 + 1;  
            int day = intGenerator.GetValue() % 28 + 1;
            int hour = intGenerator.GetValue() % 24 + 1;
            int minute = intGenerator.GetValue() % 60 + 1;
            int second = intGenerator.GetValue() % 60 + 1;
            int millisecond = intGenerator.GetValue() % 1000 + 1;
            int microsecond = intGenerator.GetValue() % 1000 + 1;

            return new DateTime(year, month, day, hour, minute, second, millisecond, microsecond, DateTimeKind.Utc);
        }
    }
}
