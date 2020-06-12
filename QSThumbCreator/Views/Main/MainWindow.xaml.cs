using System;
using System.Windows;
using Prism.Ioc;
using Prism.Regions;
using QSThumbCreator.Views.ContentLibrary;
using QSThumbCreator.Views.Login;
using QSThumbCreator.Views.Options;
using QSThumbCreator.Views.SOW;
using QSThumbCreator.Views.StreamsAndApps;

namespace QSThumbCreator.Views.Main
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly IContainerExtension _container;
        private readonly IRegionManager _regionManager;
        private readonly MainWindowViewModel _mainWindowViewModel;

        public MainWindow(IContainerExtension container, IRegionManager regionManager,
            MainWindowViewModel mainWindowViewModel)
        {
            _container = container;
            _regionManager = regionManager;
            _mainWindowViewModel = mainWindowViewModel;
            this.DataContext = _mainWindowViewModel;

            InitializeComponent();
        }

        private void OnMainWindowLoaded(object sender, EventArgs e)
        {
            IRegion mainRegion = _regionManager.Regions["MainRegion"];

            var loginView = _container.Resolve<LoginView>();
            mainRegion.Add(loginView, "LoginView");

            var streamsAndAppsView = _container.Resolve<StreamsAndAppsSelection>();
            mainRegion.Add(streamsAndAppsView, "StreamsAndAppsSelection");

            var contentLibraryChooser = _container.Resolve<ContentLibraryChooser>();
            mainRegion.Add(contentLibraryChooser, "ContentLibraryChooser");

            var optionsView = _container.Resolve<OptionsView>();
            mainRegion.Add(optionsView, "OptionsView");

            var statementOfWork = _container.Resolve<StatementOfWork>();
            mainRegion.Add(statementOfWork, "StatementOfWork");

            var completion = _container.Resolve<Completion.Completion>();
            mainRegion.Add(completion, "Completion");
        }

        private void HandleGoToGithubClick(object sender, RoutedEventArgs e)
        {
            _mainWindowViewModel.HandleGoToGithub();
        }

        private void HandleOpenLogFolderClick(object sender, RoutedEventArgs e)
        {
            _mainWindowViewModel.HandleShowLogs();
        }

        private void HandleExitClick(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}