using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tracer
{
    public class MethodInfo
    {
        public string Name { get; set; }
        public string ClassName { get; set; }
        private long _time { get; set; }
        public string Time => $"{_time} ms";

        public IEnumerable<MethodInfo> ChildMethods = new Queue<MethodInfo>();

        public MethodInfo(string name, string className, long time)
        {
            Name = name;
            ClassName = className;
            _time = time;
        }
    }
}
