using System.Collections.Generic;

namespace D2MTranslator.Models
{
    public class FileSystemItem
    {
        public string Name { get; set; }
        public List<FileSystemItem> Items { get; set; } = new List<FileSystemItem>();
        public bool IsExpanded { get; set; } = true;
        public string ParentPath { get; internal set; }

        public FileSystemItem(string name, string parentPath)
        {
            Name = name;
            ParentPath = parentPath;
        }
    }

}
