using System.Security;
using System.Windows;
using System.Windows.Controls;
using Prism.Events;
using Prism.Mvvm;
using QSThumbCreator.Events;
using QSThumbCreator.Models.Thumb;
using QSThumbCreator.Services.QlikEngine;

namespace QSThumbCreator.Views.Login
{
    public class LoginViewModel : BindableBase
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly QlikAuthModel _qlikThumbAuthModel;
        private readonly QlikEngineService _qlikEngineService;
        private string _qlikServerUrl;
        private string _qlikAdDomain;
        private string _qlikAdUsername;
        private SecureString _qlikAdPassword;
        private bool _nextEnabled;

        public LoginViewModel(IEventAggregator eventAggregator, QlikAuthModel qlikThumbAuthModel, QlikEngineService qlikEngineService)
        {
            _eventAggregator = eventAggregator;
            _qlikThumbAuthModel = qlikThumbAuthModel;
            _qlikEngineService = qlikEngineService;
        }


        public void HandleNextClick()
        {
            var handleTestAuthentication = HandleTestAuthentication(false);

            if (handleTestAuthentication)
            {
                _eventAggregator.GetEvent<NavEvent>().Publish(NavEvent.LoginNext);
            }
        }

        public string QlikServerUrl
        {
            get => _qlikServerUrl;
            set
            {
                SetProperty(ref _qlikServerUrl, value);
                ValidateInputs();
                UpdateModel();
            }
        }

        public string QlikAdDomain
        {
            get => _qlikAdDomain;
            set
            {
                SetProperty(ref _qlikAdDomain, value);
                ValidateInputs();
                UpdateModel();
            }
        }

        public string QlikAdUsername
        {
            get => _qlikAdUsername;
            set
            {
                SetProperty(ref _qlikAdUsername, value);
                ValidateInputs();
                UpdateModel();
            }
        }

        public SecureString QlikAdPassword
        {
            get => _qlikAdPassword;
            set
            {
                SetProperty(ref _qlikAdPassword, value);
                ValidateInputs();
                UpdateModel();
            }
        }

        public bool NextEnabled
        {
            get => _nextEnabled;
            set => SetProperty(ref _nextEnabled, value);
        }

        private bool ValidateInputs()
        {
            var testValue = !(string.IsNullOrWhiteSpace(_qlikServerUrl)
                              || string.IsNullOrWhiteSpace(_qlikAdDomain)
                              || string.IsNullOrWhiteSpace(_qlikAdUsername)
                              || _qlikAdPassword == null || _qlikAdPassword.Length == 0);
            if (!_nextEnabled.Equals(testValue))
            {
                NextEnabled = testValue;
            }

            return testValue;
        }

        public void PasswordBox_OnPasswordChanged(object sender, RoutedEventArgs e)
        {
            var passwordBox = sender as PasswordBox;
            _qlikAdPassword = passwordBox?.SecurePassword;
            ValidateInputs();
            UpdateModel();
        }

        public bool TestAuthentication()
        {
            var testAuth = _qlikEngineService.TestAuth();
            return testAuth;
        }

        public bool HandleTestAuthentication(bool showSuccess)
        {
            var testAuth = TestAuthentication();
            if (testAuth)
            {
                if (showSuccess)
                {
                    MessageBox.Show("Authentication Successful!", "Success", MessageBoxButton.OK);
                }
            }
            else
            {
                MessageBox.Show("Authentication Failed!", "Fail", MessageBoxButton.OK);
            }

            return testAuth;
        }

        private void UpdateModel()
        {
            _qlikThumbAuthModel.QlikServerUrl = _qlikServerUrl;
            _qlikThumbAuthModel.QlikAdDomain = _qlikAdDomain;
            _qlikThumbAuthModel.QlikAdUsername = _qlikAdUsername;
            _qlikThumbAuthModel.QlikAdPassword = _qlikAdPassword;
        }
    }
}