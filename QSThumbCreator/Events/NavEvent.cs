using Prism.Events;

namespace QSThumbCreator.Events
{
    public class NavEvent : PubSubEvent<string>
    {
        public static string LoginNext = "LOGIN_NEXT";

        public static string StreamsAppsSelPrev = "STREAMS_APPS_SEL_PREV";
        public static string StreamsAppsSelNext = "STREAMS_APPS_SEL_NEXT";

        public static string ContentLibraryPrev = "CONTENT_LIBRARY_PREV";
        public static string ContentLibraryNext = "CONTENT_LIBRARY_NEXT";

        public static string OptionsPrev = "OPTIONS_PREV";
        public static string OptionsNext = "OPTIONS_NEXT";

        public static string SowPrev = "SOW_PREV";
    }
}