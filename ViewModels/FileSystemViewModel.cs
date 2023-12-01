using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using D2MTranslator.Messages;
using D2MTranslator.Models;
using D2MTranslator.ViewModels.Models;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
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



        //public ICommand OpenSelectModFile { get; private set; }
        //public ICommand OpenSelectRefFile { get; private set; }



        

        private static string ConvertToJsonString(List<TranslationItem> items)
        {
            var options = new JsonSerializerOptions
            {
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                WriteIndented = true
            };
            return JsonSerializer.Serialize(items, options);

        }

        

        private void PopulateTreeViewWithJsonFiles(string folderPath, ObservableCollection<FileSystemItem> target, FolderType folderType)
        {
            DirectoryInfo dirInfo = new DirectoryInfo(folderPath);
            var rootItem = CreateFileSystemItem(dirInfo, folderType);
            target.Add(rootItem);
        }

        private FileSystemItem CreateFileSystemItem(DirectoryInfo directoryInfo, FolderType folderType)
        {
            var item = new FileSystemItem(directoryInfo.Name, directoryInfo.FullName, folderType);

            foreach (var dir in directoryInfo.GetDirectories())
            {
                item.Items.Add(CreateFileSystemItem(dir, folderType));
            }

            foreach (var file in directoryInfo.GetFiles("*.json"))
            {
                item.Items.Add(new FileSystemItem(file.Name, directoryInfo.FullName, folderType));
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
            InitiateRelayCommands();
            RegisterMessages();
        }

        private void InitiateRelayCommands()
        {
            //OpenSelectModFile = new RelayCommand<RoutedPropertyChangedEventArgs<object>>(param => ExecuteOpenFile(param, FolderType.Mod));
            //OpenSelectRefFile = new RelayCommand<RoutedPropertyChangedEventArgs<object>>(param => ExecuteOpenFile(param, FolderType.Reference));
            SaveModFileCommand = new RelayCommand<string>(SaveModFile);
        }

        private void RegisterMessages()
        {
            WeakReferenceMessenger.Default.Register<FileItemSelectedMessage>(this, OnFileItemSelected);
            WeakReferenceMessenger.Default.Register<FileOperationMessage>(this, (r, m) =>
            {
                if (m.FolderType == FolderType.Mod)
                {
                    PopulateTreeViewWithJsonFiles(m.FilePath, OriginalItems, FolderType.Mod);
                }
                else if (m.FolderType == FolderType.Reference)
                {
                    PopulateTreeViewWithJsonFiles(m.FilePath, ReferenceItems ,FolderType.Reference);
                }
            });
            WeakReferenceMessenger.Default.Register<IsModifiedMessage>(this, (r, m) =>
            {
                Debug.WriteLine("IsModifiedMessage Changed To " + m.IsModified);
                isChanged = m.IsModified;
            });
            WeakReferenceMessenger.Default.Register<FileOpenFinishMessage>(this, (r, m) =>
            {
                Debug.WriteLine("FileOpenFinishMessage / " + preservedPreviousItem.Name + " / " + PreviousSelectedItem?.Name);
                PreviousSelectedItem = preservedPreviousItem;
            });
        }

        public ICommand SaveModFileCommand { get; private set; }
        FileSystemItem preservedPreviousItem;
        public FileSystemItem PreviousSelectedItem
        {
            get => _previousSelectedItem; 
            set
            {
                Debug.WriteLine("who touch it?");
                _previousSelectedItem = value;
            }
        }

        private void SaveModFile(object commandParameter)
        {

        }

        private FileSystemItem _previousSelectedItem;

        private void OnFileItemSelected(object recipient, FileItemSelectedMessage message)
        {
            Debug.WriteLine("OnFileItemSelected");
            if (PreviousSelectedItem == message.Item)
                return;

            if (isChanged)
            {
                // 사용자에게 변경사항 폐기 여부 확인
                if (MessageBox.Show("Are you sure you want to discard your changes?", "Change Discard", MessageBoxButton.YesNo) == MessageBoxResult.No)
                {
                    message.Item.IsSelected = false;
                    PreviousSelectedItem.IsSelected = true;
                    return;
                }
            }

            // 파일 열기 로직 또는 다른 처리...
            preservedPreviousItem = message.Item;
            ExecuteOpenFile(message);
            
        }

        private void ExecuteOpenFile(FileItemSelectedMessage message)
        {
            var selectedItem = message.Item;
            var fullPath = selectedItem.ParentPath + "\\" + selectedItem.Name;
            Debug.WriteLine(fullPath);
            if (File.Exists(fullPath))
            {
                var fileContent = File.ReadAllText(fullPath);
                var mod = selectedItem.FolderType;
                if (mod == FolderType.Mod)
                {
                    WeakReferenceMessenger.Default.Send(new FileContentMessage(fileContent, FolderType.Mod));
                }
                else if (mod == Enums.FolderType.Reference)
                {
                    WeakReferenceMessenger.Default.Send(new FileContentMessage(fileContent, FolderType.Reference));
                }
            }
        }

        private bool isChanged = false;
    }

}
