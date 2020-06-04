using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using AutoIt;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using Prism.Events;
using QSThumbCreator.Models.Qlik;
using QSThumbCreator.Models.Thumb;
using QSThumbCreator.Processor.Events;
using QSThumbCreator.Services.QlikEngine;
using QSThumbCreator.Utility;
using Serilog;
using Size = System.Drawing.Size;

namespace QSThumbCreator.Processor
{
    public class QlikThumbProcessor
    {
        private static ILogger _log => Log.ForContext<QlikThumbProcessor>();

        private readonly IEventAggregator _eventAggregator;
        private readonly QlikAuthModel _qlikAuthModel;
        private readonly QlikThumbModel _qlikThumbModel;
        private readonly QlikEngineService _qlikEngineService;
        private readonly HashSet<string> _counterHashSet;
        private string _thumbOutputPath;
        private IWebDriver _webDriver;


        public QlikThumbProcessor(IEventAggregator eventAggregator,
            QlikAuthModel qlikAuthModel,
            QlikThumbModel qlikThumbModel,
            QlikEngineService qlikEngineService)
        {
            _eventAggregator = eventAggregator;
            _qlikAuthModel = qlikAuthModel;
            _qlikThumbModel = qlikThumbModel;
            _qlikEngineService = qlikEngineService;
            _counterHashSet = new HashSet<string>();

            _eventAggregator.GetEvent<StartProcessingEvent>().Subscribe(StartProcessing);

            _log.Information("QlikThumbProcessor created");
        }

        private void StartProcessing(string obj)
        {
            var startTime = DateTime.Now;
            _log.Information("StartProcessing invoked");

            _thumbOutputPath = CreateOutputDirectory();
            _webDriver = StartChromeDriver();

            Login();
            CreateThumbs();
            _log.Information("Processing completed");
            EndProcessing(startTime);
        }

        private void EndProcessing(DateTime startDateTime)
        {
            List<string> counterList = _counterHashSet.ToList();

            var streamCount = counterList.Count(s => s.StartsWith("STREAM"));
            var appCount = counterList.Count(s => s.StartsWith("APP"));
            var sheetCount = counterList.Count(s => s.StartsWith("SHEET"));
            var elapsedTime = DateTime.Now.Subtract(startDateTime);

            _log.Information("{0} Streams read", streamCount);
            _log.Information("{0} Apps read", appCount);
            _log.Information("{0} Sheets read", sheetCount);
            _log.Information("{0} Time Elapsed", elapsedTime.ToString());

            var endProcessing = new EndProcessingPayload
            {
                StreamsProcessedCount = streamCount,
                AppsProcessedCount = appCount,
                SheetsProcessedCount = sheetCount,
                ElapsedTimeSpan = elapsedTime
            };

            _eventAggregator.GetEvent<EndProcessingEvent>().Publish(endProcessing);
        }

        private string CreateOutputDirectory()
        {
            string testPath;
            if (string.IsNullOrWhiteSpace(_qlikThumbModel.ThumbOutputDirectory))
            {
                testPath = Path.GetTempPath();
                testPath += Path.DirectorySeparatorChar + "QlikThumbProcessor/";
            }
            else
            {
                testPath = _qlikThumbModel.ThumbOutputDirectory + Path.DirectorySeparatorChar;
            }

            if (Directory.Exists(testPath) == false)
            {
                Directory.CreateDirectory(testPath);
            }

            return testPath;
        }

        private IWebDriver StartChromeDriver()
        {
            string path = Path.GetDirectoryName(
                Assembly.GetAssembly(typeof(QlikThumbProcessor))?.Location);
            var chromeDriverPath = path;
            ChromeOptions chromeOptions = new ChromeOptions();
            // "no-sandbox" is usually used for headless operation
            // the process may be changed to headless at a later date
            chromeOptions.AddArgument("no-sandbox");
            //https://github.com/GoogleChrome/chrome-launcher/blob/master/docs/chrome-flags-for-tools.md
            chromeOptions.AddArgument("enable-automation");
            chromeOptions.SetLoggingPreference(LogType.Browser, LogLevel.All);
            chromeOptions.SetLoggingPreference(LogType.Client, LogLevel.All);
            chromeOptions.SetLoggingPreference(LogType.Profiler, LogLevel.All);
            chromeOptions.SetLoggingPreference("performance", LogLevel.All);
            IWebDriver driver = new ChromeDriver(chromeDriverPath, chromeOptions);

            driver.Manage().Window.Size = new Size(1280, 800);
            _log.Information("Returning the chrome driver");

            return driver;
        }

