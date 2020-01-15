using Serilog;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows;

namespace InstaBot
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
#if DEBUG
            Process.Start("chromekiller.exe");
#endif
            Log.Logger = new LoggerConfiguration()
                .WriteTo.File(Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "logs", "Log.txt"), rollingInterval: RollingInterval.Day)
                .CreateLogger();

            InitializeComponent();

            DataContext = new MainWindowViewModel();
        }
    }
}
