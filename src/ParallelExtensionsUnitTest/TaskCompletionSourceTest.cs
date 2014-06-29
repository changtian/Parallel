using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace ParallelExtensionsUnitTest
{
    public class TaskCompletionSourceTest
    {
        public static Task<T> RunAsync<T>(Func<T> function)
        {
            if (function == null) throw new ArgumentNullException("function");
            var tcs = new TaskCompletionSource<T>();
            ThreadPool.QueueUserWorkItem(_ =>
            {
                try
                {
                    T result = function();
                    tcs.SetResult(result);
                }
                catch (Exception exc)
                {
                    tcs.SetException(exc);
                }
            });
            return tcs.Task;
        }
    }
}
