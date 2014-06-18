using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ParallelTest
{
    class Program
    {
        static void Main(string[] args)
        {
            ThreadLocal<string> ThreadName = new ThreadLocal<string>(() =>
                {
                    return "Thread" + Thread.CurrentThread.ManagedThreadId;
                });

            Action action = () =>
                {
                    bool repeat = ThreadName.IsValueCreated;
                    Thread.Sleep(1000);
                    Console.WriteLine("ThreadName={0}{1}", ThreadName.Value, repeat ? "(repeat)" : "");
                };

            Parallel.Invoke(new Action[]{action, action, action, action, action, action, action, action});

            ThreadName.Dispose();
            Console.ReadKey();
        }
    }
}
