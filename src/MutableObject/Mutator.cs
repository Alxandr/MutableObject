using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.Core.Interceptor;
using Castle.DynamicProxy;

namespace MutableObject
{
    /// <summary>
    /// The mutator class holds all data for registered mutable types.
    /// </summary>
    /// <typeparam name="T">The mutable type.</typeparam>
    public sealed class Mutator<T> : IInterceptor
    {
        private static ProxyGenerator proxyGenerator = new ProxyGenerator();

        internal Dictionary<string, object> properties;
        internal object baseObject;
        internal T proxyObject;

        internal Mutator(T baseObject)
        {
            Type t = typeof(T);
            if (!t.IsInterface)
                throw new ArgumentException("T must be an interface");


            this.baseObject = baseObject;
            this.properties = new Dictionary<string, object>();

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

        internal IDictionary<string, object> ChangedProperties
        {
            get { return new ReadOnlyDictionary<string, object>(properties); }
        }
       
    }
}
