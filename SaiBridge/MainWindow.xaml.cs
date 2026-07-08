using System.Windows;
using SaiBridge.Models;
using SaiBridge.Services;

namespace SaiBridge;

public partial class MainWindow : Window
{
    private Settings settings = new();

    public MainWindow()
    {
        InitializeComponent();

        settings = SettingsService.Load();

        SaiPath.Text = settings.SaiPath;
        CspPath.Text = settings.CspPath;
        WorkspacePath.Text = settings.WorkspacePath;
    }

    protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
    {
        settings.SaiPath = SaiPath.Text;
        settings.CspPath = CspPath.Text;
        settings.WorkspacePath = WorkspacePath.Text;

        SettingsService.Save(settings);

        base.OnClosing(e);
    }
}