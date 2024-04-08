using System.Text;

namespace FakerProj.Generators.StringGenerators
{
    public class GeneratorString : IGenerator<string>
    {
        private readonly Random _random = new();

        public string GetValue()
        {
            int maxLen = 16;
            int length = _random.Next(maxLen);
            string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

            var stringBuilder = new StringBuilder(length);

            for (int i = 0; i < length; i++)
            {
                int index = _random.Next(chars.Length);
                char randomChar = chars[index];
                stringBuilder.Append(randomChar);
            }

            return stringBuilder.ToString();
        }
    }
}
