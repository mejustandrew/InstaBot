using InstaBotApi.CredentialsHandling;

namespace InstaBot
{
    static class CredentialsManager
    {
        public static Credentials GetLoginData()
        {
            var credentials = CredentialsRepository.ReadCredentials();

            if (credentials == null)
                credentials = GetCredentialsFromUser();

            return credentials;
        }

        public static Credentials RenewCredentials()
        {
            return GetCredentialsFromUser();
        }

        private static Credentials GetCredentialsFromUser()
        {
            var loginInfo = GetLoginInfo();
            var credentials = new Credentials
            {
                Username = loginInfo.Username,
                Password = loginInfo.Password
            };

            if (loginInfo.PersistCredentials)
            {
                CredentialsRepository.SaveCredentials(credentials);
            }

            return credentials;
        }

        private static CredentialsWindowViewModel GetLoginInfo()
        {
            var window = new CredentialsWindow();
            var viewModel = new CredentialsWindowViewModel();
            window.DataContext = viewModel;
            window.ShowDialog();

            return viewModel;
        }
    }
}
