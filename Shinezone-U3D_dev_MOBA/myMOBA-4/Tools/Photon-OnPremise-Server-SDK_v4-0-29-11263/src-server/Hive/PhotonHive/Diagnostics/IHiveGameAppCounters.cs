namespace Photon.Hive.Diagnostics
{
    public interface IHiveGameAppCounters
    {
        void WebHooksQueueSuccessIncrement();
        void WebHooksQueueErrorIncrement();
        void WebHooksHttpSuccessIncrement();
        void WebHooksHttpTimeoutIncrement();
        void WebHooksHttpErrorIncrement();
        void WebHooksHttpExecTimeIncrement(long ticks);

        void EventCacheTotalEventsIncrement();
        void EventCacheTotalEventsDecrement();
        void EventCacheTotalEventsIncrementBy(int value);
        void EventCacheTotalEventsDecrementBy(int value);
        void EventCacheSliceCountIncrement();
        void EventCacheSliceCountDecrement();

        void EventCacheSliceCountIncrementBy(int value);
    }
}
