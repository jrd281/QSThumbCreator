using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Qlik.Sense.RestClient;
using QSThumbCreator.Models.Qlik;
using QSThumbCreator.Models.Thumb;
using Serilog;

namespace QSThumbCreator.Services.QlikEngine
{
    public class QlikEngineService
    {
        private readonly ILogger _log = Log.ForContext<QlikEngineService>();
        private readonly QlikAuthModel _qlikThumbAuthModel;
        private RestClient _restClient;

        private string _sheetRetrievalString =
            "/qrs/app/object/full?filter=(objectType eq 'sheet' and app.name eq '{0}')";

        private string _contentDirUploadString = "/qrs/contentlibrary/{0}/uploadfile?externalpath={1}&overwrite=false";

        public QlikEngineService(QlikAuthModel qlikThumbAuthModel)
        {
            _qlikThumbAuthModel = qlikThumbAuthModel;
            _log.Information("QlikEngineService created");
        }

        private RestClient GetRestClient()
        {
            if (_restClient == null)
            {
                _restClient = new RestClient(_qlikThumbAuthModel.QlikServerUrl);
                _restClient.AsNtlmUserViaProxy(
                    new NetworkCredential(_qlikThumbAuthModel.QlikAdUsername,
                        _qlikThumbAuthModel.QlikAdPassword), false);
            }

            return _restClient;
        }

        /*
         * Not using this because it returns "true" even with passwords I know to be incorrect
         */
        public bool TestAuth()
        {
            /*
             * Can't use the _restClient in case of bad values being used during the test
             */
            var testRestClient = new RestClient(_qlikThumbAuthModel.QlikServerUrl);
            testRestClient.AsNtlmUserViaProxy(
                new NetworkCredential(_qlikThumbAuthModel.QlikAdUsername,
                    _qlikThumbAuthModel.QlikAdPassword), false);
            var returnValue = true;
            try
            {
                testRestClient.Get("/qrs/about");
            }
            catch (Exception e)
            {
                returnValue = false;

                _log.Information("Authentication failure");
                Console.WriteLine(e);
            }

            return returnValue;
        }

        public List<QlikSheet> RetrieveAppSheets(string id, string appName)
        {
            _log.Information("Starting the sheet retrieval for App: {0}, Id: {1}", appName, id);

            var interpolatedString = string.Format(_sheetRetrievalString, appName);

            var client = GetRestClient();

            string getResponse;
            try
            {
                getResponse = client.Get(interpolatedString);
            }
            catch (Exception ex)
            {
                _log.Error(ex, "Exception when trying to visit app named {0}", appName);
                if (ex.InnerException != null)
                {
                    _log.Error(ex, "Inner Exception when trying to visit app named {0}", appName);
                }

                throw;
            }


            var sheets = JsonConvert.DeserializeObject<List<QlikSheet>>(getResponse).ToList();

            //Filter the sheets because the REST request filters by name, and I can't figure out how 
            //to make the url params filter by app id
            var qlikSheets = sheets.Where(s => s.App.Id.Equals(id)).ToList();

            return qlikSheets;
        }

