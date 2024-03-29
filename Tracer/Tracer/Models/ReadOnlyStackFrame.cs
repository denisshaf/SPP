using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tracer.Models
{
    internal class ReadOnlyStackFrame
    {
        public readonly StackFrame Frame;

        public ReadOnlyStackFrame(StackFrame frame)
        {
            Frame = frame;
        }
    }
}
