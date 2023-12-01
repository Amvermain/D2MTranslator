using D2MTranslator.Models;

namespace D2MTranslator.Messages
{
    public class FileItemSelectedMessage
    {
        public FileSystemItem Item { get; }        

        public FileItemSelectedMessage(FileSystemItem item)
        {
            Item = item;
        }
    }

}