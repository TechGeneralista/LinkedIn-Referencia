using System;
using System.Collections.Generic;


namespace Common
{
    public class Container
    {
        public static Container Instance { get; } = new Container();


        Dictionary<Type, Func<object>> dictionary = new Dictionary<Type, Func<object>>();
        object lockObject = new object();


        public void Register<T>(Func<object> func)
        {
            lock(lockObject)
            {
                dictionary[typeof(T)] = func;
            }
        }

        public T Resolve<T>()
        {
            lock(lockObject)
            {
                Type type = typeof(T);

                if (dictionary.ContainsKey(type))
                    return (T)dictionary[type].Invoke();

                return default;
            }
        }
    }
}
