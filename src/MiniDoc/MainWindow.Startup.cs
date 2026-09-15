namespace MiniDoc;

public partial class MainWindow
{
    public Task<bool> OpenInitialPathAsync(string path)
    {
        var intent = _openTransitions.BeginOpenIntent();
        var ticket = _openTransitions.Capture(intent);
        return OpenPathAsync(path, ticket);
    }
}
