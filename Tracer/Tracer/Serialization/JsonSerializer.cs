using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Formats.Asn1;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Tracer.Models;

namespace Tracer.Serialization
{
    internal class JsonSerializer : ISerializer
    {
        public string Format => "json";
        Newtonsoft.Json.JsonSerializer serializer = new Newtonsoft.Json.JsonSerializer();

        public void Serialize(TraceResult traceResult, string path)
        {
            if (!path.Contains('.'))
                path = $"{path}.{Format}";

            using (StreamWriter stream = new StreamWriter(path))
            using (JsonWriter writer = new JsonTextWriter(stream))
            {
                writer.Formatting = Newtonsoft.Json.Formatting.Indented;
                serializer.Serialize(writer, traceResult);
            }
        }
    }
}
