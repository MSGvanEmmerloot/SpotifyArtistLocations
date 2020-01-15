using System;
using System.Collections.Generic;

namespace SpotifyArtistLocations.ExtensionMethods
{
    public static class ExtensionMethods
    {
        public static void ClearIfExists<T>(this List<T> list)
        {
            if (list != null)
            {
                Console.WriteLine("Clearing list of type " + list.GetType());
                list.Clear();
            }
        }

        public static void ClearIfExists<T, T2>(this Dictionary<T, T2> dictionary)
        {
            if (dictionary != null)
            {
                Console.WriteLine("Clearing dictionary of type " + dictionary.GetType());
                dictionary.Clear();
            }
        }

        public static bool ExistsAndContainsKey<T>(this List<T> list, T key)
        {
            if (list != null)
            {
                return list.Contains(key);
            }
            return false;
        }

        public static bool ExistsAndContainsKey<T1, T2>(this Dictionary<T1, T2> dictionary, T1 key)
        {
            if (dictionary != null)
            {
                return dictionary.ContainsKey(key);
            }
            return false;
        }

        public static void AddUnique<T>(this List<T> list, T key)
        {
            if(list == null) { return; }

            if (!list.Contains(key))
            {
                list.Add(key);
            }
        }

        public static void AddUnique<T1, T2>(this Dictionary<T1, T2> dictionary, T1 key, T2 value)
        {
            if (dictionary == null) { return; }

            if (!dictionary.ContainsKey(key))
            {
                dictionary.Add(key, value);
            }
        }

        public static void Update<T1, T2>(this Dictionary<T1, T2> dictionary, T1 key, T2 value)
        {
            if (dictionary == null) { return; }

            if (dictionary.ContainsKey(key))
            {
                dictionary[key] = value;
            }
        }
    }
}
