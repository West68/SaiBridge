using System;
using System.IO;
using System.Linq;

namespace SaiBridge.Services;

public static class FileService
{
    public static string? GetNewestPsd(string folder)
    {
        if (!Directory.Exists(folder))
            return null;

        FileInfo? file =
            new DirectoryInfo(folder)
                .GetFiles("*.psd")
                .OrderByDescending(f => f.LastWriteTime)
                .FirstOrDefault();

        return file?.FullName;
    }
}