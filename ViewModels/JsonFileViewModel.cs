﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using D2MTranslator.Messages;
using D2MTranslator.Models;
using D2MTranslator.ViewModels.Models;
using System.Collections.Generic;

namespace D2MTranslator.ViewModels
{
    public class JsonFileViewModel : ObservableObject
    {


        private bool _isModified;
        public bool IsModified
        {
            get => _isModified;
            set
            {
                _isModified = value;
                WeakReferenceMessenger.Default.Send(new IsModifiedMessage(value));
                OnPropertyChanged(nameof(IsModified));
            }
        }

        public JsonFileViewModel()
        {
            WeakReferenceMessenger.Default.Register<FileContentMessage>(this, (r, m) =>
            {
            if (m.FolderType == Enums.FolderType.Mod)
            {
                ModContentText = m.Content;
                ModContentOriginalText = m.Content;
                IsModified = false;
            }
            else if (m.FolderType == Enums.FolderType.Reference)
            {
                RefContentText = m.Content;
            }
            WeakReferenceMessenger.Default.Send(new FileOpenFinishMessage());
                
            });
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
                IsModified = ModContentOriginalText != ModContentText;
                OnPropertyChanged(nameof(ModContentText));
            }
        }
        public string ModContentOriginalText { get; set; } = "";
        public string RefContentText
        {
            get => _refContentText;
            set
            {
                _refContentText = value;
                OnPropertyChanged(nameof(RefContentText));
            }
        }
    }
}
