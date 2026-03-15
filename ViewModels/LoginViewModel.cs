namespace desktop_app.ViewModels;

public class LoginViewModel : ViewModelBase
{
    public MainWindowViewModel Parent { get; }

    public LoginViewModel(MainWindowViewModel parent)
    {
        Parent = parent;
    }
}