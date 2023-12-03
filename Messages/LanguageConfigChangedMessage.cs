namespace D2MTranslator.Messages
{
    public class LanguageConfigChangedMessage
    {

        public string PropertyName { get; private set; }
        public bool Value { get; private set; }
        public LanguageConfigChangedMessage(string propertyName, bool value)
        {
            PropertyName = propertyName;
            Value = value;
        }
    }
}