namespace DependencyInjectionContainer.Utils
{
    public static class GenericConverter
    {
        public static object ConvertToTypedEnumerable(List<object> values, Type elementType)
        {
            var genericListType = typeof(List<>).MakeGenericType(elementType);
            var typedCollection = Activator.CreateInstance(genericListType);

            var addMethod = genericListType.GetMethod("Add");
            foreach (var value in values)
            {
                addMethod!.Invoke(typedCollection, [value]);
            }

            return typedCollection!.GetType().GetMethod("AsReadOnly")!.Invoke(typedCollection, null)!;
        }
    }
}
