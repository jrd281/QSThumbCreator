using System.Windows.Controls;

namespace QSThumbCreator.Views.Completion
{
    /// <summary>
    /// Interaction logic for Completion.xaml
    /// </summary>
    public partial class Completion : UserControl
    {
        public Completion(CompletionViewModel completionViewModel)
        {
            InitializeComponent();
            this.DataContext = completionViewModel;
        }
    }
}