using System.Linq;
using Prism.Mvvm;
using QSThumbCreator.Models.Qlik;

namespace QSThumbCreator.Views.StreamsAndApps
{
    public class QlikStreamApp : BindableBase
    {
        public QlikApp QlikApp { get; set; }
        public string Name { get; set; }
        public string StreamName { get; set; }
        public int Order { get; set; }
        private string _itemType;

        private bool _isSelected;

        public QlikStreamApp(QlikApp qlikApp)
        {
            QlikApp = qlikApp;

            Name = qlikApp.Name;
            StreamName = qlikApp.Stream?.Name;
            Order = 0;
        }

        public bool IsSelected
        {
            get => _isSelected;
            set => SetProperty(ref _isSelected, value);
        }

        public bool HasRestForbiddenChars
        {
            get
            {
                return new[]
                {
                    "#", "$", "&", "+", ",", "/", ":", ";",
                    "=", "?", "@", "[", "]", "!", "'", "(", ")",
                    "*"
                }.Any(s => Name.Contains(s));
            }
        }
    }
}