namespace D2MTranslator.Messages
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