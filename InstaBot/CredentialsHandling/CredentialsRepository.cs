using EncryptString;
using Newtonsoft.Json;
using System.IO;

namespace InstaBot
{
    static class CredentialsRepository
    {
        private const string CredentialsFilePath = "Credentials.txt";

        public static Credentials ReadCredentials()
        {
            if (!File.Exists(CredentialsFilePath))
                return null;

            var fileContent = File.ReadAllText(CredentialsFilePath);
            var decriptedContent = StringCipher.Decrypt(fileContent);
            return JsonConvert.DeserializeObject<Credentials>(decriptedContent);
        }

        public static void SaveCredentials(Credentials credentials)
        {
            var serializedCredentials = JsonConvert.SerializeObject(credentials);
            var encriptedContent = StringCipher.Encrypt(serializedCredentials);
            File.WriteAllText(CredentialsFilePath, encriptedContent);
        }
    }
}
