using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MutableObject
{
    /// <summary>
    /// A helper-class for creating and managing mutable objects.
    /// </summary>
    public static class Mutate
    {
        /// <summary>
        /// Create a mutable object without any data.
        /// </summary>
        /// <typeparam name="T">The type of the mutable object.</typeparam>
        /// <returns>A mutable object of type T.</returns>
        public static T CreateMutableObject<T>()
        {
            return CreateMutableObject<T>(default(T));
        }

        /// <summary>
        /// Creates a mutable object with specified data.
        /// </summary>
        /// <typeparam name="T">The type of the mutable object.</typeparam>
        /// <param name="baseObj">The data.</param>
        /// <returns>A mutable object of type T.</returns>
        public static T CreateMutableObject<T>(T baseObj)
        {
            Mutator<T> m = new Mutator<T>(baseObj);
            return m.Object;
        }

        /// <summary>
        /// Checks if an object is mutable.
        /// </summary>
        /// <param name="obj">An object.</param>
        /// <returns>True if object is mutable, otherwise false.</returns>
        public static bool IsMutable(object obj)
        {
            return obj.GetType().GetInterfaces().Any(iface =>
            {
                if (iface.IsGenericType && iface.GetGenericTypeDefinition() == typeof(IMutable<>))
                    return true;

                return false;
            });
        }

        /// <summary>
        /// Gets all the changed properties and their values.
        /// </summary>
        /// <param name="obj">The object to get the changed values for.</param>
        /// <returns>A dictionary containing the properties and their values.</returns>
        public static IDictionary<string, object> GetChangedProperties(object obj)
        {
            if (!IsMutable(obj))
                throw new ArgumentException("Object must be mutable");

            Type interf = obj.GetType().GetInterfaces().Where(iface => iface.IsGenericType && iface.GetGenericTypeDefinition() == typeof(IMutable<>)).First();
            object mutator = interf.GetProperty("Mutator").GetGetMethod().Invoke(obj, new object[0]);
            var changedProp = mutator.GetType().GetProperty("ChangedProperties", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy | BindingFlags.Instance | BindingFlags.InvokeMethod);

            object dict = changedProp.GetValue(mutator, new object[0]);

            return (IDictionary<string, object>)dict;
        }

        /// <summary>
        /// Gets all the original properties and their values.
        /// </summary>
        /// <param name="obj">The object to get the changed values for.</param>
        /// <returns>A dictionary containing the properties and their values.</returns>
        public static IDictionary<string, object> GetOriginalProperties(object obj)
        {
            if (!IsMutable(obj))
                throw new ArgumentException("Object must be mutable");

            Type interf = obj.GetType().GetInterfaces().Where(iface => iface.IsGenericType && iface.GetGenericTypeDefinition() == typeof(IMutable<>)).First();
            object mutator = interf.GetProperty("Mutator").GetGetMethod().Invoke(obj, new object[0]);
            var changedProp = mutator.GetType().GetProperty("OriginalProperties", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy | BindingFlags.Instance | BindingFlags.InvokeMethod);

            object dict = changedProp.GetValue(mutator, new object[0]);

            return (IDictionary<string, object>)dict;
        }

        /// <summary>
        /// Gets a value indicating whether or not the MutableObject is bound to a base object.
        /// </summary>
        /// <param name="obj">The object to check is bound or not.</param>
        /// <returns>A value indicating whether or not the object is bound.</returns>
        public static bool IsBound(object obj)
        {
            if (!IsMutable(obj))
                throw new ArgumentException("Object must be mutable");

            Type interf = obj.GetType().GetInterfaces().Where(iface => iface.IsGenericType && iface.GetGenericTypeDefinition() == typeof(IMutable<>)).First();
            object mutator = interf.GetProperty("Mutator").GetGetMethod().Invoke(obj, new object[0]);
            var baseProp = mutator.GetType().GetProperty("BaseObject", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy | BindingFlags.Instance | BindingFlags.InvokeMethod);

            object baseObj = baseProp.GetValue(mutator, new object[0]);

            return baseObj != null;
        }

        /// <summary>
        /// Returns whether or not named properties are set on a unbound object.
        /// </summary>
        /// <param name="obj">The object to check properties on.</param>
        /// <param name="properties">The list of properties to check.</param>
        /// <returns>True if all properties are set; otherwise false.</returns>
        public static bool PropertiesIsSet(object obj, params string[] properties)
        {
            if (IsBound(obj))
                return true;

            var dict = GetChangedProperties(obj);
            return properties.All(prop => dict.ContainsKey(prop));
        }

        /// <summary>
        /// Resets the specified object.
        /// </summary>
        /// <param name="obj">The object.</param>
        public static void Reset(object obj)
        {
            if (!IsMutable(obj))
                throw new ArgumentException("Object must be mutable");

            Type interf = obj.GetType().GetInterfaces().Where(iface => iface.IsGenericType && iface.GetGenericTypeDefinition() == typeof(IMutable<>)).First();
            object mutator = interf.GetProperty("Mutator").GetGetMethod().Invoke(obj, new object[0]);
            var reset = mutator.GetType().GetMethod("Reset", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy | BindingFlags.Instance | BindingFlags.InvokeMethod);
            reset.Invoke(mutator, new object[0]);
        }
    }
}
