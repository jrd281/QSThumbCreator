using System;
using System.Collections.ObjectModel;
using System.Linq;
using Prism.Events;
using Prism.Mvvm;
using QSThumbCreator.Events;
using QSThumbCreator.Models.Qlik;
using QSThumbCreator.Models.Thumb;
using QSThumbCreator.Services.QlikEngine;
using Serilog;

namespace QSThumbCreator.Views.ContentLibrary
{
    public class ContentLibraryChooserViewModel : BindableBase
    {
        private readonly QlikEngineService _qlikEngineService;
        private readonly QlikThumbModel _qlikThumbModel;
        private readonly IEventAggregator _eventAggregator;
        private QlikContentLibrary _selectedQlikContentLibrary;
        private readonly ILogger _log = Log.ForContext<ContentLibraryChooserViewModel>();
        private bool _nextEnabled;

        public ContentLibraryChooserViewModel(QlikEngineService qlikEngineService,
            QlikThumbModel qlikThumbModel,
            IEventAggregator eventAggregator)
        {
            _qlikEngineService = qlikEngineService;
            _qlikThumbModel = qlikThumbModel;
            _eventAggregator = eventAggregator;

            QlikContentLibraries = new ObservableCollection<QlikContentLibrary>();
        }

        public ObservableCollection<QlikContentLibrary> QlikContentLibraries { get; set; }

        public async void HandleReloadContentLibraries()
        {
            try
            {
                var retrieveContentLibrariesAsync = _qlikEngineService.RetrieveContentLibrariesAsync();
                await retrieveContentLibrariesAsync;

                var qlikContentLibraries = retrieveContentLibrariesAsync.Result;

                if (qlikContentLibraries != null)
                {
                    qlikContentLibraries = qlikContentLibraries.OrderBy(s => s.Name).ToList();

                    QlikContentLibraries.Clear();
                    //QlikStreamApps.AddRange(streams);
                    QlikContentLibraries.AddRange(qlikContentLibraries);

                    QlikContentLibraries.ToList()
                        .Sort((contentLibrary1, contentLibrary2) =>
                            string.CompareOrdinal(contentLibrary2.Name, contentLibrary2.Name));
                }

                _log.Information("{0} content libraries found", QlikContentLibraries.Count);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                _log.Error(ex, "Error in the QlikContent library return");
                //throw;
            }
        }

        public QlikContentLibrary SelectedQlikContentLibrary
        {
            get => _selectedQlikContentLibrary;
            set
            {
                SetProperty(ref _selectedQlikContentLibrary, value);
                NextEnabled = value != null;
                _qlikThumbModel.ContentLibrary = value;
            }
        }

        public bool NextEnabled
        {
            get => _nextEnabled;
            set => SetProperty(ref _nextEnabled, value);
        }

        public void HandlePrevClick()
        {
            _eventAggregator.GetEvent<NavEvent>().Publish(NavEvent.ContentLibraryPrev);
        }

        public void HandleNextClick()
        {
            _eventAggregator.GetEvent<NavEvent>().Publish(NavEvent.ContentLibraryNext);
        }
    }
}