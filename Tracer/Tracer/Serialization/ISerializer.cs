using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tracer.Models;

namespace Tracer.Serialization
{
    internal interface ISerializer
    {
        string Format { get; }
        void Serialize(TraceResult traceResult, string path);
    }
}
