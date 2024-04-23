using System.Reflection;
using System.Runtime.CompilerServices;

namespace AssemblyBrowserLibrary.Elements
{
    public class NameSpaceElement : Element
    {
        public NameSpaceElement(string name, List<Type> types, ref List<MethodInfo> extensions)
        {
            Name = name;
            Childs = [];
            foreach(var type in types)
            {
                if(!type.IsDefined(typeof(CompilerGeneratedAttribute), false))
                {
                    var temp = new TypeElement(type, ref extensions);
                    
                    if(temp!=null)
                    {
                        Childs.Add(temp);
                    }
                }
            }
        }
        public NameSpaceElement(string name, List<Type> types)
        {
            Name = name;
            Childs = new List<Element>();
            foreach (var type in types)
            {
                if (!type.IsDefined(typeof(CompilerGeneratedAttribute), false))
                {
                    var temp = new TypeElement(type, true);

                    if (temp != null)
                    {
                        Childs.Add(temp);
                    }
                }
            }
        }

        public NameSpaceElement(string name, bool a)
        {
            Childs = new List<Element>();
            Name = name;
            Childs.Add(new TypeElement("nya", true));
            Childs.Add(new TypeElement("uhh", true));
        }

        public override void AddExtension(string baseType, MethodInfo method)
        {
            bool result = false;
            foreach(var type in Childs)
            {
                if(type.Name == baseType)
                {
                    type.AddExtension(baseType, method);
                    result = true;
                    break;
                }
            }
            if(!result)
            {
                
            }
        }
        public override string Info => $"namespace {Name}";
    }
}
