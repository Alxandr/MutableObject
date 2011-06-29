using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
                    invocation.Method.Invoke(mutator.baseObject, invocation.Arguments);

                mutator.properties[name] = invocation.Arguments[0];
            }
            //invocation.Proceed();
            if (invocation.Method.Name.StartsWith("get_", StringComparison.OrdinalIgnoreCase))
            {
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
