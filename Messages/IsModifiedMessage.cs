namespace D2MTranslator.ViewModels
{
    internal class IsModifiedMessage
    {
        public bool IsModified { get; internal set; }

        public IsModifiedMessage(bool isModified)
        {
            IsModified = isModified;
        }
    }
}