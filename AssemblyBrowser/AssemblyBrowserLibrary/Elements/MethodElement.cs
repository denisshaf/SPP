using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AssemblyBrowserLibrary.Elements
{
    public class MethodElement : Element
    {
        public string ReturnTypeName { get; set; }

        public string AccessModifier { get; set; }
        public string[] Parameters { get; set; }
        public MethodElement(MethodInfo methodInfo)
        {
            Childs = null;
            Name = methodInfo.Name;
            AccessModifier = string.Empty;

            if (methodInfo.IsPublic)
                AccessModifier = "public";
            else if (methodInfo.IsPrivate)
                AccessModifier = "private";
            else if (methodInfo.IsAssembly)
                AccessModifier = "internal";

            if (methodInfo.IsFamily)
                AccessModifier = "protected";

            if (methodInfo.IsStatic)
                AccessModifier += " static";

            ReturnTypeName = GetTypeName(methodInfo.ReturnType);
            var parameters = methodInfo.GetParameters();
            Parameters = new string[parameters.Length];

            for (int i = 0; i < parameters.Length; i++)
            {
                if (parameters[i].IsOut)
                {
                    Parameters[i] = "out ";
                }
                else if (parameters[i].ParameterType.IsByRef)
                {
                    Parameters[i] = "ref ";
                }
                else
                {
                    Parameters[i] = string.Empty;
                }
                Parameters[i] += GetTypeName(parameters[i].ParameterType);
                Parameters[i] += " " + parameters[i].Name;
            }
        }

        public MethodElement(MethodInfo methodInfo ,bool flag)
        {
            Childs = null;
            Name = methodInfo.Name;
            AccessModifier = "extension";

            if (methodInfo.IsPublic)
                AccessModifier += " public";
            else if (methodInfo.IsPrivate)
                AccessModifier += " private";
            else if (methodInfo.IsAssembly)
                AccessModifier += " internal";

            if (methodInfo.IsFamily)
                AccessModifier += " protected";

            ReturnTypeName = GetTypeName(methodInfo.ReturnType);
            var parameters = methodInfo.GetParameters();
            Parameters = new string[parameters.Length - 1];

            for (int i = 1; i < parameters.Length; i++)
            {
                if (parameters[i].IsOut)
                {
                    Parameters[i - 1] = "out ";
                }
                else if (parameters[i].ParameterType.IsByRef)
                {
                    Parameters[i - 1] = "ref ";
                }
                else
                {
                    Parameters[i - 1] = string.Empty;
                }
                Parameters[i-1] += GetTypeName(parameters[i].ParameterType);
                Parameters[i-1] += " " + parameters[i].Name;
            }
        }
        public override string Info
        {
            get 
            {
                string allParams = string.Empty;
                foreach (var param in Parameters)
                {
                    allParams += param;
                    allParams += ",";
                }
                if (allParams.Length > 0)
                {
                    allParams = allParams.Remove(allParams.Length - 1);
                }
                
                return $"method {AccessModifier} {ReturnTypeName} {Name}({allParams})"; 
            }
        }
    }
}
