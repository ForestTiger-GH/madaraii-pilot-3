using System.IO;
using System.Windows;

namespace MiniDoc;

public partial class App : Application
{
    private async void Application_Startup(object sender, StartupEventArgs e)
    {
        var window = new MainWindow();
        MainWindow = window;
        window.Show();

        if (e.Args.Length == 0) return;
        var path = Path.GetFullPath(e.Args[0]);
        if (File.Exists(path)) await window.OpenInitialPathAsync(path);
    }
}