        public Task<List<QlikStream>> RetrieveAppsAsync()
        {
            _log.Information("Starting the RetrieveStreamsAsync request");

            var client = GetRestClient();
            return client.GetAsync("/qrs/app").ContinueWith(task =>
            {
                try
                {
                    var appsAndStreamsString = task.Result;
                    var appsAndStreams = JsonConvert.DeserializeObject<List<QlikApp>>(appsAndStreamsString);

                    _log.Information("{0} apps returned", appsAndStreams?.Count ?? (object) "0");

                    var streamsDictionary =
                        (appsAndStreams ?? new List<QlikApp>()).Where(app => app.Stream != null)
                        .Select(app => app.Stream)
                        .GroupBy(p => p.Id)
                        .ToDictionary(g => g.Key, g => g.Last());

                    var appStreamGroupingDict = (appsAndStreams ?? new List<QlikApp>()).Where(app => app.Stream != null)
                        .GroupBy(u => u.Stream.Id)
                        .ToDictionary(gdc => gdc.Key, gdc => gdc.ToList());

                    appStreamGroupingDict.Select(s => s).ToList().ForEach(s =>
                    {
                        streamsDictionary.TryGetValue(s.Key, out var foundQlikStream);
                        if (foundQlikStream != null)
                        {
                            foundQlikStream.QlikApps = s.Value;
                        }
                    });

                    _log.Information("{streamreturned} streams returned", streamsDictionary?.Count);

                    return streamsDictionary.Values.ToList();
                }
                catch (AggregateException ex)
                {
                    _log.Error(ex, "AggregateException error calling the RetrieveStreamsAsync service");

                    ex.Handle(inner =>
                    {
                        if (inner is HttpRequestException)
                        {
                            _log.Error(inner, "HttpRequestException error calling the RetrieveStreamsAsync service");

                            return true;
                        }

                        return false;
                    });
                }
                catch (Exception ex)
                {
                    _log.Error(ex, " Exception error calling the RetrieveStreamsAsync service");

                    throw;
                }

                return null;
            });
        }

        public Task<List<QlikContentLibrary>> RetrieveContentLibrariesAsync()
        {
            _log.Information("Starting the RetrieveContentLibrariesAsync request");

            var client = GetRestClient();
            return client.GetAsync("/qrs/contentlibrary").ContinueWith(task =>
            {
                try
                {
                    var contentLibraryString = task.Result;

                    var contentLibraries =
                        JsonConvert.DeserializeObject<List<QlikContentLibrary>>(contentLibraryString);

                    _log.Information("{contentLibrariesReturned} content libraries returned",
                        contentLibraries?.Count ?? (object) "0");

                    return contentLibraries;
                }
                catch (AggregateException ex)
                {
                    _log.Error(ex, "AggregateException error calling the RetrieveContentLibrariesAsync service");

                    ex.Handle(inner =>
                    {
                        if (inner is HttpRequestException)
                        {
                            _log.Error(inner,
                                "HttpRequestException error calling the RetrieveContentLibrariesAsync service");

                            return true;
                        }

                        return false;
                    });
                }
                catch (Exception ex)
                {
                    _log.Error(ex, " Exception error calling the RetrieveContentLibrariesAsync service");

                    throw;
                }

                return null;
            });
        }

        public Task<string> UploadFileToContentDirectory(string contentDirectoryName, string filePath)
        {
            _log.Information("Attempting to post file at {filePath} to content directory name:{contentDirectoryName}",
                filePath, contentDirectoryName);

            var imageByteArray = ImageConversion(filePath);

            var fileName = Path.GetFileName(filePath);
            var url = string.Format(_contentDirUploadString, contentDirectoryName, fileName);
            var client = GetRestClient();
            Task<string> returnTask;
            try
            {
                returnTask = client.WithContentType("image/jpeg").PostAsync(url, imageByteArray);
            }
            catch (Exception ex)
            {
                _log.Error(ex, "Error posting to the content directory of {0}", contentDirectoryName);
                Console.WriteLine(ex);
                throw;
            }

            return returnTask;
        }

        //https://stackoverflow.com/questions/3801275/how-to-convert-image-to-byte-array
        private byte[] ImageConversion(string imageName)
        {
            //Initialize a file stream to read the image file
            FileStream fs = new FileStream(imageName, FileMode.Open, FileAccess.Read);

            //Initialize a byte array with size of stream
            byte[] imgByteArr = new byte[fs.Length];

            //Read data from the file stream and put into the byte array
            fs.Read(imgByteArr, 0, Convert.ToInt32(fs.Length));

            //Close a file stream
            fs.Close();

            return imgByteArr;
        }
    }
}