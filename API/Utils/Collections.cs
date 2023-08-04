using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace API.Utils
{
    public static class Collections
    {
        public static bool IsEmpty<T>(Collection<T>? collection)
        {
            return collection?.Count == 0;
        }

        private static bool IsEmpty<T>(params T[] array)
        {
            return array?.Length == 0;
        }

        public static bool IsNotEmpty<T>(Collection<T>? collection)
        {
            return !IsEmpty(collection);
        }

        public static bool IsNotEmpty<T>(T[] array)
        {
            return IsEmpty(array);
        }

        public static List<T> EmptyList<T>()
        {
            return new List<T>() { };
        }

        public static List<T> ListOf<T>(params T[] array)
        {
            return array?.ToList() ?? EmptyList<T>();
        }
        public static IList<T> ListNonNullOf<T>(params T[] array)
        {
            return ListOf(array).Where(x => x is not null).ToList() ?? EmptyList<T>();
        }

        public static IList<T> ListOf<T>(Collection<T>? collection)
        {
            return collection?.ToList() ?? EmptyList<T>();
        }

        public static IList<T> ListNonNullOf<T>(Collection<T>? collection)
        {
            return ListOf(collection).Where(x => x is not null).ToList();
        }

        public static IList<K> ListOf<T, K>(Collection<T>? collection, Func<T, K> key)
        {
            return ListOf(collection).Select(key).ToList();
        }

        public static ISet<T> EmptySet<T>()
        {
            return new HashSet<T>() { };
        }

        public static ISet<T> SetOf<T>(params T[] array)
        {
            return array?.ToHashSet() ?? EmptySet<T>();
        }

        public static ISet<T> SetOf<T>(Collection<T>? collection)
        {
            return collection?.ToHashSet() ?? EmptySet<T>();
        }
        public static ISet<K> SetOf<T, K>(Collection<T>? collection, Func<T, K> key)
        {
            return SetOf(collection).Select(key).ToHashSet();
        }

        public static IDictionary<K, V> EmptyMap<K, V>()
        {
            return new Dictionary<K, V>() { };
        }

        public static IDictionary<K, E> MapOf<E, K>(Collection<E> collection, Func<E, K> key) where K : notnull
        {
            return ListOf(collection).ToDictionary(key, item => item);
        }

        public static IDictionary<K, V> MapOf<E, K, V>(Collection<E> collection, Func<E, K> key, Func<E, V> value) where K : notnull
        {
            return ListOf(collection).ToDictionary(key, value);
        }

        public static V? GetMapValue<K, V>(IDictionary<K, V> map, K? key)
        {
            try
            {
                return map.TryGetValue(key, out V value) ? value : default(V);
            }
            catch (Exception e)
            {
                return default(V);
            }
        }
    }
}
