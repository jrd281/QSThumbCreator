using NUnit.Framework;
using System;
using System.Linq;
using System.Security;
using System.Threading.Tasks;
using QSThumbCreator.Models.Thumb;
using QSThumbCreator.Services.QlikEngine;

namespace QSThumbCreator.Services.Tests
{
    [TestFixture()]
    public class QlikEngineServiceTests
    {
        private QlikEngineService _qlikEngineService;
        private QlikAuthModel _qlikThumbAuthModel;

        [SetUp]
        public void Setup()
        {
            // Have to use environment variables since we don't have Secrets Manager
            // without Core.  The reason we don't have Core is because AutoIt doesn't
            // seem to want to play nicely with it.
            // J.Dallas 05-06-2020
            string testUrl = Environment.GetEnvironmentVariable("QSTHUMBTESTURL");
            string testDomain = Environment.GetEnvironmentVariable("QSTHUMBTESTDOMAIN");
            string testUsername = Environment.GetEnvironmentVariable("QSTHUMBTESTUSERNAME");
            string testPassword = Environment.GetEnvironmentVariable("QSTHUMBTESTPASSWORD");
            SecureString secureString = new SecureString();
            testPassword.ToList().ForEach(s =>
            {
                secureString.AppendChar(s);
            });
            _qlikThumbAuthModel = new QlikAuthModel
            {
                QlikServerUrl = testUrl,
                QlikAdDomain = testDomain,
                QlikAdUsername = testUsername,
                QlikAdPassword = secureString
            };

            _qlikEngineService = new QlikEngineService(_qlikThumbAuthModel);
        }

        [Test()]
        public async Task QlikEngineAppStreamTest()
        {
            try
            {
                var task = _qlikEngineService.RetrieveAppsAsync();
                await task;

                var taskResult = task.Result;
                
                Assert.NotNull(taskResult);
            }
            catch (Exception e)
            {
                Assert.Fail("Should not have thrown an error");
            }
        }

        [Test()]
        public async Task QlikEngineAppStreamTestIncorrectCredentials()
        {
            _qlikThumbAuthModel.QlikAdUsername = "MickeyMouse";
            AsyncTestDelegate asyncTestDelegate = () => _qlikEngineService.RetrieveAppsAsync();
            Assert.ThrowsAsync<AggregateException>(asyncTestDelegate);
        }
    }
}