using Prism.Events;

namespace QSThumbCreator.Processor.Events
{
    public class StartProcessingEvent : PubSubEvent<string>
    {
        public static string Start = "START";
    }
}