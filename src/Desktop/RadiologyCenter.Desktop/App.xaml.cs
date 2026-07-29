namespace RadiologyCenter.Desktop;

public partial class App : Application
{
    private readonly LocalhostService _localhost;

    public App()
    {
        InitializeComponent();
        _localhost = new LocalhostService();
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        var window = new Window(new MainPage()) { Title = "RadiologyCenter" };
        window.Destroying += (_, _) => _localhost.Stop();
        return window;
    }

    protected override void OnStart()
    {
        _ = _localhost.StartAsync();
    }
}
