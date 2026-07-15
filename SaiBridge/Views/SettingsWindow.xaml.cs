using System.Windows;
using SaiBridge.Models;
using SaiBridge.Services;

namespace SaiBridge.Views;

public partial class SettingsWindow : Window
{
    private Settings settings;

    public SettingsWindow()
    {
        InitializeComponent();

        settings =
            SettingsService.Load();

        DarkMode.IsChecked =
            settings.DarkMode;

        GlassEffect.IsChecked =
            settings.GlassEffect;

        AutoStart.IsChecked =
            settings.AutoStart;

        AutoUpdate.IsChecked =
            settings.AutoCheckUpdate;
    }

    private void Save_Click(
        object sender,
        RoutedEventArgs e)
    {
        settings.DarkMode =
            DarkMode.IsChecked == true;

        settings.GlassEffect =
            GlassEffect.IsChecked == true;

        settings.AutoStart =
            AutoStart.IsChecked == true;

        settings.AutoCheckUpdate =
            AutoUpdate.IsChecked == true;

        SettingsService.Save(settings);

        ThemeService.Apply(
    settings.DarkMode);

        Close();
    }
}