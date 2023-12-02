using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace D2MTranslator.Services
{
    public class ConfigurationService
    {
        public Dictionary<string, bool> LanguageVisibility { get; set; }
        public ConfigurationService()
        {
            
            LanguageVisibility = new Dictionary<string, bool>();
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
                LanguageVisibility = JsonSerializer.Deserialize<Dictionary<string, bool>>(jsonText);
            }
        }
    }
}
