using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace InstaBot
{
    static class SettingsRepository
    {
        private const string PreferencesFilePath = "Preferences.txt";

        public static void SaveTagSettings(List<TagSpecsViewModel> tagSpecs)
        {
            var serializedTags = JsonConvert.SerializeObject(tagSpecs);
            File.WriteAllText(PreferencesFilePath, serializedTags);
        }

        public static List<TagSpecsViewModel> ReadTagSettings()
        {
            if (!File.Exists(PreferencesFilePath))
                return new List<TagSpecsViewModel>();

            var serializedTags = File.ReadAllText(PreferencesFilePath);
            return JsonConvert.DeserializeObject<List<TagSpecsViewModel>>(serializedTags);
        }
    }
}
