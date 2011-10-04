using System;
using System.Reflection;
using Castle.Core.Interceptor;

namespace MutableObject
{
    class MutatorInterceptor<T> : IInterceptor
    {
        Mutator<T> mutator;
        public MutatorInterceptor(Mutator<T> mutator)
        {
            this.mutator = mutator;
        }

        public void Intercept(IInvocation invocation)
        {
            string name = invocation.Method.Name.Substring(4);
            if (invocation.Method.Name.StartsWith("set_", StringComparison.OrdinalIgnoreCase))
            {

                if (mutator.baseObject != null)
                {
                    if (!mutator.origProperties.ContainsKey(name))
                    {
                        var property = mutator.baseObject.GetType().GetProperty(name, BindingFlags.Instance | BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.NonPublic);
                        if (property != null)
                            mutator.origProperties.Add(name, property.GetValue(mutator.baseObject, new object[0]));
                    }
                    invocation.Method.Invoke(mutator.baseObject, invocation.Arguments);
                }

                mutator.properties[name] = invocation.Arguments[0];
            }
            //invocation.Proceed();
            if (invocation.Method.Name.StartsWith("get_", StringComparison.OrdinalIgnoreCase))
            {
                if (name == "Mutator")
                {
                    invocation.ReturnValue = mutator;
                    return;
                }
                if (mutator.baseObject != null)
                    invocation.ReturnValue = invocation.Method.Invoke(mutator.baseObject, invocation.Arguments);
                else
                {
                    if (!mutator.properties.ContainsKey(name))
                        throw new InvalidOperationException("Property " + name + " not set.");
                    invocation.ReturnValue = mutator.properties[name];
                }
            }
        }
    }
}
