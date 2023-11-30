using D2MTranslator.ViewModels.Models;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Input;

namespace D2MTranslator.ViewModels
{
    public class MainViewModel : ObservableObject
    {
        public ObservableCollection<FileSystemItem>? OriginalItems { get; private set; } = new ObservableCollection<FileSystemItem>();
        public ObservableCollection<FileSystemItem>? ReferenceItems { get; private set; } = new ObservableCollection<FileSystemItem>();

        private FileSystemItem? _selectedModItem;
        private FileSystemItem? _selectedRefItem;

        public FileSystemItem SelectedModItem
        {
            get => _selectedModItem;
            set
            {
                _selectedModItem = value;
                OnPropertyChanged(nameof(SelectedModItem));
                // 선택된 아이템 변경에 따른 추가 로직
            }
        }

        public FileSystemItem SelectedRefItem
        {
            get => _selectedRefItem;
            set
            {
                _selectedRefItem = value;
                OnPropertyChanged(nameof(SelectedRefItem));
                // 선택된 아이템 변경에 따른 추가 로직
            }
        }

        private List<TranslationItem>? _serializedModContent;
        private List<TranslationItem>? _serializedReferenceContent;
        public List<TranslationItem>? SerializedModContent
        {
            get => _serializedModContent;
            set
            {
                _serializedModContent = value;
                OnPropertyChanged(nameof(SerializedModContent));
            }
        }
        public List<TranslationItem>? SerializedReferenceContent
        {
            get => _serializedReferenceContent;
            set
            {
                _serializedReferenceContent = value;
                OnPropertyChanged(nameof(SerializedReferenceContent));
            }
        }


        private string? _modContentText;
        private string? _refContentText;
        public string ModContentText
        {
            get => _modContentText;
            set
            {
                _modContentText = value;
                OnPropertyChanged(nameof(ModContentText));
            }
        }
        public string RefContentText
        {
            get => _refContentText;
            set
            {
                _refContentText = value;
                OnPropertyChanged(nameof(RefContentText));
            }
        }

        private List<string>? _visibleProperties;

        public ICommand OpenModFolderCommand { get; private set; }
        public ICommand OpenRefFolderCommand { get; private set; }

        public ICommand OpenSelectModFile { get; private set; }
        public ICommand OpenSelectRefFile { get; private set; }



        private void ExecuteOpenFile(RoutedPropertyChangedEventArgs<object> param, FolderType mod)
        {
            if (param.NewValue is FileSystemItem selectedItem)
            {
                var fullPath = selectedItem.ParentPath + "\\" + selectedItem.Name;
                Debug.WriteLine(fullPath);
                if (File.Exists(fullPath))
                {
                    var fileContent = File.ReadAllText(fullPath);
                    if (mod == FolderType.Mod)
                    {
                        ModText = fileContent;
                        //LoadJsonAsLines(fileContent, ModLines);
                    }
                    else if (mod == FolderType.Ref)
                    {
                        RefText = fileContent;
                        //LoadJsonAsLines(fileContent, RefLines);
                    }
                }
            }
        }

        private static string ConvertToJsonString(List<TranslationItem> items)
        {
            var options = new JsonSerializerOptions
            {
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                WriteIndented = true
            };
            return JsonSerializer.Serialize(items, options);

        }

        private enum FolderType
        {
            Mod,
            Ref
        }

        private void ExecuteOpenFolder(object parameter, FolderType type)
        {
            CommonOpenFileDialog dialog = new()
            {
                IsFolderPicker = true,
                Multiselect = false
            };
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                if (dialog.FileName == null)
                {
                    return;
                }

                if (type == FolderType.Mod)
                    PopulateTreeViewWithJsonFiles(dialog.FileName, OriginalItems);
                else if (type == FolderType.Ref)
                    PopulateTreeViewWithJsonFiles(dialog.FileName, ReferenceItems);
            }
        }


        public List<string>? VisibleProperties
        {
            get => _visibleProperties;
            set
            {
                _visibleProperties = value;
                OnPropertyChanged(nameof(VisibleProperties));
            }
        }

        private void PopulateTreeViewWithJsonFiles(string folderPath, ObservableCollection<FileSystemItem> target)
        {
            DirectoryInfo dirInfo = new DirectoryInfo(folderPath);
            var rootItem = CreateFileSystemItem(dirInfo);
            target.Add(rootItem);
        }

        private FileSystemItem CreateFileSystemItem(DirectoryInfo directoryInfo)
        {
            var item = new FileSystemItem(directoryInfo.Name, directoryInfo.FullName);

            foreach (var dir in directoryInfo.GetDirectories())
            {
                item.Items.Add(CreateFileSystemItem(dir));
            }

            foreach (var file in directoryInfo.GetFiles("*.json"))
            {
                item.Items.Add(new FileSystemItem(file.Name, directoryInfo.FullName));
            }

            return item;
        }

        public ObservableCollection<LineItem> ModLines { get; private set; } = new ObservableCollection<LineItem>();
        public ObservableCollection<LineItem> RefLines { get; private set; } = new ObservableCollection<LineItem>();

        private string _refText;
        public string RefText
        {
            get => _refText;
            set
            {
                _refText = value;
                OnPropertyChanged(nameof(RefText));
            }
        }
        private string _modText;
        public string ModText
        {
            get => _modText;
            set
            {
                _modText = value;
                OnPropertyChanged(nameof(ModText));
            }
        }

        private void LoadJsonAsLines(string jsonContent, ObservableCollection<LineItem> lines)
        {
            lines.Clear();
            var splittedLines = jsonContent.Split('\n');
            foreach (var line in splittedLines)
            {
                lines.Add(new LineItem { Text = line });
            }
            Debug.Write(lines);
        }

        public MainViewModel()
        {
            OpenModFolderCommand = new RelayCommand(param => ExecuteOpenFolder(param, FolderType.Mod));
            OpenRefFolderCommand = new RelayCommand(param => ExecuteOpenFolder(param, FolderType.Ref));
            OpenSelectModFile = new RelayCommand(param => ExecuteOpenFile((RoutedPropertyChangedEventArgs<object>)param, FolderType.Mod));
            OpenSelectRefFile = new RelayCommand(param => ExecuteOpenFile((RoutedPropertyChangedEventArgs<object>)param, FolderType.Ref));
        }

    }

}
