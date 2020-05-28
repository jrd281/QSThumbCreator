using System.Windows;
using Prism.Ioc;
using Prism.Unity;
using QSThumbCreator.Models.Thumb;
using QSThumbCreator.Processor;
using QSThumbCreator.Services.QlikEngine;
using QSThumbCreator.Utility;
using QSThumbCreator.Views.Main;
using Serilog;
using Serilog.Events;
using Unity;

namespace QSThumbCreator
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : PrismApplication
    {
        protected override Window CreateShell()
        {
            string loggingPath = CommonApplicationDataPath.GetFolderWithinCommonApplicationData("Logging", true);

            var template =
                "{Level:u4} {Timestamp:yyyy-MM-dd HH:mm:ss.ffffff} {SourceContext} {Message} {Exception}{NewLine}";

            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .WriteTo.File(loggingPath, outputTemplate: template,
                    restrictedToMinimumLevel: LogEventLevel.Verbose,
                    rollingInterval: RollingInterval.Day,
                    buffered: false)
                .CreateLogger();

            Log.Information("App.xaml - Creating the shell");
            return Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton(typeof(QlikAuthModel));
            containerRegistry.RegisterSingleton(typeof(QlikThumbModel));
            containerRegistry.RegisterSingleton(typeof(QlikEngineService));

            // https://stackoverflow.com/questions/61131731/dotnetcore-prism-7-wpf-service-not-being-created
            containerRegistry.RegisterInstance(containerRegistry.GetContainer().Resolve<QlikThumbProcessor>());
        }
    }
}