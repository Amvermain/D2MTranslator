using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using D2MTranslator.Messages;
using D2MTranslator.Models;
using D2MTranslator.Services;
using D2MTranslator.ViewModels.Models;
using Ninject;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace D2MTranslator.ViewModels
{
    public class InteractiveViewModel : ObservableObject
    {
        // TODO: 1. byte limit check
        public ObservableCollection<TranslationItem> TranslationItems { get; private set; }
        private ReferenceJsonDataService referenceJsonDataService;

        public InteractiveViewModel()
        {
            TranslationItems = new ObservableCollection<TranslationItem>();
            referenceJsonDataService = App.Kernel.Get<ReferenceJsonDataService>();
            WeakReferenceMessenger.Default.Register<FileContentMessage>(this, (r, m) =>
            {
                Debug.WriteLine("FileContentMessage Received");
                if (m.FolderType == Enums.FolderType.Mod)
                {
                    var items = JsonSerializer.Deserialize<List<TranslationItem>>(m.Content);
                    TranslationItems.Clear();
                    foreach (var item in items)
                    {
                        var referenceItem = referenceJsonDataService.GetTranslationItem(item.id);
                        if (referenceItem != null)
                        {
                            item.referenceItem = referenceItem;
                        }
                        TranslationItems.Add(item);
                    }
                }

                WeakReferenceMessenger.Default.Send(new FileOpenFinishMessage());
            });
            WeakReferenceMessenger.Default.Register<FileSaveMessage>(this, (r, m) => { 
                Debug.WriteLine("FileSaveMessage Received");
                var json = JsonSerializer.Serialize(value: TranslationItems.ToList(), options: new JsonSerializerOptions() {
                    WriteIndented = true,
                    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                });
                WeakReferenceMessenger.Default.Send(new SavedFileContentMessage(json, m.selectedModItem));
            });
            //var item = new TranslationItem(0, "key", "test");
            //TranslationItems.Add(item);
        }
    }
}
