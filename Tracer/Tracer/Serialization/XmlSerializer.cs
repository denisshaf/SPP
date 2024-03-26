using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tracer.Models;

namespace Tracer.Serialization
{
    internal class XmlSerializer : ISerializer
    {
        public string Format => "xml";
        Newtonsoft.Json.JsonSerializer serializer = new Newtonsoft.Json.JsonSerializer();

        public void Serialize(TraceResult traceResult, string path)
        {
            if (!path.Contains('.'))
                path = $"{path}.{Format}";

            var json = JsonConvert.SerializeObject(traceResult);
            var node = JsonConvert.DeserializeXNode(json, "root");
            using (StreamWriter stream = new StreamWriter(path))
            {
                stream.Write(node?.ToString());
            }
        }
    }
}
