using Newtonsoft.Json;
using System.IO;
using System.Reflection;

namespace InstaBotApi.CredentialsHandling
{
    public static class CredentialsRepository
    {
        private static string CredentialsFilePath = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "Credentials.txt");

        public static Credentials ReadCredentials()
        {
            if (!File.Exists(CredentialsFilePath))
                return null;

            var fileContent = File.ReadAllText(CredentialsFilePath);
            var decriptedContent = Encryption.DecryptString(fileContent);
            return JsonConvert.DeserializeObject<Credentials>(decriptedContent);
        }

        public static void SaveCredentials(Credentials credentials)
        {
            var serializedCredentials = JsonConvert.SerializeObject(credentials);
            var encriptedContent = Encryption.EncryptString(serializedCredentials);
            File.WriteAllText(CredentialsFilePath, encriptedContent);
        }
    }
}
