using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using D2MTranslator.Messages;
using D2MTranslator.Models;
using D2MTranslator.ViewModels.Models;
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
        // TODO: 2. 비교 & 덮어쓰기 or 수정
        // TODO: 3. enUS 수정내역 분석해서 바뀐 것만 체크하도록
        public ObservableCollection<TranslationItem> TranslationItems { get; private set; }

        public InteractiveViewModel()
        {
            TranslationItems = new ObservableCollection<TranslationItem>();
            WeakReferenceMessenger.Default.Register<FileContentMessage>(this, (r, m) =>
            {
                Debug.WriteLine("FileContentMessage Received");
                if (m.FolderType == Enums.FolderType.Mod)
                {
                    var items = JsonSerializer.Deserialize<List<TranslationItem>>(m.Content);
                    TranslationItems.Clear();
                    foreach (var item in items)
                    {
                        TranslationItems.Add(item);
                    }
                }

                WeakReferenceMessenger.Default.Send(new FileOpenFinishMessage());
            });
            var item = new TranslationItem(0, "key", "test");
            TranslationItems.Add(item);
        }
        
    }
}
