using System;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.WindowsAPICodePack.Dialogs;
using Prism.Events;
using Prism.Mvvm;
using QSThumbCreator.Events;
using QSThumbCreator.Models.Thumb;

namespace QSThumbCreator.Views.Options
{
    public class OptionsViewModel : BindableBase
    {
        private readonly QlikThumbModel _qlikThumbModel;
        private readonly IEventAggregator _eventAggregator;
        private ObservableCollection<OptionComboboxItem> _taskOptionsCollection;
        private OptionComboboxItem _selectedTaskOption;
        private string _thumbOutputDirectory;
        private bool _nextEnabled;

        public OptionsViewModel(QlikThumbModel qlikThumbModel, IEventAggregator eventAggregator)
        {
            _qlikThumbModel = qlikThumbModel;
            _eventAggregator = eventAggregator;

            TaskOptionsCollection = new ObservableCollection<OptionComboboxItem>
            {
                new OptionComboboxItem
                {
                    DisplayText = "Only create the thumbnails and save to a folder",
                    ItemValue = QlikThumbModel.TaskLocalSaveOnly
                },
                new OptionComboboxItem
                {
                    DisplayText = "Create the thumbnails and save to a Content Directory",
                    ItemValue = QlikThumbModel.TaskContentDirectorySave
                }
            };

            SelectedTaskOption =
                TaskOptionsCollection.First(s => s.ItemValue.Equals(QlikThumbModel.TaskLocalSaveOnly));
        }

        public void HandlePrevClick()
        {
            _eventAggregator.GetEvent<NavEvent>().Publish(NavEvent.OptionsPrev);
        }

        public void HandleNextClick()
        {
            _eventAggregator.GetEvent<NavEvent>().Publish(NavEvent.OptionsNext);
        }

        public ObservableCollection<OptionComboboxItem> TaskOptionsCollection
        {
            get => _taskOptionsCollection;
            set => SetProperty(ref _taskOptionsCollection, value);
        }

        public OptionComboboxItem SelectedTaskOption
        {
            get => _selectedTaskOption;
            set
            {
                SetProperty(ref _selectedTaskOption, value);
                _qlikThumbModel.TaskType = _selectedTaskOption.ItemValue;
                Validate();
            }
        }

        public string ThumbOutputDirectory
        {
            get => _thumbOutputDirectory;
            set
            {
                SetProperty(ref _thumbOutputDirectory, value);
                _qlikThumbModel.ThumbOutputDirectory = value;
                Validate();
            }
        }

        public bool NextEnabled
        {
            get => _nextEnabled;
            set => SetProperty(ref _nextEnabled, value);
        }

        public void HandleSelectThumbOutputDirectory()
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog();
            if (string.IsNullOrWhiteSpace(ThumbOutputDirectory) == false)
            {
                dialog.InitialDirectory = ThumbOutputDirectory;
            }
            else
            {
                dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            }

            dialog.IsFolderPicker = true;
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                ThumbOutputDirectory = dialog.FileName;
                Validate();
            }
        }

        private bool Validate()
        {
            var nextShouldBeEnabled = false;
            if (_selectedTaskOption != null)
            {
                if (_selectedTaskOption.ItemValue.Equals(QlikThumbModel.TaskLocalSaveOnly))
                {
                    if (string.IsNullOrWhiteSpace(ThumbOutputDirectory) == false)
                    {
                        nextShouldBeEnabled = true;
                    }
                }
                else if (_selectedTaskOption.ItemValue.Equals(QlikThumbModel.TaskContentDirectorySave))
                {
                    nextShouldBeEnabled = true;
                }
            }

            if (NextEnabled != nextShouldBeEnabled)
            {
                NextEnabled = nextShouldBeEnabled;
            }

            return nextShouldBeEnabled;
        }
    }

    public class OptionComboboxItem
    {
        public String DisplayText { get; set; }
        public String ItemValue { get; set; }
    }
}