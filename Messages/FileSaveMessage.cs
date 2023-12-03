using D2MTranslator.Models;

namespace D2MTranslator.Messages
{
    internal class FileSaveMessage
    {
        public FileSystemItem? selectedModItem;

        public FileSaveMessage(FileSystemItem? selectedModItem)
        {
            this.selectedModItem = selectedModItem;
        }
    }
}