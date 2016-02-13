using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Hypermedia
{
    public static class TypeHelper
    {
        /// <summary>
        /// Returns a value indicating whether or not he given type is to be considered a reference type.
        /// </summary>
        /// <param name="type">The type to check whether it is a reference type.</param>
        /// <returns>true if the given type is to be considered a reference type, false if not.</returns>
        public static bool IsReferenceType(Type type)
        {
            return type != typeof(string) && type.GetTypeInfo().IsValueType == false && type.GetTypeInfo().IsPrimitive == false;
        }

        /// <summary>
        /// Gets the underlying ICollection type.
        /// </summary>
        /// <param name="type">The type to extract the underlying collection type (if any).</param>
        /// <returns>The underlying collection type.</returns>
        public static Type GetCollectionType(Type type)
        {
            return GetCollectionType(type.GetTypeInfo());
        }

        /// <summary>
        /// Gets the underlying ICollection type.
        /// </summary>
        /// <param name="type">The type to extract the underlying collection type (if any).</param>
        /// <returns>The underlying collection type.</returns>
        public static Type GetCollectionType(TypeInfo type)
        {
            Type collectionType;
            if (TryGetCollectionType(type, out collectionType))
            {
                return collectionType;
            }

            return null;
        }

        /// <summary>
        /// Attempt to get the ICollection type from the given base type.
        /// </summary>
        /// <param name="type">The base type info to get the collection type from.</param>
        /// <param name="collectionType">The collection type for the given base type.</param>
        /// <returns>true if a collection type was found, false if not.</returns>
        public static bool TryGetCollectionType(Type type, out Type collectionType)
        {
            return TryGetCollectionType(type.GetTypeInfo(), out collectionType);
        }

        /// <summary>
        /// Attempt to get the ICollection type from the given base type.
        /// </summary>
        /// <param name="type">The base type info to get the collection type from.</param>
        /// <param name="collectionType">The collection type for the given base type.</param>
        /// <returns>true if a collection type was found, false if not.</returns>
        public static bool TryGetCollectionType(TypeInfo type, out Type collectionType)
        {
            collectionType =
                type.ImplementedInterfaces
                    .FirstOrDefault(t => t.GetTypeInfo().IsGenericType && t.GetGenericTypeDefinition() == typeof(ICollection<>));

            return collectionType != null;
        }

        /// <summary>
        /// Returns a value indicating whether or not the given type is an IEnumerable type.
        /// </summary>
        /// <param name="type">The type to test against.</param>
        /// <returns>true if the type is enumerable, false if not.</returns>
        public static bool IsEnumerable(Type type)
        {
            return GetEnumerableType(type) != null;
        }

        /// <summary>
        /// Gets the underlying IEnumerable<T> type.
        /// </summary>
        /// <param name="type">The type to extract the underlying enumerable type (if any).</param>
        /// <returns>The underlying enumerable type.</returns>
        public static Type GetEnumerableType(Type type)
        {
            return GetEnumerableType(type.GetTypeInfo());
        }

        /// <summary>
        /// Gets the underlying IEnumerable<T> type.
        /// </summary>
        /// <param name="type">The type to extract the underlying enumerable type (if any).</param>
        /// <returns>The underlying enumerable type.</returns>
        static Type GetEnumerableType(TypeInfo type)
        {
            Type enumerableType;
            if (TryGetEnumerableType(type, out enumerableType))
            {
                return enumerableType;
            }

            return null;
        }

        /// <summary>
        /// Attempt to get the IEnumerable<T></T> type from the given base type.
        /// </summary>
        /// <param name="type">The base type info to get the enumerable type from.</param>
        /// <param name="enumerableType">The enumerable type for the given base type.</param>
        /// <returns>true if a enumerable type was found, false if not.</returns>
        public static bool TryGetEnumerableType(Type type, out Type enumerableType)
        {
            return TryGetEnumerableType(type.GetTypeInfo(), out enumerableType);
        }

        /// <summary>
        /// Attempt to get the IEnumerable<T></T> type from the given base type.
        /// </summary>
        /// <param name="type">The base type info to get the enumerable type from.</param>
        /// <param name="enumerableType">The enumerable type for the given base type.</param>
        /// <returns>true if a enumerable type was found, false if not.</returns>
        static bool TryGetEnumerableType(TypeInfo type, out Type enumerableType)
        {
            enumerableType =
                type.ImplementedInterfaces
                    .FirstOrDefault(t => t.GetTypeInfo().IsGenericType && t.GetGenericTypeDefinition() == typeof(IEnumerable<>));

            return enumerableType != null;
        }
        
        /// <summary>
        /// Gets the underlying element type.
        /// </summary>
        /// <param name="type">The type to extract the underlying type (if any).</param>
        /// <returns>The underlying element type.</returns>
        public static Type GetUnderlyingType(Type type)
        {
            return GetUnderlyingType(type.GetTypeInfo());
        }

        /// <summary>
        /// Gets the underlying element type.
        /// </summary>
        /// <param name="type">The type to extract the underlying type (if any).</param>
        /// <returns>The underlying element type.</returns>
        /// <returns>The underlying type.</returns>
        public static Type GetUnderlyingType(TypeInfo type)
        {
            if (type.IsArray)
            {
                return type.GetElementType();
            }

            if (type.IsGenericType == false)
            {
                return type.AsType();
            }

            Type enumerableType;
            if (TryGetEnumerableType(type, out enumerableType))
            {
                return enumerableType.GetTypeInfo().GenericTypeArguments[0]; ;
            }

            return type.AsType();
        }
    }
}
