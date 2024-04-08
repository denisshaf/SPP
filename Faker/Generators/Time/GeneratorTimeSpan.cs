using FakerProj.Generators;
using FakerProj.Generators.Numbers.Integers;

namespace FakerLab.Generators.TimeGenerators
{
    public class GeneratorTimeSpan : IGenerator<TimeSpan>
    {
        public TimeSpan GetValue()
        {
            var intGetnerator = new GeneratorInt();

            int days = intGetnerator.GetValue();
            int hours = intGetnerator.GetValue() % 24 + 1;
            int minutes = intGetnerator.GetValue() % 60 + 1;
            int seconds = intGetnerator.GetValue() % 60 + 1;
            int milliseconds = intGetnerator.GetValue() % 1000 + 1;
            int microseconds = intGetnerator.GetValue() % 1000 + 1;

            return new TimeSpan(days, hours, minutes, seconds, milliseconds, microseconds);
        }
    }
}
