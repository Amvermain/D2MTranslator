using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using D2MTranslator.Messages;
using D2MTranslator.Models;
using D2MTranslator.Services;
using D2MTranslator.ViewModels.Models;
using Ninject;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Windows;
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
            }
        }

        public FileSystemItem SelectedRefItem
        {
            get => _selectedRefItem;
            set
            {
                _selectedRefItem = value;
                OnPropertyChanged(nameof(SelectedRefItem));
            }
        }

        private void PopulateTreeViewWithJsonFiles(string folderPath, ObservableCollection<FileSystemItem> target, FolderType folderType)
        {
            target.Clear();
            DirectoryInfo dirInfo = new DirectoryInfo(folderPath);
            var rootItem = CreateFileSystemItem(dirInfo, folderType);
            target.Add(rootItem);
        }

        private FileSystemItem CreateFileSystemItem(DirectoryInfo directoryInfo, FolderType folderType)
        {
            var item = new FileSystemItem(directoryInfo.Name, directoryInfo.FullName, folderType);

            foreach (var dir in directoryInfo.GetDirectories())
            {
                if (!dir.Name.StartsWith("."))
                    item.Items.Add(CreateFileSystemItem(dir, folderType));
            }

            foreach (var file in directoryInfo.GetFiles("*.json"))
            {
                item.Items.Add(new FileSystemItem(file.Name, directoryInfo.FullName, folderType));
            }

            return item;
        }

        public FileSystemViewModel()
        {
            _referenceJsonDataService = App.Kernel.Get<ReferenceJsonDataService>();
            _configurationService = App.Kernel.Get<ConfigurationService>();
            InitiateRelayCommands();
            RegisterMessages();
        }

        private void InitiateRelayCommands()
        {

        }

        private readonly ReferenceJsonDataService _referenceJsonDataService;
        private readonly ConfigurationService _configurationService;

        private void RegisterMessages()
        {
            WeakReferenceMessenger.Default.Register<FileItemSelectedMessage>(this, OnFileItemSelected);
            WeakReferenceMessenger.Default.Register<FileOperationMessage>(this, (r, m) =>
            {
                if (m.FileOperation == FileOperation.Open)
                {
                    var folderType = m.FolderType;
                    var folderPath = m.FilePath;
                    if (folderType == FolderType.Mod)
                    {
                        PopulateTreeViewWithJsonFiles(folderPath, OriginalItems, FolderType.Mod);
                    }
                    else if (folderType == FolderType.Reference)
                    {
                        PopulateTreeViewWithJsonFiles(folderPath, ReferenceItems, FolderType.Reference);
                    }
                }
                else if (m.FileOperation == FileOperation.Save)
                {
                    WeakReferenceMessenger.Default.Send(new FileSaveMessage(_selectedModItem));
                }

            });
            WeakReferenceMessenger.Default.Register<IsModifiedMessage>(this, (r, m) =>
            {
                isChanged = m.IsModified;
            });
            WeakReferenceMessenger.Default.Register<FileOpenFinishMessage>(this, (r, m) =>
            {
                Debug.WriteLine("FileOpenFinishMessage / " + preservedPreviousItem.Name + " / " + PreviousSelectedItem?.Name);
                PreviousSelectedItem = preservedPreviousItem;
            });
            WeakReferenceMessenger.Default.Register<SavedFileContentMessage>(this, (r, m) =>
            {
                Debug.WriteLine("SavedFileContentMessage Received");
                File.WriteAllText(m.selectedModItem.ParentPath + "\\" + m.selectedModItem.Name, m.json);
                WeakReferenceMessenger.Default.Send(new IsModifiedMessage(false));
            });
            WeakReferenceMessenger.Default.Register<AutomergeMessage>(this, (r, m) =>
            {
                if (OriginalItems != null && ReferenceItems != null)
                {
                    var list = new List<FileSystemItem>();
                    list.AddRange(OriginalItems);
                    RecursiveAutomerge(list);
                }
            });
        }

        private void RecursiveAutomerge(List<FileSystemItem> Items)
        {
            foreach (var item in Items)
            {
                Debug.WriteLine("testing : " + item);
                if (item.Items.Count > 0)
                {
                    RecursiveAutomerge(item.Items);
                }
                else
                {
                    Debug.WriteLine("testing : " + item.Name);
                    var fullPath = item.ParentPath + "\\" + item.Name;
                    if (File.Exists(fullPath))
                    {
                        if (_referenceJsonDataService.filePaths.ContainsKey(item.Name))
                        {
                            var refFilePath = _referenceJsonDataService.filePaths[item.Name];
                            var fileContent = File.ReadAllText(fullPath);
                            var refFileContent = File.ReadAllText(refFilePath);

                            Debug.WriteLine($"modfile {fullPath} opened and reffile {refFilePath} Opened");
                            var jsonOptions = new JsonSerializerOptions()
                            {
                                AllowTrailingCommas = true
                            };
                            var modItems = JsonSerializer.Deserialize<List<TranslationItem>>(fileContent, jsonOptions);
                            var refItems = JsonSerializer.Deserialize<List<TranslationItem>>(refFileContent, jsonOptions);
                            if (modItems != null && refItems != null)
                            {
                                foreach (var modItem in modItems)
                                {
                                    var refItem = refItems.Find(x => x.id == modItem.id);
                                    if (refItem != null)
                                    {
                                        if (modItem.enUS == refItem.enUS)
                                        {
                                            //Debug.WriteLine("enUS matched");
                                            //for each property in modItem
                                            foreach (var property in typeof(TranslationItem).GetProperties())
                                            {
                                                if (property.Name == "id" || property.Name == "Key" || property.Name == "enUS" || property.Name == "referenceItem" || property.Name == "IsValid" || property.Name == "IsExpanded")
                                                    continue;

                                                if (_configurationService.LanguageVisibility.ContainsKey(property.Name) && _configurationService.LanguageVisibility[property.Name])
                                                {
                                                    //Debug.WriteLine($"property {property.Name} from {modItem.enUS}");
                                                    var modValue = property.GetValue(modItem);
                                                    var refValue = property.GetValue(refItem);
                                                    if ((modValue != null || modValue.ToString() != "") && !modValue.ToString().Equals(refValue.ToString()))
                                                    {
                                                        Debug.WriteLine("property : " + property.Name + " is going to merge. " + modValue + " turns into " + refValue);
                                                        Debug.WriteLine(!modValue.ToString().Equals(refValue.ToString()));
                                                        property.SetValue(modItem, refValue);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }

                                var json = JsonSerializer.Serialize(value: modItems, options: new JsonSerializerOptions()
                                {
                                    WriteIndented = true,
                                    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                                });
                                json = json.Replace("\\\\n", "\\n");
                                File.WriteAllText(fullPath, json);
                            }
                        }
                    }
                }
            }
        }

        FileSystemItem preservedPreviousItem;
        public FileSystemItem PreviousSelectedItem
        {
            get => _previousSelectedItem;
            set
            {
                _previousSelectedItem = value;
            }
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
                if (MessageBox.Show("Are you sure you want to discard your changes?", "Discard Changes", MessageBoxButton.YesNo) == MessageBoxResult.No)
                {
                    message.Item.IsSelected = false;
                    if (PreviousSelectedItem != null)
                        PreviousSelectedItem.IsSelected = true;
                    return;
                }
            }

            if (message.Item.FolderType == FolderType.Mod)
                _selectedModItem = message.Item;
            else if (message.Item.FolderType == FolderType.Reference)
                _selectedRefItem = message.Item;

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
                else if (mod == FolderType.Reference)
                {
                    WeakReferenceMessenger.Default.Send(new FileContentMessage(fileContent, FolderType.Reference));
                }
            }
        }

        private bool isChanged = false;
    }

}
