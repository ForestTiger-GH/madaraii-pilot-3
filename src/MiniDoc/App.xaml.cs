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

        var verificationClose = e.Args.Length >= 2 &&
            string.Equals(e.Args[0], "--verify-close-after-open", StringComparison.Ordinal);
        var rawPath = verificationClose ? e.Args[1] : e.Args[0];
        var path = Path.GetFullPath(rawPath);
        if (!File.Exists(path)) return;

        await window.OpenInitialPathAsync(path);
        if (verificationClose) window.Close();
    }
}
