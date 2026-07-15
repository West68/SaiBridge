using System.Windows;
using SaiBridge.Services;

namespace SaiBridge
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var settings = SettingsService.Load();

            ThemeService.Apply(settings.DarkMode);
        }
    }
}