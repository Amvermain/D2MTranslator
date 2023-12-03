using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using D2MTranslator.Messages;
using D2MTranslator.Services;
using Microsoft.WindowsAPICodePack.Dialogs;
using Ninject;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using static D2MTranslator.Enums;

namespace D2MTranslator.ViewModels
{
    public class MainViewModel: ObservableObject
    {
        private LanguageConfigWindow languageConfigWindow;
        private ConfigurationService configurationService;

        public bool isConfigWindowClosed = true;
        public bool IsModified { get; set; }
        public MainViewModel()
        {
            IsModified = false;
            WeakReferenceMessenger.Default.Register<IsModifiedMessage>(this, (r, m) =>
            {
                IsModified = m.IsModified;
            });
            WeakReferenceMessenger.Default.Register<LanguageConfigClosingMessage>(this, (r, m) =>
            {
                isConfigWindowClosed = true;
            });
            CurrentViewModel = App.Kernel.Get<InteractiveViewModel>();
            configurationService = App.Kernel.Get<ConfigurationService>();
            OpenModFolderCommand = new RelayCommand(() => ExecuteOpenFolder(FolderType.Mod));
            OpenRefFolderCommand = new RelayCommand(() => ExecuteOpenFolder(FolderType.Reference));
            ChangeViewCommand = new RelayCommand<string>((type) => ChangeView(type));
            SaveCommand = new RelayCommand(() => ExecuteSave());
            ConfigLanguage = new RelayCommand(() => OpenLanguageConfigWindow());
            AutomergeCommand = new RelayCommand(() => ExecuteAutomerge());
        }

        private void ExecuteAutomerge()
        {
            WeakReferenceMessenger.Default.Send(new AutomergeMessage());
        }

        private void ExecuteSave()
        {
            WeakReferenceMessenger.Default.Send(new FileOperationMessage(FileOperation.Save));
        }

        private void OpenLanguageConfigWindow()
        {
            
            //check is window closed. if closed, create new one.
            if (isConfigWindowClosed)
            {
                languageConfigWindow = new LanguageConfigWindow();
                isConfigWindowClosed = false;
            }
            
            languageConfigWindow.Show();
            //WeakReferenceMessenger.Default.Send(new OpenLanguageConfigWindowMessage());
        }

        public ICommand OpenModFolderCommand { get; private set; }
        public ICommand OpenRefFolderCommand { get; private set; }
        public ICommand ChangeViewCommand { get; private set; }
        public ICommand ConfigLanguage { get; private set; }
        public ICommand SaveCommand { get; private set; }
        public ICommand AutomergeCommand { get; private set; }

        public void OnClose(object sender, CancelEventArgs e)
        {
            configurationService.Save();
            if (languageConfigWindow != null)
            {
                languageConfigWindow.Close();
            }
        }

        private object currentViewModel;
        public object CurrentViewModel { get => currentViewModel; set => SetProperty(ref currentViewModel, value); }

        private void ExecuteOpenFolder(FolderType type)
        {
            string folderType = type == FolderType.Mod ? "D2MTMod" : "D2MTReference";
            var folderPath = type == FolderType.Mod ? configurationService.LastOpenedModDir : configurationService.LastOpenedRefDir;
            CommonOpenFileDialog dialog = new CommonOpenFileDialog(folderType)
            {
                IsFolderPicker = true,
                Multiselect = false,
                InitialDirectory = folderPath
            };
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                if (dialog.FileName == null)
                {
                    return;
                }

                if (type == FolderType.Mod)
                {
                    configurationService.LastOpenedModDir = dialog.FileName;
                } else if (type == FolderType.Reference)
                {
                    configurationService.LastOpenedRefDir = dialog.FileName;
                }
                Debug.WriteLine(dialog.FileName + " opened");
                WeakReferenceMessenger.Default.Send(new FileOperationMessage(FileOperation.Open, dialog.FileName, type));
                
            }
        }

        private void ChangeView(string type)
        {
            if (type == "Editor")
            {
                CurrentViewModel = App.Kernel.Get<JsonFileViewModel>();
                
            } else if (type == "Interactive")
            {
                CurrentViewModel = App.Kernel.Get<InteractiveViewModel>();
            }
        }
    }
}
