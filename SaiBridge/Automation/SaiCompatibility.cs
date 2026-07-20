namespace SaiBridge.Automation;

public static class SaiCompatibility
{
    // 所有保存窗口

    public static readonly string[] SaveDialogs =
    {
        "保存",
        "另存",
        "导出",
        "Save",
        "Export"
    };

    // 所有打开窗口

    public static readonly string[] OpenDialogs =
    {
        "打开",
        "Open"
    };

    // 所有重新打开窗口

    public static readonly string[] ReopenDialogs =
    {
        "重新打开",
        "Reopen"
    };

    // 所有路径栏

    public static readonly string[] LocationNames =
    {
        "位置",
        "路径",
        "Location"
    };

    // OK按钮

    public static readonly string[] OkButtons =
    {
        "OK",
        "确定",
        "打开",
        "保存"
    };

    public static readonly string[] OpenButtons =
{
    "打开",
    "Open",
    "OK"
};

    public static readonly string[] PathNames =
    {
    "位置",
    "路径",
    "Location"
};

    public static bool Match(
    string title,
    params string[] keywords)
    {
        foreach (string keyword in keywords)
        {
            if (title.Contains(keyword))
            {
                return true;
            }
        }

        return false;
    }
}