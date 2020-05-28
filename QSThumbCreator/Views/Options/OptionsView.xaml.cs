using System.Windows;
using System.Windows.Controls;

namespace QSThumbCreator.Views.Options
{
    /// <summary>
    /// Interaction logic for OptionsView.xaml
    /// </summary>
    public partial class OptionsView : UserControl
    {
        private readonly OptionsViewModel _optionsViewModel;

        public OptionsView(OptionsViewModel optionsViewModel)
        {
            InitializeComponent();
            _optionsViewModel = optionsViewModel;
            this.DataContext = optionsViewModel;
        }

        private void HandlePrevClick(object sender, RoutedEventArgs e)
        {
            _optionsViewModel.HandlePrevClick();
        }

        private void HandleNextClick(object sender, RoutedEventArgs e)
        {
            _optionsViewModel.HandleNextClick();
        }

        private void HandleBrowseForDirectoryClick(object sender, RoutedEventArgs e)
        {
            _optionsViewModel.HandleSelectThumbOutputDirectory();
        }
    }
}