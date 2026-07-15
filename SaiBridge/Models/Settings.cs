namespace SaiBridge.Models;

public class Settings
{
    public string SaiPath { get; set; } = "";

    public string CspPath { get; set; } = "";

    public string PsdCachePath { get; set; } = "";

    public string WorkspacePath { get; set; } = "";


    public bool DarkMode { get; set; } = true;

    public bool GlassEffect { get; set; } = true;


    public string Language { get; set; } = "zh-CN";


    public bool AutoStart { get; set; } = false;

    public bool AutoCheckUpdate { get; set; } = true;
}