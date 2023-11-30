using CommunityToolkit.Mvvm.ComponentModel;

namespace D2MTranslator.Models
{
    public class LineItem : ObservableObject
    {
        private string _text;
        public string Text
        {
            get => _text;
            set
            {
                if (_text != value)
                {
                    _text = value;
                    OnPropertyChanged(nameof(Text));
                }
            }
        }
    }

}
