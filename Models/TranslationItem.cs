using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Printing;

namespace D2MTranslator.Models
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:명명 스타일", Justification = "<보류 중>")]
    [Serializable]
    public class TranslationItem : ObservableObject
    {
        private bool _isExpanded;
    public bool IsExpanded
    {
        get => _isExpanded;
        set => SetProperty(ref _isExpanded, value);
    }

        public int id { get; set; }
        public string Key { get; set; }
        private string _deDE;
        public string? deDE
        {
            get => _deDE;
            set => SetProperty(ref _deDE, EscapeNewLine(value));
        }
        public string enUS { get; set; }

        private string _esES;
        public string? esES
        {
            get => _esES;
            set => SetProperty(ref _esES, EscapeNewLine(value));
        }
        private string _esMX;
        public string? esMX { get => _esMX; set => SetProperty(ref _esMX, EscapeNewLine(value)); }
        private string _frFR;
        public string? frFR { get => _frFR; set => SetProperty(ref _frFR, EscapeNewLine(value)); }
        private string _itIT;
        public string? itIT { get => _itIT; set => SetProperty(ref _itIT, EscapeNewLine(value)); }
        private string _jaJP;
        public string? jaJP { get => _jaJP; set => SetProperty(ref _jaJP, EscapeNewLine(value)); }
        private string _koKR;
        public string? koKR { get => _koKR; set => SetProperty(ref _koKR, EscapeNewLine(value)); }
        private string _plPL;
        public string? plPL { get => _plPL; set => SetProperty(ref _plPL, EscapeNewLine(value)); }
        private string _ptBR;
        public string? ptBR { get => _ptBR; set => SetProperty(ref _ptBR, EscapeNewLine(value)); }
        private string _ruRU;
        public string? ruRU { get => _ruRU; set => SetProperty(ref _ruRU, EscapeNewLine(value)); }
        private string _zhCN;
        public string? zhCN { get => _zhCN; set => SetProperty(ref _zhCN, EscapeNewLine(value)); }
        private string _zhTW;
        public string? zhTW { get => _zhTW; set => SetProperty(ref _zhTW, EscapeNewLine(value)); }

        private string EscapeNewLine(string text)
        {
            return text.Replace("\n", "\\n");
        }

        public TranslationItem(int id, string key, string enUS)
        {
            this.id = id;
            Key = key;
            this.enUS = enUS;
        }

        public TranslationItem() {
            id = 0;
            Key = "testKey";
            enUS = "test string";
        }
    }
}
