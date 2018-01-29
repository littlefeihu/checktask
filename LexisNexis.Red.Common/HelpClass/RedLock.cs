using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LexisNexis.Red.Common.HelpClass
{
    public class RedLock
    {
        private int currentThreadCount = 0;
        private int limitCount = 0;
        private static AutoResetEvent autoResetEvent = new AutoResetEvent(false);
        public RedLock(int limitCount)
        {
            this.limitCount = limitCount;
        }
        public Task Enter()
        {
            return Task.Run(() =>
            {
                if (Interlocked.Increment(ref currentThreadCount) <= this.limitCount)
                    return;
                autoResetEvent.WaitOne();
            });

        }

        public void Release()
        {
            if (Interlocked.Decrement(ref currentThreadCount) >= this.limitCount)
                autoResetEvent.Set();
        }
    }
}
