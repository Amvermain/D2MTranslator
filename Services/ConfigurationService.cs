using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using D2MTranslator.Messages;
using D2MTranslator.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.Json;

namespace D2MTranslator.Services
{
    [Serializable]
    public class ConfigurationService: ObservableObject
    {
        public bool isHidingSameTranslation = true;

        public Dictionary<string, bool> LanguageVisibility { get; set; }
        public string? LastOpenedModDir { get; set; }
        public string? LastOpenedRefDir { get; set; }
        public ConfigurationService()
        {
            LanguageVisibility = new Dictionary<string, bool>() {
                { "deDE", true },
                { "esES", true },
                { "esMX", true },
                { "frFR", true },
                { "itIT", true },
                { "jaJP", true },
                { "koKR", true },
                { "plPL", true },
                { "ptBR", true },
                { "ruRU", true },
                { "zhCN", true },
                { "zhTW", true }
            };
            LastOpenedModDir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            LastOpenedRefDir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            WeakReferenceMessenger.Default.Register<LanguageConfigChangedMessage>(this, (r, m) =>
            {
                if (LanguageVisibility.ContainsKey(m.PropertyName))
                    LanguageVisibility[m.PropertyName] = m.Value;
                else
                {
                    if (m.PropertyName == "SkipSame")
                    {
                        Debug.WriteLine("Skip Same " + m.Value);
                        isHidingSameTranslation = m.Value;
                    }
                }

                Debug.WriteLine($"LanguageConfigChangedMessage Received: {m.PropertyName} {m.Value}");
                OnPropertyChanged(nameof(LanguageVisibility));
            });
            LoadConfiguration();
        }

        private void LoadConfiguration()
        {
            var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var appConfigFolder = Path.Combine(appDataPath, "D2MTranslator");
            if (!Directory.Exists(appConfigFolder))
            {
                Directory.CreateDirectory(appConfigFolder);
            }
            var configFilePath = Path.Combine(appConfigFolder, "config.json");
            if (File.Exists(configFilePath))
            {
                var jsonText = File.ReadAllText(configFilePath);
                JsonElement jsonElement = JsonSerializer.Deserialize<JsonElement>(jsonText);
                if (jsonElement.TryGetProperty("LanguageVisibility", out var languageVisibilityElement))
                {
                    var visibility = JsonSerializer.Deserialize<Dictionary<string, bool>>(languageVisibilityElement.GetRawText());
                    if (visibility != null)
                        LanguageVisibility = visibility;
                }

                if (jsonElement.TryGetProperty("LastOpenedModDir", out var lastOpenedModDirElement))
                {
                    LastOpenedModDir = lastOpenedModDirElement.GetString();
                }

                if (jsonElement.TryGetProperty("LastOpenedRefDir", out var lastOpenedRefDirElement))
                {
                    LastOpenedRefDir = lastOpenedRefDirElement.GetString();
                }

                if (jsonElement.TryGetProperty("isHidingSameTranslation", out var isHidingSameTranslationElement))
                {
                    isHidingSameTranslation = isHidingSameTranslationElement.GetBoolean();
                }
            }
            WeakReferenceMessenger.Default.Send(new LanguageConfigLoadedMessage());
            
        }

        public void Save()
        {
            var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var appConfigFolder = Path.Combine(appDataPath, "D2MTranslator");
            if (!Directory.Exists(appConfigFolder))
            {
                Directory.CreateDirectory(appConfigFolder);
            }
            var configFilePath = Path.Combine(appConfigFolder, "config.json");
            var jsonText = JsonSerializer.Serialize(this);
            File.WriteAllText(configFilePath, jsonText);
        }
    }
}
