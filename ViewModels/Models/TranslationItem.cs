using System;

namespace D2MTranslator.ViewModels.Models
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:명명 스타일", Justification = "<보류 중>")]
    [Serializable]
    public class TranslationItem
    {
        public int id { get; set; }
        public string Key { get; set; }
        public string? deDE { get; set; }
        public string enUS { get; set; }
        public string? esES { get; set; }
        public string? esMX { get; set; }
        public string? frFR { get; set; }
        public string? itIT { get; set; }
        public string? jaJP { get; set; }
        public string? koKR { get; set; }
        public string? plPL { get; set; }
        public string? ptBR { get; set; }
        public string? ruRU { get; set; }
        public string? zhCN { get; set; }
        public string? zhTW { get; set; }

        public TranslationItem(int id, string key, string enUS)
        {
            this.id = id;
            Key = key;
            this.enUS = enUS;
        }
    }
}
