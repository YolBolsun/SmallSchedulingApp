using Microsoft.UI.Xaml;
using SmallSchedulingApp.Helpers;
using SmallSchedulingApp.Services;

namespace SmallSchedulingApp
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application
    {
        private MainWindow? _window;
        private SystemTrayHelper? _systemTrayHelper;

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            InitializeComponent();

            // Force dark theme
            this.RequestedTheme = ApplicationTheme.Dark;
        }

        /// <summary>
        /// Invoked when the application is launched.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            // Create window but don't show it (system tray app)
            _window = new MainWindow();

            // Initialize system tray
            _systemTrayHelper = new SystemTrayHelper(_window);
            _systemTrayHelper.Initialize();

            // Set the system tray helper in the window so it can handle messages
            _window.SetSystemTrayHelper(_systemTrayHelper);

            // Handle cleanup when window is actually closed
            _window.Closed += (s, e) =>
            {
                _systemTrayHelper?.Dispose();
                DatabaseService.Instance.Dispose();
            };
        }
    }
}
