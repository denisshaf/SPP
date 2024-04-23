using System.Reflection;

namespace AssemblyBrowserLibrary.Elements
{
    public class PropertyElement:Element
    {
        public string PropTypeName { get; set; }
        public string AccessModifier { get; set; }
        public override string Info => $"property {AccessModifier} {PropTypeName} {Name}";
        public PropertyElement(PropertyInfo propertyInfo)
        {
            Childs = null;
            AccessModifier = string.Empty;

            if(propertyInfo.GetMethod is not null)
            {
                if (propertyInfo.GetMethod.IsPublic)
                    AccessModifier = "public";
                else if (propertyInfo.GetMethod.IsPrivate)
                    AccessModifier = "private";
                else if (propertyInfo.GetMethod.IsAssembly)
                    AccessModifier = "internal";
                else if (propertyInfo.GetMethod.IsFamily)
                    AccessModifier = "protected";
                if (propertyInfo.GetMethod.IsStatic)
                    AccessModifier += " static";
            }
            else if (propertyInfo.SetMethod is not null)
            {
                if (propertyInfo.SetMethod.IsPublic)
                    AccessModifier = "public";
                else if (propertyInfo.SetMethod.IsPrivate)
                    AccessModifier = "private";
                else if (propertyInfo.SetMethod.IsAssembly)
                    AccessModifier = "internal";
                else if (propertyInfo.SetMethod.IsFamily)
                    AccessModifier = "protected";
                if (propertyInfo.SetMethod.IsStatic)
                    AccessModifier += " static";
            }
            
            Name = propertyInfo.Name;
            PropTypeName = GetTypeName(propertyInfo.PropertyType);
        }
    }
}
