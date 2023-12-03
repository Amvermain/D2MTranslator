using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using D2MTranslator.Messages;
using D2MTranslator.Services;
using Ninject;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Text.Json.Serialization;
using System.Windows.Input;

namespace D2MTranslator.Models
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:명명 스타일", Justification = "<보류 중>")]
    [Serializable]
    public class TranslationItem : ObservableObject
    {
        [JsonIgnore]
        public bool IsValid
        {
            get
            {
                if (_configurationService.isHidingSameTranslation)
                {
                    return !IsVisibleElementsAreSame();
                }
                return true;
            }
        }

        [JsonIgnore]
        public Dictionary<string, bool> LanguageVisibility
        {
            get => _configurationService.LanguageVisibility;
        }

        private bool IsVisibleElementsAreSame()
        {
            if (referenceItem == null) return false;
            bool result = true;
            foreach (var property in typeof(TranslationItem).GetProperties())
            {
                
                if (property.Name == "id" || property.Name == "Key" || property.Name == "enUS" || property.Name == "referenceItem" || property.Name == "IsValid" || property.Name == "IsExpanded")
                    continue;
                if (_configurationService.LanguageVisibility.ContainsKey(property.Name))
                {
                    if (_configurationService.LanguageVisibility[property.Name] == false) {
                        continue; 
                    }
                }
                var value = property.GetValue(this);
                var referenceValue = property.GetValue(referenceItem);
                if (value != null && referenceValue != null)
                {
                    if (value.ToString() != referenceValue.ToString())
                    {
                        result = false;
                        break;
                    }
                }
            }

            return result;
        }
        [JsonIgnore]
        private readonly ReferenceJsonDataService _referenceJsonDataService;
        [JsonIgnore]
        private readonly ConfigurationService _configurationService;

        [JsonIgnore]
        private bool _isExpanded;
        [JsonIgnore]
        public bool IsExpanded
        {
            get => _isExpanded;
            set
            {
                _isExpanded = value;
                Debug.WriteLine("IsExpanded = " + value);
                if (_isExpanded)
                {
                    referenceItem = _referenceJsonDataService.GetTranslationItem(id);
                    Debug.WriteLine("referenceItem = " + referenceItem);
                }
                SetProperty(ref _isExpanded, value);
            }
        }

        public int id { get; set; }
        public string Key { get; set; }
        public string enUS { get; set; }
        private string _deDE;
        public string? deDE
        {
            get => _deDE; set
            {
                if (SetProperty(ref _deDE, EscapeNewLine(value)))
                {
                    SendIsModifiedMessage();
                }
            }
        }

        

        private string _esES;
        public string? esES
        {
            get => _esES; set
            {
                if (SetProperty(ref _esES, EscapeNewLine(value)))
                {
                    SendIsModifiedMessage();
                }
            }
        }
        private string _esMX;
        public string? esMX
        {
            get => _esMX; set
            {
                if (SetProperty(ref _esMX, EscapeNewLine(value)))
                {
                    SendIsModifiedMessage();
                }
            }
        }
        private string _frFR;
        public string? frFR { get => _frFR; set {
                if (SetProperty(ref _frFR, EscapeNewLine(value)))
                {
                    SendIsModifiedMessage();
                }
            } }
        private string _itIT;
        public string? itIT { get => _itIT; set {
                if (SetProperty(ref _itIT, EscapeNewLine(value)))
                {
                    SendIsModifiedMessage();
                }
            } }
        private string _jaJP;
        public string? jaJP { get => _jaJP; set {
                if (SetProperty(ref _jaJP, EscapeNewLine(value)))
                {
                    SendIsModifiedMessage();
                }
            } }
        private string _koKR;
        public string? koKR { get => _koKR; set {
                if (SetProperty(ref _koKR, EscapeNewLine(value)))
                {
                    SendIsModifiedMessage();
                }
            } }
        private string _plPL;
        public string? plPL { get => _plPL; set {
                if (SetProperty(ref _plPL, EscapeNewLine(value)))
                {
                    SendIsModifiedMessage();
                }
            } }
        private string _ptBR;
        public string? ptBR { get => _ptBR; set {
                if (SetProperty(ref _ptBR, EscapeNewLine(value)))
                {
                    SendIsModifiedMessage();
                }
            } }
        private string _ruRU;
        public string? ruRU { get => _ruRU; set {
                if (SetProperty(ref _ruRU, EscapeNewLine(value)))
                {
                    SendIsModifiedMessage();
                }
            } }
        private string _zhCN;
        public string? zhCN { get => _zhCN; set {
                if (SetProperty(ref _zhCN, EscapeNewLine(value)))
                {
                    SendIsModifiedMessage();
                }
            } }
        private string _zhTW;
        public string? zhTW { get => _zhTW; set {
                if (SetProperty(ref _zhTW, EscapeNewLine(value)))
                {
                    SendIsModifiedMessage();
                }
            } }

        private void SendIsModifiedMessage()
        {
            WeakReferenceMessenger.Default.Send(new IsModifiedMessage(true));
        }

        [JsonIgnore]
        private TranslationItem _referenceItem;
        [JsonIgnore]
        public TranslationItem referenceItem { get => _referenceItem; set => SetProperty(ref _referenceItem, value); }

        private string EscapeNewLine(string text)
        {
            return text.Replace("\n", "\\n");
        }

        public void OverwriteTranslation(string param)
        {
            PropertyInfo propertyInfo = typeof(TranslationItem).GetProperty(param);
            if (propertyInfo == null || referenceItem == null)
            {
                Debug.WriteLine("propertyInfo is null");
                return;
            }
            var valueToCopy = propertyInfo.GetValue(referenceItem);
            propertyInfo.SetValue(this, valueToCopy);
        }

        [JsonIgnore]
        public ICommand OverwriteCommand { get; private set; }

        public TranslationItem(int id, string key, string enUS)
        {
            if (!DesignerProperties.GetIsInDesignMode(new System.Windows.DependencyObject()))
            {
                _referenceJsonDataService = App.Kernel.Get<ReferenceJsonDataService>();
                _configurationService = App.Kernel.Get<ConfigurationService>();
                referenceItem = _referenceJsonDataService.GetTranslationItem(id);

                WeakReferenceMessenger.Default.Register<ReferencedTranslationResponseMessage>(this, (r, m) =>
                {
                    referenceItem = m.Item;
                });
                WeakReferenceMessenger.Default.Send(new ReferencedTranslationRequestMessage(this.id));
                WeakReferenceMessenger.Default.Register<LanguageConfigChangedMessage>(this, (r, m) =>
                {
                    if (m.PropertyName == "SkipSame")
                    {
                        OnPropertyChanged(nameof(IsValid));
                    } else if (_configurationService.LanguageVisibility.ContainsKey(m.PropertyName))
                    {
                        OnPropertyChanged(nameof(LanguageVisibility));
                    }

                });

                OverwriteCommand = new RelayCommand<string>(OverwriteTranslation);
            }
            
            this.id = id;
            Key = key;
            this.enUS = enUS;
        }

        public TranslationItem() : this(0, "key", "asdf") {
            
        }
    }
}
