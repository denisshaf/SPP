using AssemblyBrowserLibrary.Elements;
using System.Reflection;

namespace AssemblyBrowserLibrary
{
    public class Browser
    {
        private Assembly? _workingAssembly;
        public Element GetTreeHead(string path)
        {
            try
            {
                _workingAssembly = Assembly.LoadFrom(path);
            }
            catch
            {
                return new TreeHead("Error while loading assembly");
            }

            if(_workingAssembly is not null)
            {
                var allTypes = _workingAssembly.GetTypes();
                var temp = ParseAssembly(allTypes);
                var result = new TreeHead(temp);
                return result;
            }
            return new TreeHead("Something went wrong");
        }

        public Dictionary<string, List<Type>> ParseAssembly(Type[] allTypes)
        {
            var result = new Dictionary<string, List<Type>>();
            foreach (var type in allTypes)
            {
                var space = type.Namespace;
                if(result.GetValueOrDefault(space) is null)
                {
                    result.Add(space, []);
                }
                result[space].Add(type);
            }

            return result;
        }
    }
}
