using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using Prism.Events;
using Prism.Mvvm;
using QSThumbCreator.Events;
using QSThumbCreator.Models.Thumb;
using QSThumbCreator.Services.QlikEngine;
using Serilog;

namespace QSThumbCreator.Views.StreamsAndApps
{
    public class StreamsAndAppsViewModel : BindableBase
    {
        // https://github.com/serilog/serilog/issues/1294
        private static ILogger _log => Log.ForContext<StreamsAndAppsViewModel>();
        private readonly IEventAggregator _eventAggregator;
        private readonly QlikEngineService _qlikEngineService;
        private readonly QlikThumbModel _qlikThumbModel;
        private bool _nextEnabled;
        private bool _showProgressRing;
        public ObservableCollection<QlikStreamApp> QlikStreamApps;

        public StreamsAndAppsViewModel(QlikEngineService qlikEngineService,
            QlikThumbModel qlikThumbModel,
            IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _qlikEngineService = qlikEngineService;
            _qlikThumbModel = qlikThumbModel;

            QlikStreamApps = new ObservableCollection<QlikStreamApp>();

            FilteredQlikStreamApps = (ListCollectionView) CollectionViewSource.GetDefaultView(QlikStreamApps);
            FilteredQlikStreamApps.GroupDescriptions.Add(new PropertyGroupDescription("StreamName"));
            FilteredQlikStreamApps.SortDescriptions.Add(new SortDescription("StreamName", ListSortDirection.Ascending));
            FilteredQlikStreamApps.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));
        }

        public bool NextEnabled
        {
            get => _nextEnabled;
            set => SetProperty(ref _nextEnabled, value);
        }

        public ListCollectionView FilteredQlikStreamApps { get; set; }

        public bool ShowProgressRing
        {
            get => _showProgressRing;
            set => SetProperty(ref _showProgressRing, value);
        }

        public void HandlePrevClick()
        {
            _eventAggregator.GetEvent<NavEvent>().Publish(NavEvent.StreamsAppsSelPrev);
        }

        public void HandleNextClick()
        {
            _eventAggregator.GetEvent<NavEvent>().Publish(NavEvent.StreamsAppsSelNext);
        }

        public async void HandleReloadStreamsAndApps()
        {
            ShowProgressRing = true;
            try
            {
                var retrieveAppsAndStreamsAsyncTask = _qlikEngineService.RetrieveAppsAsync();
                await retrieveAppsAndStreamsAsyncTask;
                var qlikStreams = retrieveAppsAndStreamsAsyncTask.Result;
                if (qlikStreams != null)
                {
                    var apps = qlikStreams.SelectMany(s => s.QlikApps)
                        .ToList().Select(s => new QlikStreamApp(s)).ToList();

                    QlikStreamApps.Clear();
                    QlikStreamApps.AddRange(apps);

                    QlikStreamApps.ToList().Sort((streamApp1, streamApp2) =>
                    {
                        if (streamApp1.StreamName != streamApp2.StreamName)
                        {
                            return string.CompareOrdinal(streamApp1.StreamName, streamApp2.StreamName);
                        }

                        return streamApp1.Order.CompareTo(streamApp2.Order);
                    });

                    FilteredQlikStreamApps.Refresh();
                }

                _log.Information("{0} apps returned", QlikStreamApps.Count);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                _log.Error(ex, "Error in the ReloadStreamsAndApps call");
                throw;
            }
            finally
            {
                ShowProgressRing = false;
            }
        }

        public void HandleAppClicked()
        {
            NextEnabled = QlikStreamApps.ToList().Any(s => s.IsSelected);
            _qlikThumbModel.SelectedQlikApps = QlikStreamApps.ToList()
                .Where(s => s.IsSelected)
                .Select(s => s.QlikApp)
                .ToList();
        }
    }
}