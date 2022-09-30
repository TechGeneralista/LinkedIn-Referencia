using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows;
using System.Windows.Media;


namespace Common
{
    public static class Extension
    {
        public static T GetVisualParent<T>(this DependencyObject child, string name = null) where T : DependencyObject
        {
            if (child == null)
                return default;

            DependencyObject parent = VisualTreeHelper.GetParent(child);

            if (parent is T)
            {
                if (name == null)
                    return (T)parent;
                else
                {
                    FrameworkElement fe = child as FrameworkElement;
                    if (fe != null && fe.Name == name)
                        return (T)parent;
                }
            }

            if (parent != null)
                return GetVisualParent<T>(parent);

            return default;
        }

        public static DependencyObject GetVisualChild<T>(this DependencyObject parent, string name = null)
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(parent, i);

                if (child is T)
                {
                    if (name == null)
                        return child;
                    else
                    {
                        FrameworkElement fe = child as FrameworkElement;
                        if (fe != null && fe.Name == name)
                            return child;
                    }
                }

                if (VisualTreeHelper.GetChildrenCount(child) > 0)
                {
                    DependencyObject ret = GetVisualChild<T>(child, name);

                    if (ret != null)
                        return ret;
                }
            }

            return default;
        }

        public static string RemoveSpace(this string s)
            => s.Replace(" ", string.Empty);

        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            if (source == null)
                return;

            foreach (T element in source)
                action(element);
        }

        public static void ForEach(this IEnumerable source, Action<object> action)
        {
            if (source == null)
                return;

            foreach (object element in source)
                action(element);
        }

        public static byte[] Serialize(this object obj)
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();

            using (MemoryStream memoryStream = new MemoryStream())
            {
                binaryFormatter.Serialize(memoryStream, obj);
                return memoryStream.ToArray();
            }
        }

        public static T Deserialize<T>(this byte[] data)
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();

            using (MemoryStream memoryStream = new MemoryStream(data))
                return (T)binaryFormatter.Deserialize(memoryStream);
        }
    }
}
