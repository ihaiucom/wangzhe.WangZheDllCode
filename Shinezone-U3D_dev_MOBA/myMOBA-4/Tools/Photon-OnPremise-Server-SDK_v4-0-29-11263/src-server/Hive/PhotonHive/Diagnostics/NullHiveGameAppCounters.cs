namespace Photon.Hive.Diagnostics
{
    public class NullHiveGameAppCounters : IHiveGameAppCounters
    {
        public static readonly NullHiveGameAppCounters Instance = new NullHiveGameAppCounters();
        public void WebHooksQueueSuccessIncrement()
        {
        }

        public void WebHooksQueueErrorIncrement()
        {
        }

        public void WebHooksHttpSuccessIncrement()
        {
        }

        public void WebHooksHttpTimeoutIncrement()
        {
        }

        public void WebHooksHttpErrorIncrement()
        {
        }

        public void WebHooksHttpExecTimeIncrement(long ticks)
        {
        }

        public void EventCacheTotalEventsIncrement()
        {
        }

        public void EventCacheTotalEventsDecrement()
        {
        }

        public void EventCacheTotalEventsIncrementBy(int value)
        {
        }

        public void EventCacheTotalEventsDecrementBy(int value)
        {
        }

        public void EventCacheSliceCountIncrement()
        {
        }

        public void EventCacheSliceCountDecrement()
        {
        }

        public void EventCacheSliceCountIncrementBy(int value)
        {
        }
    }
}
