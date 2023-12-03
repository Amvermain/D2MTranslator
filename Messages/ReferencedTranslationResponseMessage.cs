namespace D2MTranslator.Models
{
    public class ReferencedTranslationResponseMessage
    {
        public TranslationItem Item { get; }

        public ReferencedTranslationResponseMessage(TranslationItem item)
        {
            Item = item;
        }
    }
}