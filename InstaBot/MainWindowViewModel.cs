using System.Threading;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Linq;
using InstaBotApi;

namespace InstaBot
{
    enum WaitingPeriod
    {
        Short,
        Medium,
        Long
    }

    public class MainWindowViewModel : NotifiableObject
    {
        public ICommand StartCommand { get; }
        public ICommand StopCommand { get; }
        public bool IsBotRunning { get; set; }
        public ObservableCollection<TagSpecsViewModel> TagSpecs { get; set; }
        public bool RenewCredentialsOnStart { get; set; }

        private bool _isEditingMode;

        public bool IsEditingMode
        {
            get => _isEditingMode;
            set
            {
                _isEditingMode = value;
                IsBotRunning = !_isEditingMode;
            }
        }

        public MainWindowViewModel()
        {
            StartCommand = new RelayCommand(StartRunning);
            StopCommand = new RelayCommand(StopRunning);
            var specs = SettingsRepository.ReadTagSettings();
            TagSpecs = new ObservableCollection<TagSpecsViewModel>(specs);
            IsEditingMode = true;
        }

        private void StartRunning(object obj)
        {
            SettingsRepository.SaveTagSettings(TagSpecs.ToList());

            IsEditingMode = false;
            if (RenewCredentialsOnStart)
                CredentialsManager.RenewCredentials();

            var tags = TagSpecs.Select(x => new Tag { LikesNumber = x.LikesNumber, Name = x.TagName });
            BotRunner.RunBotForTagsAsync(tags);
        }

        private void StopRunning(object obj)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            LikesLogger.LogStop();
            BotRunner.StopRunning();
            Mouse.OverrideCursor = null;
            IsEditingMode = true;
        }
    }
}
