using CommunityToolkit.Mvvm.Collections;
using CommunityToolkit.Mvvm.Messaging;
using D2MTranslator.Messages;
using D2MTranslator.Models;
using Ninject.Planning.Targets;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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
        private readonly Dictionary<int, TranslationItem> referenceData = new();
        public readonly Dictionary<string, string> filePaths = new();
        public ReferenceJsonDataService()
        {
            Debug.WriteLine("ReferenceJsonDataService 생성");
            WeakReferenceMessenger.Default.Register<FileOperationMessage>(this, (r, m) =>
            {
                if (m.FileOperation == FileOperation.Open)
                {
                    if (m.FolderType == FolderType.Reference)
                    {
                        filePaths.Clear();
                        referenceData.Clear();
                        ListupAllFile(m.FilePath);
                    }
                }
                
            });
            WeakReferenceMessenger.Default.Register<FileItemSelectedMessage>(this, (r, m) =>
            {
                Debug.WriteLine(m.Item.Name + " is selected");
                
                if (m.Item.FolderType == FolderType.Mod)
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
                                    if (referenceData.ContainsKey(item.id))
                                    {
                                        Debug.WriteLine("중복된 아이디가 있습니다. " + item.id);
                                        return;
                                    }
                                    referenceData.Add(item.id, item);
                                });
                        }
                    }
                }
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
                if (filePaths.ContainsKey(file.Name))
                {
                    Debug.WriteLine("중복된 파일명이 있습니다. " + file.Name);
                    continue;
                }
                filePaths.Add(file.Name, file.FullName);             
            }
        }

        public TranslationItem GetTranslationItem(int id)
        {
            if (referenceData.ContainsKey(id))
            {
                return referenceData[id];
            }
            return null;
        }

        public void AddReferenceFile(FileOperationMessage m)
        {
            CreateFileSystemItem(new DirectoryInfo(m.FilePath));
        }
    }
}
