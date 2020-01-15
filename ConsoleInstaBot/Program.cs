using InstaBotApi;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace ConsoleInstaBot
{
    class Program
    {
        static void Main(string[] args)
        {
            var serializedTags = File.ReadAllText("Preferences.txt");
            var tags = JsonConvert.DeserializeObject<List<Tag>>(serializedTags);
            //BotRunner.RunBotForTagsAsync(tags);
        }
    }
}
