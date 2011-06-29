using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.Core.Interceptor;

namespace MutableObject
{
    [Serializable]
    class CallLoggingInterceptor : IInterceptor
    {
        private int _indention;

        #region IInterceptor Members

        public void Intercept(IInvocation invocation)
        {
            try
            {
                _indention++;
                Count++;

                Console.WriteLine("{0}Intercepting: {1}", new string('\t', _indention), invocation.Method.Name);
                invocation.Proceed();
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("Exception in method {0}{1}{2}", invocation.Method.Name,
                                                Environment.NewLine, ex.Message));
                throw;
            }
            finally
            {
                _indention--;
            }
        }

        public int Count { get; private set; }

        #endregion
    }
}
