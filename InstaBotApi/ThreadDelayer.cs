using System;
using System.Threading;

namespace InstaBotApi
{
    public enum WaitingPeriod
    {
        Short,
        Medium,
        Long
    }

    static class ThreadDelayer
    {
        public static void WaitSomeTime(WaitingPeriod waitingPeriod = WaitingPeriod.Medium)
        {
            int delayFactor = 2;
            switch (waitingPeriod)
            {
                case WaitingPeriod.Short:
                    Thread.Sleep(delayFactor * 500);
                    break;
                case WaitingPeriod.Medium:
                    Thread.Sleep(delayFactor * 3000);
                    break;
                case WaitingPeriod.Long:
                    Thread.Sleep(delayFactor * 6000);
                    break;
                default:
                    throw new NotSupportedException();
            }
        }
    }
}
