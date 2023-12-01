namespace D2MTranslator.ViewModels
{
    public class IsModifiedMessage
    {
        public bool IsModified { get; internal set; }

        public IsModifiedMessage(bool isModified)
        {
            IsModified = isModified;
        }
    }
}