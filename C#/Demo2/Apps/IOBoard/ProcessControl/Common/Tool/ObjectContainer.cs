using System.Collections.Generic;


namespace Common.Tool
{
    public static class ObjectContainer
    {
        static Dictionary<string, object> objects = new Dictionary<string, object>();

        public static void Set<T>(T obj) => objects[typeof(T).Name] = obj;

        public static T Get<T>()
        {
            try
            {
                return (T)objects[typeof(T).Name];
            }
            catch { }

            return default;
        }
    }
}
