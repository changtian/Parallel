using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks.Schedulers;
using System.Threading.Tasks;
using System.Threading;

namespace ParallelExtensionsUnitTest
{
    public class TaskScheduleTest
    {
        static void Main()
        {
            LimitedConcurrencyLevelTaskScheduler lcts = new LimitedConcurrencyLevelTaskScheduler(2);
            List<Task> tasks = new List<Task>();

            TaskFactory factory = new TaskFactory(lcts);
            CancellationTokenSource cts = new CancellationTokenSource();

            Object lockObj = new Object();
            int outputItem = 0;
            for (int tCtr = 0; tCtr <= 4; tCtr++)
            {
                int iteration = tCtr;
                Task t = factory.StartNew(() =>
                {
                    for (int i = 0; i < 1000; i++)
                    {
                        Console.Write("{0} in task t-{1} on thread {2} ", i, iteration, Thread.CurrentThread.ManagedThreadId);

                        outputItem++;
                        if (outputItem % 3 == 0)
                            Console.WriteLine();
                    }
                }, cts.Token);
                tasks.Add(t);
            }

            for (int tCtr = 0; tCtr < 4; tCtr++)
            {
                int iteration = tCtr;
                Task t1 = factory.StartNew(() =>
                {
                    for (int outer = 0; outer <= 10; outer++)
                    {
                        for (int i = 0x21; i <= 0x7E; i++)
                        {
                            lock (lockObj)
                            {
                                Console.Write("'{0}' in task t1-{1} on thread {2} ", Convert.ToChar(i), iteration, Thread.CurrentThread.ManagedThreadId);
                                outputItem++;
                                if (outputItem % 3 == 0)
                                    Console.WriteLine();
                            }
                        }
                    }
                }, cts.Token);
                tasks.Add(t1);
            }

            Task.WaitAll(tasks.ToArray());
            Console.WriteLine("\n\nSuccessful completion.");
        }
    }

    public class LimitedConcurrencyLevelTaskScheduler : TaskScheduler
    {
        [ThreadStatic]
        private static bool _currentThreadIsProcessiongItems;

        private readonly LinkedList<Task> _tasks = new LinkedList<Task>();//protected by lock(_tasks)

        private readonly int _maxDegreeOfParallelism;

        private int _delegatesQueuedOrRunning = 0;

        public LimitedConcurrencyLevelTaskScheduler(int maxDegreeOfParallelism)
        {
            if (maxDegreeOfParallelism < 1) throw new ArgumentOutOfRangeException("maxDegreeOfParallelism");
            _maxDegreeOfParallelism = maxDegreeOfParallelism;
        }

        protected sealed override void QueueTask(Task task)
        {
            lock (_tasks)
            {
                _tasks.AddLast(task);
                if (_delegatesQueuedOrRunning < _maxDegreeOfParallelism)
                {
                    ++_delegatesQueuedOrRunning;
                    NotifyThreadPoolOfPendingWork();
                }
            }
        }

        private void NotifyThreadPoolOfPendingWork()
        {
            ThreadPool.UnsafeQueueUserWorkItem(_ => {
                _currentThreadIsProcessiongItems = true;
                try
                {
                    while (true)
                    {
                        Task item;
                        lock (_tasks)
                        {
                            if (_tasks.Count == 0)
                            {
                                --_delegatesQueuedOrRunning;
                                break;
                            }

                            item = _tasks.First.Value;
                            _tasks.RemoveFirst();
                        }
                        base.TryExecuteTask(item);
                    }
                }
                finally { _currentThreadIsProcessiongItems = false; }
            },null);
        }

        protected sealed override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
        {
            if (!_currentThreadIsProcessiongItems) return false;

            if (taskWasPreviouslyQueued)
                if (TryDequeue(task))
                    return base.TryExecuteTask(task);
                else
                    return false;
            else
                return base.TryExecuteTask(task);
        }

        protected sealed override bool TryDequeue(Task task)
        {
            lock (_tasks) return _tasks.Remove(task);
        }

        // Gets the maximum concurrency level supported by this scheduler.  
        public sealed override int MaximumConcurrencyLevel { get { return _maxDegreeOfParallelism; } }

        protected sealed override IEnumerable<Task> GetScheduledTasks()
        {
            bool lockTaken = false;
            try
            {
                Monitor.TryEnter(_tasks, ref lockTaken);
                if (lockTaken) return _tasks;
                else throw new NotSupportedException();
            }
            finally
            {
                if (lockTaken) Monitor.Exit(_tasks);
            }
        }
    }
}
