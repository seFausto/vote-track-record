using System;

namespace Extensions
{ 
    public static class ClassExtensions
    {
        /// <summary>
        /// Convenience method for ensuring a return is not null, basically shortcuts the null propogator (??)
        /// </summary>
        /// <typeparam name="T">The type of object</typeparam>
        /// <param name="obj">The object to new up if null</param>
        /// <returns>The object passed in or a new instance of it's type</returns>
        public static T OrNew<T>(this T obj) where T : class, new()
        {
            return obj ?? new T();
        }
    }

    public static class EnumerableExtensions
    {
        /// <summary>
        /// Convenience method for checking an enumerable is not null and has Any() records
        /// Prevents you from doing list?.Any() ?? false
        /// </summary>
        /// <param name="collection">The collection to check</param>
        /// <returns>True if the collection is not null and has at least 1 item</returns>
        public static bool HasItems<T>(this IEnumerable<T> collection)
        {
            return collection != null && collection.Any();
        }
    }
}