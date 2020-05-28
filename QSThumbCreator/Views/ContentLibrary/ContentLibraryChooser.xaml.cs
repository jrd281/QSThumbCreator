using System.Windows;
using System.Windows.Controls;

namespace QSThumbCreator.Views.ContentLibrary
{
    /// <summary>
    /// Interaction logic for ContentLibraryChooser.xaml
    /// </summary>
    public partial class ContentLibraryChooser : UserControl
    {
        private readonly ContentLibraryChooserViewModel _contentLibraryChooserViewModel;
        private bool _hasBeenActivated;

        public ContentLibraryChooser(ContentLibraryChooserViewModel contentLibraryChooserViewModel)
        {
            InitializeComponent();

            _contentLibraryChooserViewModel = contentLibraryChooserViewModel;
            this.DataContext = _contentLibraryChooserViewModel;
        }

        private void HandlePrevClick(object sender, RoutedEventArgs e)
        {
            _contentLibraryChooserViewModel.HandlePrevClick();
        }

        private void HandleNextClick(object sender, RoutedEventArgs e)
        {
            _contentLibraryChooserViewModel.HandleNextClick();
        }

        private void ContentLibraryChooser_OnLoaded(object sender, RoutedEventArgs e)
        {
            if (_hasBeenActivated == false)
            {
                _hasBeenActivated = true;
                _contentLibraryChooserViewModel.HandleReloadContentLibraries();
            }
        }
    }
}