        private void Login()
        {
            _log.Information("Attempting to login");

            var url = _qlikAuthModel.QlikServerUrl;
            Uri uri = new Uri(url);
            _webDriver.Navigate().GoToUrl(uri.AbsoluteUri);

            WebDriverWait wait = new WebDriverWait(_webDriver, TimeSpan.FromSeconds(20));
            wait.Until(s =>
                ((IJavaScriptExecutor) _webDriver).ExecuteScript("return document.readyState").Equals("complete"));

            var domain = _qlikAuthModel.QlikAdDomain;
            var username = _qlikAuthModel.QlikAdUsername;
            var autoItPassword = _qlikAuthModel.QlikAutoItPassword;

            AutoItX.WinActivate("", "Sign In");
            //https://www.autoitscript.com/forum/topic/186559-some-special-characters-not-allowed-within-string/
            var usernameString = domain + "\\" + username + "{TAB}" + autoItPassword + "{ENTER}";
            AutoItX.Send(usernameString);

            wait = new WebDriverWait(_webDriver, TimeSpan.FromSeconds(20));
            try
            {
                wait.Until(s =>
                    ((IJavaScriptExecutor) _webDriver).ExecuteScript("return document.readyState").Equals("complete"));
            }
            catch (Exception ex)
            {
                _log.Error(ex, "Exception while waiting for the login process to complete successfully");
                throw;
            }
        }

        private void CreateThumbs()
        {
            var selectedApps = _qlikThumbModel.SelectedQlikApps;

            Uri uri = new Uri(_qlikAuthModel.QlikServerUrl);

            selectedApps.ForEach(s => { CreateAppThumbs(s, uri); });

            _webDriver.Close();
        }

        private void CreateAppThumbs(QlikApp qlikApp, Uri uri)
        {
            var appName = qlikApp.Name;
            var appId = qlikApp.Id;
            var streamName = qlikApp.StreamName;

            _counterHashSet.Add("STREAM/" + streamName);
            _counterHashSet.Add("APP/" + appId);

            _log.Information("Creating thumbs for {0}", appName);

            var appDir = _thumbOutputPath + Path.DirectorySeparatorChar
                                          + FileNameUtility.EscapeFilenameWindows(streamName)
                                          + Path.DirectorySeparatorChar +
                                          FileNameUtility.EscapeFilenameWindows(appName);

            if (Directory.Exists(appDir) == false)
            {
                Directory.CreateDirectory(appDir);
            }

            var retrieveAppSheets = _qlikEngineService.RetrieveAppSheets(qlikApp.Id, qlikApp.Name);

            foreach (QlikSheet qlikSheet in retrieveAppSheets)
            {
                var sheetPath = qlikSheet.EngineObjectId;
                var sheetName = qlikSheet.Name;

                _counterHashSet.Add("SHEET/" + sheetPath);

                string url = uri.AbsoluteUri + "sense/app/" + appId + "/sheet/" + sheetPath + "/state/analysis";

                _webDriver.Navigate().GoToUrl(url);

                ((IJavaScriptExecutor) _webDriver).ExecuteScript(QlikThumbProcessorJs.WaitForSheetLoadJsChrome);
                var wait = new WebDriverWait(_webDriver, TimeSpan.FromSeconds(20));
                wait.Until(s =>
                    ((IJavaScriptExecutor) _webDriver).ExecuteScript("return window.oneSecondPastLastEvent();")
                    .Equals(true));

                var fileName = FileNameUtility.EscapeFilenameWindows(streamName + "_" + appName + "_" + sheetName);
                var filePath = appDir + Path.DirectorySeparatorChar + fileName + ".jpg";

                _log.Information("Writing out the thumb for Stream:{0}, App:{1}, Sheet:{2}", streamName, appName,
                    sheetName);

                ITakesScreenshot scrShot = ((ITakesScreenshot) _webDriver);
                scrShot.GetScreenshot().SaveAsFile(filePath);

                if (string.IsNullOrWhiteSpace(_qlikThumbModel.TaskType) == false &&
                    _qlikThumbModel.TaskType.Equals(QlikThumbModel.TaskContentDirectorySave))
                {
                    _log.Information("Uploading the thumb for Stream:{0}, App:{1}, Sheet:{2} Content Library {3}",
                        streamName, appName, sheetName, _qlikThumbModel.ContentLibrary.Name);

                    _qlikEngineService.UploadFileToContentDirectory(_qlikThumbModel.ContentLibrary.Name, filePath);
                }
            }
        }
    }
}