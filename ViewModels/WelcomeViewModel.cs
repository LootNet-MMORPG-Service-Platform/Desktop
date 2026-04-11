namespace desktop_app.ViewModels;

public class WelcomeViewModel : ViewModelBase
{
    public AuthShellViewModel Parent { get; }

    public WelcomeViewModel(AuthShellViewModel parent)
    {
        Parent = parent;
    }
}