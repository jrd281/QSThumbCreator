using System.Linq;
using Prism.Events;
using Prism.Mvvm;
using QSThumbCreator.Events;
using QSThumbCreator.Models.Thumb;
using QSThumbCreator.Processor.Events;

namespace QSThumbCreator.Views.SOW
{
    public class StatementOfWorkViewModel : BindableBase
    {
        private readonly QlikThumbModel _qlikThumbModel;
        private readonly IEventAggregator _eventAggregator;
        private string _contentLibraryString;
        private string _appsListString;
        private bool _contentLibraryVisible;

        public StatementOfWorkViewModel(QlikThumbModel qlikThumbModel, IEventAggregator eventAggregator)
        {
            _qlikThumbModel = qlikThumbModel;
            _eventAggregator = eventAggregator;
        }

        public void RefreshText()
        {
            ContentLibraryString = "";
            if (_qlikThumbModel.TaskType != null &&
                _qlikThumbModel.TaskType.Equals(QlikThumbModel.TaskContentDirectorySave))
            {
                ContentLibraryString = _qlikThumbModel.ContentLibrary?.Name;

                ContentLibraryVisible = true;
            }
            else if (_qlikThumbModel.TaskType != null &&
                     _qlikThumbModel.TaskType.Equals(QlikThumbModel.TaskLocalSaveOnly))
            {
                ContentLibraryVisible = false;
            }

            var selectedApps = _qlikThumbModel.SelectedQlikApps.ToList();

            selectedApps.Sort((app1, app2) =>
            {
                var strCompare = string.CompareOrdinal(app1.StreamName, app2.StreamName);
                if (strCompare == 0)
                {
                    return string.CompareOrdinal(app1.Name, app2.Name);
                }

                return strCompare;
            });
            var appAndStreamNames = selectedApps.Select(s => "(" + s.StreamName + ")-" + s.Name + "\n");
            AppsListString = string.Join("\n", appAndStreamNames);
        }

        public string ContentLibraryString
        {
            get => _contentLibraryString;
            set => SetProperty(ref _contentLibraryString, value);
        }

        public string AppsListString
        {
            get => _appsListString;
            set => SetProperty(ref _appsListString, value);
        }

        public bool ContentLibraryVisible
        {
            get => _contentLibraryVisible;
            set => SetProperty(ref _contentLibraryVisible, value);
        }

        public void HandlePrevClick()
        {
            _eventAggregator.GetEvent<NavEvent>().Publish(NavEvent.SowPrev);
        }

        public void HandleGoClick()
        {
            _eventAggregator.GetEvent<StartProcessingEvent>().Publish(StartProcessingEvent.Start);
        }
    }
}