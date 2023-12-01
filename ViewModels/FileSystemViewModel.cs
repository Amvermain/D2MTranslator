using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using D2MTranslator.Messages;
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
using static D2MTranslator.Enums;

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
                    if (mod == Enums.FolderType.Mod)
                    {
                        WeakReferenceMessenger.Default.Send(new FileContentMessage(fileContent, FolderType.Mod));
                        //LoadJsonAsLines(fileContent, ModLines);
                    }
                    else if (mod == Enums.FolderType.Reference)
                    {
                        WeakReferenceMessenger.Default.Send(new FileContentMessage(fileContent, FolderType.Reference));
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
            
            OpenSelectModFile = new RelayCommand<RoutedPropertyChangedEventArgs<object>>(param => ExecuteOpenFile(param, FolderType.Mod));
            OpenSelectRefFile = new RelayCommand<RoutedPropertyChangedEventArgs<object>>(param => ExecuteOpenFile(param, FolderType.Reference));
            SaveModFileCommand = new RelayCommand<string>(SaveModFile);

            WeakReferenceMessenger.Default.Register<FileOperationMessage>(this, (r, m) =>
            {
                Debug.WriteLine("message called");
                if (m.FolderType == FolderType.Mod)
                {
                    PopulateTreeViewWithJsonFiles(m.FilePath, OriginalItems);
                }
                else if (m.FolderType == FolderType.Reference)
                {
                    PopulateTreeViewWithJsonFiles(m.FilePath, ReferenceItems);
                }
            });
        }

        public ICommand SaveModFileCommand { get; private set; }

        private void SaveModFile(object commandParameter)
        {

        }
    }

}
