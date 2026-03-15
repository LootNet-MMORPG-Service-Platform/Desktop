namespace desktop_app.ViewModels;

public class WelcomeViewModel : ViewModelBase
{
    public MainWindowViewModel Parent { get; }

    public WelcomeViewModel(MainWindowViewModel parent)
    {
        Parent = parent;
    }
}