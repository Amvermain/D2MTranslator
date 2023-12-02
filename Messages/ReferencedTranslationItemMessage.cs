using D2MTranslator.Models;

namespace D2MTranslator.Messages
{
    public class ReferencedTranslationItemMessage
    {
        public TranslationItem Item { get; }


        public ReferencedTranslationItemMessage(TranslationItem translationItem) {
            Item = translationItem;
        }
    }
}