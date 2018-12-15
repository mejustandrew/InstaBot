using System.Windows.Input;

namespace InstaBot
{
    class CredentialsWindowViewModel : NotifiableObject
    {
        public ICommand LoginCommand { get; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool PersistCredentials { get; set; } = true;
    }
}
