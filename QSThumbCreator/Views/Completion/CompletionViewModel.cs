using Prism.Events;
using Prism.Mvvm;
using QSThumbCreator.Processor.Events;

namespace QSThumbCreator.Views.Completion
{
    public class CompletionViewModel : BindableBase
    {
        private string _streamsCount;
        private string _appsCount;
        private string _sheetsCount;
        private string _timeElapsed;

        public CompletionViewModel(IEventAggregator _eventAggregator)
        {
            _eventAggregator.GetEvent<EndProcessingEvent>().Subscribe(HandleEndProcessingEvent);
        }

        public void HandleEndProcessingEvent(EndProcessingPayload payload)
        {
            StreamsCount = payload.StreamsProcessedCount.ToString();
            AppsCount = payload.AppsProcessedCount.ToString();
            SheetsCount = payload.SheetsProcessedCount.ToString();

            var timeSpanString = "";
            if (payload.ElapsedTimeSpan.Hours > 0)
            {
                timeSpanString += payload.ElapsedTimeSpan.Hours.ToString() +
                                  (payload.ElapsedTimeSpan.Hours == 1 ? "Hour" : "Hours");
                timeSpanString += " ";
            }

            if (payload.ElapsedTimeSpan.Hours > 0 || payload.ElapsedTimeSpan.Minutes > 0)
            {
                timeSpanString += payload.ElapsedTimeSpan.Minutes.ToString() +
                                  (payload.ElapsedTimeSpan.Minutes == 1 ? " Minute " : " Minutes ");
                timeSpanString += " ";
            }

            timeSpanString += payload.ElapsedTimeSpan.Seconds.ToString() +
                              (payload.ElapsedTimeSpan.Seconds == 1 ? " Second " : " Seconds");
            TimeElapsed = timeSpanString;
        }

        public string StreamsCount
        {
            get => _streamsCount;
            set => SetProperty(ref _streamsCount, value);
        }

        public string AppsCount
        {
            get => _appsCount;
            set => SetProperty(ref _appsCount, value);
        }

        public string SheetsCount
        {
            get => _sheetsCount;
            set => SetProperty(ref _sheetsCount, value);
        }

        public string TimeElapsed
        {
            get => _timeElapsed;
            set => SetProperty(ref _timeElapsed, value);
        }
    }
}