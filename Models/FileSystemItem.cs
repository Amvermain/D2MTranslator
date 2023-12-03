using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using D2MTranslator.Messages;
using D2MTranslator.Services;
using Ninject;
using System.Collections.Generic;
using System.Diagnostics;

namespace D2MTranslator.Models
{
    public class FileSystemItem: ObservableObject
    {
        public string Name { get; set; }
        public List<FileSystemItem> Items { get; set; } = new List<FileSystemItem>();
        public bool IsExpanded { get; set; }
        public string ParentPath { get; internal set; }
        public Enums.FolderType FolderType { get; set; }

        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (SetProperty(ref _isSelected, value))
                {
                    Debug.WriteLine("Selected Changed");
                    OnSelectedChanged();
                }
            }
        }

        private void OnSelectedChanged()
        {
            // 파일 선택 변경시 실행될 로직
            // 예: FileSystemViewModel에 알림
            if (IsSelected == true)
                WeakReferenceMessenger.Default.Send(new FileItemSelectedMessage(this));
        }

        public FileSystemItem(string name, string parentPath, Enums.FolderType folderType)
        {
            IsExpanded = App.Kernel.Get<ConfigurationService>().IsExpandByDefault;
            Name = name;
            ParentPath = parentPath;
            FolderType = folderType;
        }
    }

}
