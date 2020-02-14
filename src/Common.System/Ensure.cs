using System;
using System.Collections.Generic;
using System.Linq;

namespace Common.System
{
    public static class Ensure
    {
        public static T NotNull<T>(T arg, string paramName, string message = null) where T : class
        {
            if (arg == null)
            {
                throw new ArgumentNullException(paramName, message);
            }

            return arg;
        }

        public static string NotNullOrEmpty(string arg, string paramName, string message = null)
        {
            NotNull(arg, paramName, message);

            if (arg == string.Empty)
            {
                throw new ArgumentException(message ?? "The string argument should not be empty", paramName);
            }

            return arg;
        }

        public static void GreaterThan<T>(T value, T limit, string paramName, string message = null)
            where T : IComparable<T>
        {
            if (value.CompareTo(limit) <= 0)
            {
                throw new ArgumentException(message ?? $"Should be greater than '{limit}'", paramName);
            }
        }

        public static IReadOnlyCollection<T> NotNullOrEmpty<T>(
            IReadOnlyCollection<T> collection,
            string paramName,
            string message = null)
        {
            NotNull(collection, paramName, message);

            if (!collection.Any())
            {
                throw new ArgumentException(message ?? "The collection should not be empty", paramName);
            }

            return collection;
        }
    }
}
