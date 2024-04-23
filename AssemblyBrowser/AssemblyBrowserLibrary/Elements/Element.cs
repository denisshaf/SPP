using System.Reflection;
using System.Text;

namespace AssemblyBrowserLibrary.Elements
{
    public abstract class Element
    {
        public string? Name { get; set; }
        public List<Element>? Childs { get; set; }

        public abstract string Info { get; }
        public static string GetTypeName(Type type)
        {
            var nullableType = Nullable.GetUnderlyingType(type);
            if (nullableType != null)
                return nullableType.Name + "?";

            if (!(type.IsGenericType && type.Name.Contains('`')))
            {
                var name = type.Name switch
                {
                    "String" => "string",
                    "Int32" => "int",
                    "Decimal" => "decimal",
                    "Object" => "object",
                    "Void" => "void",
                    _ => string.IsNullOrWhiteSpace(type.FullName) ? type.Name : type.FullName,
                };

                return name;
            }

            var sb = new StringBuilder(type.Name.Substring(0, type.Name.IndexOf('`')));
            sb.Append('<');
            var isFirst = true;
            foreach (var t in type.GetGenericArguments())
            {
                if (!isFirst)
                {
                    isFirst = false;
                    sb.Append(',');
                }
                sb.Append(GetTypeName(t));
            }
            sb.Append('>');

            return sb.ToString();
        }

        public virtual void AddExtension(string baseType, MethodInfo method)
        {

        }
    }
}