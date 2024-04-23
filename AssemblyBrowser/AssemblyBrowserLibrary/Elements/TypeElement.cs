using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace AssemblyBrowserLibrary.Elements
{
    public class TypeElement : Element
    {
        public string Additions;
        public TypeElement(Type type, ref List<MethodInfo> extensions)
        {
            Name = type.Name;
            Additions = string.Empty;

            if (type.IsAbstract)
                Additions += "abstract ";
            else if (type.IsSealed)
                Additions += "sealed ";
            
            Childs = new List<Element>();
            FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
            foreach(FieldInfo fieldInfo in fields)
            {
                if(!fieldInfo.IsDefined(typeof(CompilerGeneratedAttribute)))
                {
                    Childs.Add(new FieldElement(fieldInfo));
                }
            }

            PropertyInfo[] properties = type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance| BindingFlags.Static);
            foreach(PropertyInfo propertyInfo in properties)
            {
                Childs.Add(new PropertyElement(propertyInfo));
            }
            
            
            MethodInfo[] methods = type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance|BindingFlags.Static);
            foreach (MethodInfo methodInfo in methods)
            {
                if (methodInfo.IsDefined(typeof(ExtensionAttribute)))
                {
                    extensions.Add(methodInfo);
                }
                else if (!methodInfo.IsDefined(typeof(CompilerGeneratedAttribute)))
                {
                    Childs.Add(new MethodElement(methodInfo));
                }
            }
        }

        public TypeElement(Type type, bool flag)
        {
            Name = type.Name;
            Additions = string.Empty;
            if (type.IsAbstract)
                Additions += "abstract ";
            else if (type.IsSealed)
                Additions += "sealed ";

            Childs = [];
        }
        public TypeElement(string name, bool a)
        {
            Childs = [];
            Additions = string.Empty;
            Name = name;
            Childs.Add(new FieldElement("nyasaasas","int"));
            Childs.Add(new FieldElement("uhasasah", "string"));
        }

        public override void AddExtension(string baseType, MethodInfo method)
        {
            Childs.Add(new MethodElement(method,true));
        }
        public override string Info => $"{Additions}{Name}";
    }
}
