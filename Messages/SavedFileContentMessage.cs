using D2MTranslator.Models;

namespace D2MTranslator.Messages
{
    public class SavedFileContentMessage
    {
        public string json;
        public FileSystemItem? selectedModItem;

        public SavedFileContentMessage(string json, FileSystemItem? selectedModItem)
        {
            this.json = json;
            this.selectedModItem = selectedModItem;
        }
    }
}