using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D2MTranslator.Messages
{
    public class FileOperationMessage
    {
        public Enums.FileOperation FileOperation { get; set; }
        public string FilePath { get; set; }
        public Enums.FolderType? FolderType { get; set; }

        public FileOperationMessage(Enums.FileOperation fileOperation, string filePath, Enums.FolderType? folderType)
        {
            FileOperation = fileOperation;
            FilePath = filePath;
            FolderType = folderType;
        }

        public FileOperationMessage(Enums.FileOperation save)
        {
            FileOperation = save;
        }
    }
}
