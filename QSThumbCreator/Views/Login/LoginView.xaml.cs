using System.Windows;
using System.Windows.Controls;

namespace QSThumbCreator.Views.Login
{
    /// <summary>
    /// Interaction logic for LoginView.xaml
    /// </summary>
    public partial class LoginView : UserControl
    {
        private readonly LoginViewModel _loginViewModel;

        public LoginView(LoginViewModel loginViewModel)
        {
            InitializeComponent();

            _loginViewModel = loginViewModel;
            this.DataContext = loginViewModel;
        }

        private void HandleNextClick(object sender, RoutedEventArgs e)
        {
            _loginViewModel.HandleNextClick();
        }

        private void PasswordBox_OnPasswordChanged(object sender, RoutedEventArgs e)
        {
            _loginViewModel.PasswordBox_OnPasswordChanged(sender, e);
        }

        private void HandleTestConnection(object sender, RoutedEventArgs e)
        {
            _loginViewModel.HandleTestAuthentication(true);
        }
    }
}