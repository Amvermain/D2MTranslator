using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using D2MTranslator.Messages;
using Microsoft.WindowsAPICodePack.Dialogs;
using Ninject;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using static D2MTranslator.Enums;

namespace D2MTranslator.ViewModels
{
    public class MainViewModel: ObservableObject
    {
        public bool IsModified { get; set; }
        public MainViewModel()
        {
            IsModified = false;
            WeakReferenceMessenger.Default.Register<IsModifiedMessage>(this, (r, m) =>
            {
                IsModified = m.IsModified;
            });
            CurrentViewModel = App.Kernel.Get<JsonFileViewModel>();

            OpenModFolderCommand = new RelayCommand(() => ExecuteOpenFolder(FolderType.Mod));
            OpenRefFolderCommand = new RelayCommand(() => ExecuteOpenFolder(FolderType.Reference));
        }


        public ICommand OpenModFolderCommand { get; private set; }
        public ICommand OpenRefFolderCommand { get; private set; }

        private object currentViewModel;
        public object CurrentViewModel { get => currentViewModel; set => SetProperty(ref currentViewModel, value); }

        private void ExecuteOpenFolder(FolderType type)
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

                Debug.WriteLine("message sent!");
                WeakReferenceMessenger.Default.Send(new FileOperationMessage(FileOperation.Open, dialog.FileName, type));
                
            }
        }
    }
}
