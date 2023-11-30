using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using D2MTranslator.Models;
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
    public class FileSystemViewModel : ObservableObject
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


        private List<string>? _visibleProperties;

        public ICommand OpenModFolderCommand { get; private set; }
        public ICommand OpenRefFolderCommand { get; private set; }

        public ICommand OpenSelectModFile { get; private set; }
        public ICommand OpenSelectRefFile { get; private set; }



        private void ExecuteOpenFile(RoutedPropertyChangedEventArgs<object> param, Enums.FolderType mod)
        {
            if (param.NewValue is FileSystemItem selectedItem)
            {
                var fullPath = selectedItem.ParentPath + "\\" + selectedItem.Name;
                Debug.WriteLine(fullPath);
                if (File.Exists(fullPath))
                {
                    var fileContent = File.ReadAllText(fullPath);
                    if (mod == Enums.FolderType.Mod)
                    {
                        WeakReferenceMessenger.Default.Send(new FileContentMessage(fileContent, Enums.FolderType.Mod));
                        //LoadJsonAsLines(fileContent, ModLines);
                    }
                    else if (mod == Enums.FolderType.Reference)
                    {
                        WeakReferenceMessenger.Default.Send(new FileContentMessage(fileContent, Enums.FolderType.Reference));
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

        private void ExecuteOpenFolder(object parameter, Enums.FolderType type)
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

                if (type == Enums.FolderType.Mod)
                    PopulateTreeViewWithJsonFiles(dialog.FileName, OriginalItems);
                else if (type == Enums.FolderType.Reference)
                    PopulateTreeViewWithJsonFiles(dialog.FileName, ReferenceItems);
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

        public FileSystemViewModel()
        {
            OpenModFolderCommand = new RelayCommand(param => ExecuteOpenFolder(param, Enums.FolderType.Mod));
            OpenRefFolderCommand = new RelayCommand(param => ExecuteOpenFolder(param, Enums.FolderType.Reference));
            OpenSelectModFile = new RelayCommand(param => ExecuteOpenFile((RoutedPropertyChangedEventArgs<object>)param, Enums.FolderType.Mod));
            OpenSelectRefFile = new RelayCommand(param => ExecuteOpenFile((RoutedPropertyChangedEventArgs<object>)param, Enums.FolderType.Reference));
        }

    }

}
