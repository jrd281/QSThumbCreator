using System;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace QSThumbCreator.Views.StreamsAndApps
{
    /// <summary>
    /// Interaction logic for StreamsAndAppsSelection.xaml
    /// </summary>
    public partial class StreamsAndAppsSelection : UserControl
    {
        private bool _hasBeenActivated;
        private readonly StreamsAndAppsViewModel _streamsAndAppsViewModel;

        public StreamsAndAppsSelection(StreamsAndAppsViewModel streamsAndAppsViewModel)
        {
            InitializeComponent();
            this.DataContext = streamsAndAppsViewModel;
            _streamsAndAppsViewModel = streamsAndAppsViewModel;
        }

        private void StreamsAndAppsSelection_OnLoaded(object sender, RoutedEventArgs e)
        {
            if (_hasBeenActivated == false)
            {
                _hasBeenActivated = true;
                _streamsAndAppsViewModel.HandleReloadStreamsAndApps();
            }
        }

        private void HandleNextClick(object sender, RoutedEventArgs e)
        {
            _streamsAndAppsViewModel.HandleNextClick();
        }

        private void HandlePrevClick(object sender, RoutedEventArgs e)
        {
            _streamsAndAppsViewModel.HandlePrevClick();
        }


        private void HandleAppClicked(object sender, RoutedEventArgs e)
        {
            _streamsAndAppsViewModel.HandleAppClicked();
        }
    }

    public class IconToImageSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var icon = value as Icon;
            if (icon == null)
            {
                Trace.TraceWarning("Attempted to convert {0} instead of Icon object in IconToImageSourceConverter",
                    value);
                return null;
            }

            ImageSource imageSource = Imaging.CreateBitmapSourceFromHIcon(
                icon.Handle,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());
            return imageSource;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class StreamDgTemplateSelector : DataTemplateSelector
    {
        public DataTemplate CheckboxTemplate { get; set; }
        public DataTemplate EmptyTemplate { get; set; }
        public DataTemplate OldTemplate { get; set; }

        public override DataTemplate SelectTemplate
            (object item, DependencyObject container)
        {
            if (container is ContentPresenter presenter)
            {
                DataGridCell cell = presenter.Parent as DataGridCell;

                if (cell?.DataContext is QlikStreamApp qlikStreamApp)
                {
                    if (qlikStreamApp.HasRestForbiddenChars)
                    {
                        return EmptyTemplate;
                    }

                    return CheckboxTemplate;
                }
            }

            return EmptyTemplate;
        }
    }
}