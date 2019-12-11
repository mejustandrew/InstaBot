using System.Diagnostics;
using System.Linq;
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

            InitializeComponent();

            DataContext = new MainWindowViewModel();
        }
    }
}
