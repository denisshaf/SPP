using System.Reflection;

namespace AssemblyBrowserLibrary.Elements
{
    public class FieldElement : Element
    {
        public string? AccessModifier { get; set; }

        public string FieldTypeName { get; set; }
        public override string Info => "field " + AccessModifier + " " + FieldTypeName + " " + Name;

        public FieldElement(FieldInfo fieldInfo)
        {
            Childs = null!;
            Name = fieldInfo.Name;
            AccessModifier = string.Empty;

            if (fieldInfo.IsPublic)
                AccessModifier = "public";
            else if (fieldInfo.IsPrivate)
                AccessModifier = "private";
            else if (fieldInfo.IsAssembly)
                AccessModifier = "internal";

            if (fieldInfo.IsFamily)
                AccessModifier = "protected";

            if (fieldInfo.IsStatic)
                AccessModifier += " static";

            FieldTypeName = GetTypeName(fieldInfo.FieldType);
        }
        public FieldElement(string name, string typeName)
        {
            Childs = null!;
            Name = name;
            FieldTypeName = typeName;
        }
    }
}
