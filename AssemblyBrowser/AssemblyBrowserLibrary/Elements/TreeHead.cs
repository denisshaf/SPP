using System.Reflection;

namespace AssemblyBrowserLibrary.Elements
{
    public class TreeHead : Element
    {
        public override string Info => "TreeHead";
        public TreeHead(Dictionary<string, List<Type>> allTypes)
        {
            Childs = [];
            Name = "ok";
            var extensions = new List<MethodInfo>();

            foreach(var space in allTypes.Keys)
            {
                var temp = new NameSpaceElement(space, allTypes[space], ref extensions);
                if(temp.Childs.Count > 0)
                {
                    Childs.Add(temp);
                }
            }
            foreach(var method in extensions)
            {
                AddExtension(method);
            }
        }
        public TreeHead(string message)
        {
            Name = message;
            Childs = null;
        }
        public TreeHead(bool a)
        {
            Childs = [];
            Name = "nyanyanaynaynaynay";
            Childs.Add(new NameSpaceElement("uhuhu", true));
            Childs.Add(new NameSpaceElement("uhusaudhau", true));
        }
        private void AddExtension(MethodInfo extension)
        {
            bool result = false;
            var a = MethodInfoExtensions.GetBaseDefinition(extension);
            var baseType = extension.GetParameters()[0].ParameterType;
            foreach(var space in Childs)
            {
                if(space.Name == baseType.Namespace)
                {
                    space.AddExtension(baseType.Name, extension);
                    result = true;
                    break;
                }
            }

            if(!result)
            {
                var extensions = new List<Type> { baseType };
                
                Childs.Add(new NameSpaceElement(baseType.Namespace, extensions));
                foreach (var space in Childs)
                {
                    if (space.Name == baseType.Namespace)
                    {
                        space.AddExtension(baseType.Name, extension);
                        break;
                    }
                }
            }
        }
    }
}
