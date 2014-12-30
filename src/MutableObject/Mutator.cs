using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reflection;
using Castle.DynamicProxy;

namespace MutableObject
{
    /// <summary>
    /// The mutator class holds all data for registered mutable types.
    /// </summary>
    /// <typeparam name="T">The mutable type.</typeparam>
    public sealed class Mutator<T>
    {
        private static ProxyGenerator proxyGenerator = new ProxyGenerator();

        internal Dictionary<string, object> properties;
        internal Dictionary<string, object> origProperties;
        internal object baseObject;
        internal T proxyObject;

        internal Mutator(T baseObject)
        {
            Type t = typeof(T);
            if (!t.IsInterface)
                throw new ArgumentException("T must be an interface");


            this.baseObject = baseObject;
            this.properties = new Dictionary<string, object>();
            this.origProperties = new Dictionary<string, object>();

            this.proxyObject = (T)proxyGenerator.CreateInterfaceProxyWithoutTarget(typeof(T),
                new Type[] { typeof(IMutable<>).MakeGenericType(typeof(T)) },
#if (DEBUG && LOG_INTERCEPTION)
                new CallLoggingInterceptor(),
#endif
 new MutatorInterceptor<T>(this));
        }

        internal T Object
        {
            get { return proxyObject; }
        }

        internal IImmutableDictionary<string, object> ChangedProperties
        {
            get { return properties.ToImmutableDictionary(); }
        }

        internal IImmutableDictionary<string, object> OriginalProperties
        {
            get { return origProperties.ToImmutableDictionary(); }
        }

        internal Object BaseObject
        {
            get { return baseObject; }
        }

        internal void Reset()
        {
            this.properties = new Dictionary<string, object>();
            foreach (var prop in this.origProperties)
            {
                var property = baseObject.GetType().GetProperty(prop.Key, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                if (property != null)
                    property.SetValue(baseObject, prop.Value, new object[0]);
            }
            this.origProperties = new Dictionary<string, object>();
        }
    }
}
