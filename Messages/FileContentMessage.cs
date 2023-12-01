namespace D2MTranslator.ViewModels.Models
{
    public class FileContentMessage
    {
        public string Content { get; private set; }
        public Enums.FolderType FolderType { get; private set; }

        public FileContentMessage(string content, Enums.FolderType mod)
        {
            Content = content;
            FolderType = mod;
        }
    }
}
