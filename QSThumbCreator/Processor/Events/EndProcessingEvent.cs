using System;
using Prism.Events;

namespace QSThumbCreator.Processor.Events
{
    public class EndProcessingEvent : PubSubEvent<EndProcessingPayload>
    {
    }

    public class EndProcessingPayload
    {
        public int StreamsProcessedCount { get; set; }
        public int AppsProcessedCount { get; set; }
        public int SheetsProcessedCount { get; set; }
        public TimeSpan ElapsedTimeSpan { get; set; }
    }
}