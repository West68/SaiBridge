using System.Windows;

namespace SaiBridge.Services;

public static class ThemeService
{
    public static bool IsDark { get; private set; }

    public static void Apply(bool dark)
    {
        IsDark = dark;

        var dictionaries =
            Application.Current.Resources.MergedDictionaries;

        dictionaries.Clear();

        dictionaries.Add(
            new ResourceDictionary
            {
                Source = new Uri(
                    dark
                        ? "/Themes/Dark.xaml"
                        : "/Themes/Light.xaml",
                    UriKind.Relative)
            });
    }

    public static void ToggleTheme()
    {
        Apply(!IsDark);
    }
}