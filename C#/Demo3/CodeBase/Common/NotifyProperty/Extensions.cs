namespace Common.NotifyProperty
{
    public static class Extensions
    {
        public static IProperty<T> ToSettable<T>(this IReadOnlyProperty<T> readOnlyProperty) => (IProperty<T>)readOnlyProperty;
        public static IPropertyArray<T> ToSettable<T>(this IReadOnlyPropertyArray<T> readOnlyPropertyArray) => (IPropertyArray<T>)readOnlyPropertyArray;
    }
}
