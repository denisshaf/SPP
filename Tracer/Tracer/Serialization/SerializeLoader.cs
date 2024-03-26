using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Tracer.Models;

namespace Tracer.Serialization
{
    public class SerializeLoader
    {
        public void Serialize(TraceResult result, string fileName)
        {
            var asm = Assembly.LoadFrom(@"Tracer.dll");
            var type = typeof(ISerializer);
            var types = asm.GetTypes()
            .Where(p => type.IsAssignableFrom(p) && !p.IsInterface);
            foreach (var tSerializer in types)
            {
                ISerializer? serializer = (ISerializer?)Activator.CreateInstance(tSerializer);
                var method = tSerializer.GetMethod("Serialize");
                method?.Invoke(serializer, new object[] { result, fileName });
            }
        }
    }
}
