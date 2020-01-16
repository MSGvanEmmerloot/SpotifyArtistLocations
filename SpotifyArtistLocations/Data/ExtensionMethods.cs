using System;
using System.Collections.Generic;

namespace SpotifyArtistLocations.ExtensionMethods
{
    public static class ExtensionMethods
    {
        // ClearIfExists clears list/dictionary content if it exists
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
        // ClearIfExists

        // ExistsAndContainsKey returns the result of list.Contains or dictionary.ContainsKey if the list / dictionary exists
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
        // ExistsAndContainsKey

        // AddUnique adds an entry to a list/dictionary if the list/dictionary exists and it does not yet contain the entry
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
        // AddUnique

        // AddUniqueByObjectProperty adds an entry to a list if the list exists and it does not yet contain an entry with a property value matching the given property value of the given key
        // The given property string should be the name of a property of the given key
        public static void AddUniqueByObjectProperty<T>(this List<T> list, T key, string property)
        {
            if (list == null) { return; }

            Console.WriteLine("Processing " + key);

            Type type = key.GetType();
            System.Reflection.PropertyInfo propertyInfo = type.GetProperty(property);
            if (propertyInfo == null)
            {
                Console.WriteLine(" Object of type " + type + " does not contain property " + property);
                return;
            }
            Console.WriteLine("Found " + property + " property in object of type " + type);

            object? keyVal = propertyInfo.GetValue(key, null);

            for (int i = 0; i < list.Count; i++)
            {
                T entry = list[i];
                object? entryVal = propertyInfo.GetValue(entry, null);

                // Key already present in list
                if (entryVal == keyVal)
                {
                    Console.WriteLine("Object with " + property + " " + entryVal + " already in list");
                    return;
                }
            }

            list.Add(key);
            Console.WriteLine("Added " + key + " to list");
        }
        // AddUniqueByObjectProperty

        // Update updates a dictionary entry if the dictionary exists and the key exists
        public static void Update<T1, T2>(this Dictionary<T1, T2> dictionary, T1 key, T2 value)
        {
            if (dictionary == null) { return; }

            if (dictionary.ContainsKey(key))
            {
                dictionary[key] = value;
            }
        }
        // Update
    }
}
