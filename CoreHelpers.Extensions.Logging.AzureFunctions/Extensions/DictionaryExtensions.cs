using System;
using System.Collections.Generic;

namespace CoreHelpers.Extensions.Logging.Extensions
{
    internal static class DictionaryExtensions
    {
        public static T GetValueOrDefault<T>(this IDictionary<string, object> dictionary, string key)
        {
            object value;
            if (dictionary != null && dictionary.TryGetValue(key, out value))
            {
                return (T)value;
            }
            return default(T);
        }

        public static void Dump(this IDictionary<string, object> dictionary)
        {
            Console.WriteLine("Content Dictionary:");
            foreach (var k in dictionary)
                Console.WriteLine($"Key: {k.Key}; Value: {k.Value}");
        }
    }
}
