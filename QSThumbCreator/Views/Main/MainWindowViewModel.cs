using System.Diagnostics;
using System.Reflection;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using QSThumbCreator.Events;
using QSThumbCreator.Models.Thumb;
using QSThumbCreator.Processor.Events;
using QSThumbCreator.Utility;
using Serilog;

namespace QSThumbCreator.Views.Main
{
    public class MainWindowViewModel : BindableBase
    {
        // https://github.com/serilog/serilog/issues/1294
        private static ILogger _log => Log.ForContext<MainWindowViewModel>();
        private readonly QlikThumbModel _qlikThumbModel;
        private readonly IRegionManager _regionManager;
        private string _title;

        public MainWindowViewModel(QlikThumbModel qlikThumbModel,
            IEventAggregator eventAggregator,
            IRegionManager regionManager)
        {
            Title = "QS ThumbCreator - " + Assembly.GetExecutingAssembly().GetName().Version.ToString();
            _qlikThumbModel = qlikThumbModel;
            _regionManager = regionManager;
            eventAggregator.GetEvent<NavEvent>().Subscribe(HandleNavEvent);
            eventAggregator.GetEvent<EndProcessingEvent>().Subscribe(HandleEndProcessing);
        }

        private void HandleEndProcessing(EndProcessingPayload endProcessingPayload)
        {
            _log.Information("Navigating to the Completion view");

            var mainRegion = _regionManager.Regions["MainRegion"];
            mainRegion.RequestNavigate("Completion");
        }

        public void HandleGoToGithub()
        {
            // https://stackoverflow.com/questions/502199/how-to-open-a-web-page-from-my-application
            var psi = new ProcessStartInfo
            {
                FileName = "https://github.com/jrd281/QSThumbCreator",
                UseShellExecute = true
            };
            Process.Start(psi);
        }

        public void HandleShowLogs()
        {
            string loggingPath = CommonApplicationDataPath.GetFolderWithinCommonApplicationData("Logging", true);

            var explorerWindowProcess = new Process
            {
                StartInfo =
                {
                    FileName = "explorer.exe",
                    Arguments = "/select,\"" + loggingPath + "\""
                }
            };

            explorerWindowProcess.Start();
        }

        public string Title
        {
            get => _title;
            set
            {
                SetProperty(ref _title, value);
            }
        }

        private void HandleNavEvent(string navEventType)
        {
            if (string.IsNullOrEmpty(navEventType) == false)
            {
                var mainRegion = _regionManager.Regions["MainRegion"];

                if (navEventType.Equals(NavEvent.LoginNext))
                {
                    _log.Information("Navigating to the StreamsAndAppsSelection view");
                    mainRegion.RequestNavigate("StreamsAndAppsSelection");
                }
                else if (navEventType.Equals(NavEvent.StreamsAppsSelNext))
                {
                    _log.Information("Navigating to the Options view");
                    mainRegion.RequestNavigate("OptionsView");
                }
                else if (navEventType.Equals(NavEvent.OptionsNext))
                {
                    if (_qlikThumbModel.TaskType.Equals(QlikThumbModel.TaskContentDirectorySave))
                    {
                        _log.Information("Navigating to the ContentLibrary view because we have a content library: {0}",
                            _qlikThumbModel.ContentLibrary?.Name);
                        mainRegion.RequestNavigate("ContentLibraryChooser");
                    }
                    else
                    {
                        _log.Information("Navigating to the SOW view");
                        mainRegion.RequestNavigate("StatementOfWork");
                    }
                }
                else if (navEventType.Equals(NavEvent.ContentLibraryNext))
                {
                    _log.Information("Navigating to the SOW view");
                    mainRegion.RequestNavigate("StatementOfWork");
                }

                else if (navEventType.Equals(NavEvent.StreamsAppsSelPrev))
                {
                    _log.Information("Navigating back to the LoginView view");
                    mainRegion.RequestNavigate("LoginView");
                }
                else if (navEventType.Equals(NavEvent.OptionsPrev))
                {
                    _log.Information("Navigating back to the StreamsAndAppsSelection view");
                    mainRegion.RequestNavigate("StreamsAndAppsSelection");
                }
                else if (navEventType.Equals(NavEvent.ContentLibraryPrev))
                {
                    _log.Information("Navigating back to the Options view");
                    mainRegion.RequestNavigate("OptionsView");
                }
                else if (navEventType.Equals(NavEvent.SowPrev))
                {
                    if (_qlikThumbModel.TaskType.Equals(QlikThumbModel.TaskContentDirectorySave))
                    {
                        _log.Information(
                            "Navigating back to the ContentLibrary view because we have a content library: {0}",
                            _qlikThumbModel.ContentLibrary?.Name);
                        mainRegion.RequestNavigate("ContentLibraryChooser");
                    }
                    else
                    {
                        _log.Information("Navigating to the Options view");
                        mainRegion.RequestNavigate("OptionsView");
                    }
                }
            }
        }
    }
}