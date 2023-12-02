using CommunityToolkit.Mvvm.Collections;
using CommunityToolkit.Mvvm.Messaging;
using D2MTranslator.Messages;
using D2MTranslator.Models;
using Ninject.Planning.Targets;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static D2MTranslator.Enums;

namespace D2MTranslator.Services
{
    public class ReferenceJsonDataService
    {
        private Dictionary<int, TranslationItem> referenceData = new();
        private Dictionary<string, string> filePaths = new();
        public ReferenceJsonDataService()
        {
            WeakReferenceMessenger.Default.Register<FileOperationMessage>(this, (r, m) =>
            {
                if (m.FolderType == FolderType.Reference)
                {
                    filePaths.Clear();
                    referenceData.Clear();
                    ListupAllFile(m.FilePath);
                }
            });
            WeakReferenceMessenger.Default.Register<FileItemSelectedMessage>(this, (r, m) =>
            {
                referenceData.Clear();
                var filename = m.Item.Name;
                if (filePaths.ContainsKey(filename))
                {
                    var filePath = filePaths[filename];
                    if (filePath != null)
                    {
                        var fileContent = File.ReadAllText(filePath);
                        var result = JsonSerializer.Deserialize<List<TranslationItem>>(fileContent);

                        if (result != null && result.Count > 0)
                            result.ForEach((item) => {
                                referenceData.Add(item.id, item);
                            });
                    }
                }
            });
            WeakReferenceMessenger.Default.Register<TranslationItemSelectedMessage>(this, (r, m) =>
            {
                WeakReferenceMessenger.Default.Send(new ReferencedTranslationItemMessage(referenceData[m.Id]));
            });
        }

        private void ListupAllFile(string filePath)
        {
            DirectoryInfo dirInfo = new DirectoryInfo(filePath);
            CreateFileSystemItem(dirInfo);
        }

        private void CreateFileSystemItem(DirectoryInfo dirInfo)
        {
            foreach (var dir in dirInfo.GetDirectories())
            {
                CreateFileSystemItem(dir);
            }

            foreach (var file in dirInfo.GetFiles("*.json"))
            {
                filePaths.Add(file.Name, file.FullName);             
            }
        }
    }
}
