using System.Windows;
using System.Windows.Controls;

namespace QSThumbCreator.Views.SOW
{
    /// <summary>
    /// Interaction logic for StatementOfWork.xaml
    /// </summary>
    public partial class StatementOfWork : UserControl
    {
        private readonly StatementOfWorkViewModel _statementOfWorkViewModel;

        public StatementOfWork(StatementOfWorkViewModel statementOfWorkViewModel)
        {
            _statementOfWorkViewModel = statementOfWorkViewModel;
            InitializeComponent();
            this.DataContext = statementOfWorkViewModel;
        }

        private void HandlePrevClick(object sender, RoutedEventArgs e)
        {
            _statementOfWorkViewModel.HandlePrevClick();
        }

        private void HandleGoClick(object sender, RoutedEventArgs e)
        {
            _statementOfWorkViewModel.HandleGoClick();
        }

        private void StatementOfWork_OnLoaded(object sender, RoutedEventArgs e)
        {
            _statementOfWorkViewModel.RefreshText();
        }
    }
}