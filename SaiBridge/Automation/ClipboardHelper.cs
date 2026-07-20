using System.Windows;

namespace SaiBridge.Automation;

public static class ClipboardHelper
{
    public static void SetText(string text)
    {
        Clipboard.SetText(text);
    }
}