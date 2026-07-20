using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Automation;

namespace SaiBridge.Automation;

public static class SaiAutomation
{
    public static Action<string>? Logger;

    //==================================================
    // Win32
    //==================================================

    [DllImport("user32.dll")]
    private static extern bool SetForegroundWindow(IntPtr hWnd);

    [DllImport("user32.dll")]
    private static extern IntPtr GetForegroundWindow();

    public static IntPtr FindForegroundWindow()
    {
        return GetForegroundWindow();
    }


    [DllImport("user32.dll")]
    private static extern bool ShowWindow(
        IntPtr hWnd,
        int nCmdShow);


    [DllImport("user32.dll")]
    private static extern bool IsIconic(
        IntPtr hWnd);


    [DllImport("user32.dll")]
    private static extern bool EnumWindows(
        EnumWindowsProc lpEnumFunc,
        IntPtr lParam);


    [DllImport("user32.dll")]
    private static extern int GetWindowText(
        IntPtr hWnd,
        StringBuilder text,
        int maxLength);

    // ★诊断用：公开窗口标题获取
    public static void GetWindowTextForDiag(IntPtr hWnd, StringBuilder text, int maxLength)
    {
        GetWindowText(hWnd, text, maxLength);
    }


    [DllImport("user32.dll")]
    private static extern uint GetWindowThreadProcessId(
        IntPtr hWnd,
        out uint processId);



    private delegate bool EnumWindowsProc(
        IntPtr hWnd,
        IntPtr lParam);



    [DllImport("user32.dll")]
    private static extern void keybd_event(
        byte bVk,
        byte bScan,
        uint dwFlags,
        UIntPtr dwExtraInfo);



    private const int SW_RESTORE = 9;

    private const uint KEYEVENTF_KEYUP = 0x0002;



    // 键值

    private const byte VK_CONTROL = 0x11;
    private const byte VK_SHIFT = 0x10;

    private const byte VK_S = 0x53;
    private const byte VK_L = 0x4C;
    private const byte VK_V = 0x56;
    private const byte VK_A = 0x41;

    public static void PressCtrlA()
    {
        KeyDown(VK_CONTROL);
        KeyPress(VK_A);
        KeyUp(VK_CONTROL);
    }

    private const byte VK_RETURN = 0x0D;
    private const byte VK_ESCAPE = 0x1B;
    private const byte VK_TAB = 0x09;

    private const byte VK_UP = 0x26;
    private const byte VK_DOWN = 0x28;
    private const byte VK_LEFT = 0x25;
    private const byte VK_RIGHT = 0x27;

    private const byte VK_F5 = 0x74;
    private const byte VK_END = 0x23;

    // 键按下/抬起间隔（毫秒）
    private const int KeyInterval = 10;
    // 组合键间隔
    private const int ComboInterval = 10;



    //==================================================
    // 查找 SAI
    //==================================================


    public static Process? FindSaiProcess(string exePath)
    {
        if (!File.Exists(exePath))
            return null;


        string name =
            Path.GetFileNameWithoutExtension(exePath);


        foreach (Process p in Process.GetProcessesByName(name))
        {
            if (!p.HasExited)
                return p;
        }


        return null;
    }




    //==================================================
    // 激活 SAI
    //==================================================


    public static void Activate(Process process)
    {
        IntPtr hwnd = process.MainWindowHandle;


        if (hwnd == IntPtr.Zero)
            return;


        if (IsIconic(hwnd))
        {
            ShowWindow(hwnd, SW_RESTORE);
        }


        ShowWindow(hwnd, SW_RESTORE);


        Thread.Sleep(20);


        bool activated = SetForegroundWindow(hwnd);

        if (!activated)
        {
            System.Diagnostics.Debug.WriteLine("SetForegroundWindow 失败");
        }

        Thread.Sleep(40);
    }





    //==================================================
    // 快捷键
    //==================================================


    public static void PressCtrlS()
    {
        KeyDown(VK_CONTROL);
        KeyPress(VK_S);
        KeyUp(VK_CONTROL);
    }



    public static void PressCtrlShiftS()
    {
        KeyDown(VK_CONTROL);
        KeyDown(VK_SHIFT);

        KeyPress(VK_S);

        KeyUp(VK_SHIFT);
        KeyUp(VK_CONTROL);
    }
    public static void PressCtrlO()
    {
        KeyDown(VK_CONTROL);
        KeyPress(0x4F);      // O
        KeyUp(VK_CONTROL);
    }

    public static void ExportPsd()
    {
        // Alt+F
        KeyDown(0x12);
        KeyPress(0x46);
        KeyUp(0x12);

        Thread.Sleep(50);

        KeyPress(0x45);

        Thread.Sleep(80);

        PressDown();

        Thread.Sleep(30);

        PressEnter();

        Thread.Sleep(150);
    }


    public static void PressCtrlL()
    {
        KeyDown(VK_CONTROL);
        KeyPress(VK_L);
        KeyUp(VK_CONTROL);
    }





    public static void PressCtrlV()
    {
        KeyDown(VK_CONTROL);
        KeyPress(VK_V);
        KeyUp(VK_CONTROL);
    }



    public static void PressEnter()
    {
        KeyPress(VK_RETURN);
    }



    public static void PressEsc()
    {
        KeyPress(VK_ESCAPE);
    }



    public static void PressTab()
    {
        KeyPress(VK_TAB);
    }



    public static void PressUp()
    {
        KeyPress(VK_UP);
    }


    public static void PressDown()
    {
        KeyPress(VK_DOWN);
    }


    public static void PressLeft()
    {
        KeyPress(VK_LEFT);
    }


    public static void PressRight()
    {
        KeyPress(VK_RIGHT);
    }



    public static void PressF5()
    {
        KeyPress(VK_F5);
    }

    public static void PressEnd()
    {
        KeyPress(VK_END);
    }




    public static void Wait(int ms)
    {
        Thread.Sleep(ms);
    }




    //==================================================
    // 查找 打开文件对话框
    //==================================================

    private static readonly string[] OpenDialogTitles =
    {
        "打开",
        "Open"
    };

    /// <summary>
    /// 轮询查找文件打开对话框，超时返回 IntPtr.Zero
    /// </summary>
    public static IntPtr FindOpenDialogWindow(int timeoutMs = 3000)
    {
        IntPtr result = IntPtr.Zero;
        int elapsed = 0;
        const int interval = 100;

        while (elapsed < timeoutMs)
        {
            EnumWindows(
                (hWnd, lParam) =>
                {
                    StringBuilder sb = new StringBuilder(256);
                    GetWindowText(hWnd, sb, sb.Capacity);
                    string title = sb.ToString();

                    foreach (string keyword in OpenDialogTitles)
                    {
                        if (title.Contains(keyword))
                        {
                            result = hWnd;
                            return false; // 找到，停止枚举
                        }
                    }

                    return true;
                },
                IntPtr.Zero);

            if (result != IntPtr.Zero)
                return result;

            Thread.Sleep(interval);
            elapsed += interval;
        }

        return IntPtr.Zero;
    }

    //==================================================
    // 查找 SAI 保存窗口
    //==================================================


    public static IntPtr FindSaveDialog(Process sai)
    {
        IntPtr result = IntPtr.Zero;



        EnumWindows(
            (hWnd, lParam) =>
            {

                GetWindowThreadProcessId(
                    hWnd,
                    out uint pid);



                // 只检查 SAI 自己的窗口

                if (pid != sai.Id)
                    return true;



                StringBuilder sb =
                    new StringBuilder(256);



                GetWindowText(
                    hWnd,
                    sb,
                    sb.Capacity);



                string title =
                    sb.ToString();


                if (SaiCompatibility.Match(
        title,
        SaiCompatibility.SaveDialogs))
                {
                    result = hWnd;
                    return false;
                }

                return true;






            },
            IntPtr.Zero);



        return result;
    }




    //==================================================
    // 键盘底层
    //==================================================


    private static void KeyDown(byte key)
    {
        keybd_event(
            key,
            0,
            0,
            UIntPtr.Zero);
    }



    private static void KeyUp(byte key)
    {
        keybd_event(
            key,
            0,
            KEYEVENTF_KEYUP,
            UIntPtr.Zero);
    }



    private static void KeyPress(byte key)
    {
        KeyDown(key);

        Thread.Sleep(KeyInterval);

        KeyUp(key);

        Thread.Sleep(KeyInterval);
    }








    public static void TestAutomationPath(IntPtr hwnd)
    {
        AutomationElement root =
            AutomationElement.FromHandle(hwnd);


        AutomationElement? path =
            root.FindFirst(
                TreeScope.Descendants,
                new PropertyCondition(
                    AutomationElement.NameProperty,
                    "路径"));


        if (path == null)
        {
            Console.WriteLine("没有找到路径节点");
            return;
        }


        Console.WriteLine("找到路径节点");

        Console.WriteLine(
            path.Current.ControlType.ProgrammaticName);
    }
    public static void SetFileName(string text)
    {
        Thread.Sleep(500);

        PressCtrlA();

        Thread.Sleep(100);

        Clipboard.SetText(text);

        PressCtrlV();

        Thread.Sleep(500);
    }




    public static void SetPath(
     IntPtr dialog,
     string path)
    {
        var rect =
            MouseHelper.GetRect(dialog);


        int x =
            rect.Left + 500;


        int y =
            rect.Top + 55;


        MouseHelper.Click(
            x,
            y);


        Thread.Sleep(50);

        PressCtrlA();

        Thread.Sleep(10);

        Clipboard.SetText(path);

        PressCtrlV();

        Thread.Sleep(50);

        PressEnter();

        Thread.Sleep(100);
    }

    public static void OpenFileInCsp(
    IntPtr openDialog,
    string filePath)
    {
        string folder =
    Path.GetDirectoryName(filePath)!;

        string file =
            Path.GetFileName(filePath);

        Console.WriteLine(file);

        // ★诊断：记录传入的窗口句柄和标题
        {
            StringBuilder tsb = new StringBuilder(256);
            GetWindowText(openDialog, tsb, tsb.Capacity);
            Logger?.Invoke($"[DIAG] OpenFileInCsp openDialog=0x{openDialog:X} title=\"{tsb}\"\n");
        }

        // 地址栏输入文件夹
        PressCtrlL();

        Thread.Sleep(30);

        Clipboard.SetText(folder);

        PressCtrlV();

        Thread.Sleep(30);

        PressEnter();

        Thread.Sleep(100);

        // ★诊断：检查 Enter 之后的前台窗口
        {
            IntPtr fg = GetForegroundWindow();
            StringBuilder tsb = new StringBuilder(256);
            GetWindowText(fg, tsb, tsb.Capacity);
            Logger?.Invoke($"[DIAG] After Enter, foreground=0x{fg:X} title=\"{tsb}\"\n");
        }

        // 文件名已通过 UIAutomation 设置，焦点也已在文件名框上
        SetOpenDialogFileName(
    openDialog,
    file);

        Thread.Sleep(50);

        // ★诊断：检查 SetOpenDialogFileName 之后的前台窗口
        {
            IntPtr fg = GetForegroundWindow();
            StringBuilder tsb = new StringBuilder(256);
            GetWindowText(fg, tsb, tsb.Capacity);
            Logger?.Invoke($"[DIAG] After SetFileName, foreground=0x{fg:X} title=\"{tsb}\"\n");
        }

        PressEnter();
    }
    public static void ConfirmOverwrite()
    {
        Thread.Sleep(50);

        PressEnter();

        Thread.Sleep(80);
    }

    public static void ExportPsdWorkflow(
    Process sai,
    string psdCachePath)
    {


        Wait(50);

        ExportPsd();

        Wait(100);

        IntPtr dialog =
            FindSaveDialog(sai);

        if (dialog == IntPtr.Zero)
            return;

        SetPath(
            dialog,
            psdCachePath);

        Wait(200);

        PressEnter();

        Wait(200);

        PressEnter();

        Wait(200);

        PressEnter();
    }


    public static void SetOpenDialogFileName(
    IntPtr hwnd,
    string fileName)
    {
        AutomationElement root =
            AutomationElement.FromHandle(hwnd);

        AutomationElement? edit = null;

        // ★诊断：记录开始搜索
        Logger?.Invoke($"[DIAG] SetOpenDialogFileName start, hwnd=0x{hwnd:X}, file=\"{fileName}\"\n");

        // 重试：对话框文件列表可能还在加载，UIA 树未就绪
        for (int i = 0; i < 10; i++)
        {
            edit = root.FindFirst(
                TreeScope.Descendants,
                new AndCondition(
                    new PropertyCondition(
                        AutomationElement.AutomationIdProperty,
                        "1148"),
                    new PropertyCondition(
                        AutomationElement.ControlTypeProperty,
                        ControlType.Edit)));

            if (edit != null)
                break;

            Logger?.Invoke($"[DIAG] SetOpenDialogFileName retry {i}/10: edit not found\n");
            Thread.Sleep(50);
        }

        if (edit == null)
        {
            Logger?.Invoke("[DIAG] SetOpenDialogFileName FAILED: edit not found after 10 retries\n");
            return;
        }

        Logger?.Invoke($"[DIAG] SetOpenDialogFileName: edit found, ClassName=\"{edit.Current.ClassName}\"\n");

        // 先设焦点到文件名输入框，再设值
        try
        {
            edit.SetFocus();
            Logger?.Invoke("[DIAG] SetOpenDialogFileName: SetFocus OK\n");
        }
        catch (Exception ex)
        {
            Logger?.Invoke($"[DIAG] SetOpenDialogFileName: SetFocus threw: {ex.Message}\n");
        }

        Thread.Sleep(20);

        if (edit.TryGetCurrentPattern(
    ValuePattern.Pattern,
    out object pattern))
        {
            ((ValuePattern)pattern).SetValue(fileName);
            Logger?.Invoke("[DIAG] SetOpenDialogFileName: SetValue OK\n");
        }
        else
        {
            Logger?.Invoke("[DIAG] SetOpenDialogFileName: ValuePattern NOT available\n");
        }
    }

    //==================================================
    // Explorer
    //==================================================

    public static void OpenExplorerSelect(
        string filePath)
    {
        if (!File.Exists(filePath))
            return;

        Process.Start(
            "explorer.exe",
            $"/select,\"{filePath}\"");
    }

    public static System.Windows.Rect GetSelectedExplorerItemRect(
    IntPtr explorer)
    {
        AutomationElement root =
            AutomationElement.FromHandle(explorer);

        AutomationElementCollection items =
     root.FindAll(
         TreeScope.Descendants,
         System.Windows.Automation.Condition.TrueCondition);

        foreach (AutomationElement item in items)
        {
            try
            {
                if (item.Current.ControlType != ControlType.ListItem)
                    continue;

                if (item.TryGetCurrentPattern(
                    SelectionItemPattern.Pattern,
                    out object pattern))
                {
                    SelectionItemPattern p =
                        (SelectionItemPattern)pattern;

                    if (p.Current.IsSelected)
                    {
                        return item.Current.BoundingRectangle;
                    }
                }
            }
            catch
            {
            }
        }

        return System.Windows.Rect.Empty;

    }

    public static void DragSelectedFileToSai(
     Process sai)
    {
        Wait(500);

        IntPtr explorer =
            FindForegroundWindow();


        if (explorer == IntPtr.Zero)
        {
            MessageBox.Show("没有找到 Explorer");
            return;
        }


        var saiRect =
            MouseHelper.GetRect(
                sai.MainWindowHandle);


        System.Windows.Rect rect = System.Windows.Rect.Empty;

        // 最多等待1秒
        for (int i = 0; i < 10; i++)
        {
            rect = GetSelectedExplorerItemRect(explorer);

            if (rect != System.Windows.Rect.Empty)
                break;

            Thread.Sleep(100);
        }

        if (rect == System.Windows.Rect.Empty)
        {

            return;
        }

        int startX =
            (int)(rect.Left + rect.Width / 2);

        int startY =
            (int)(rect.Top + rect.Height / 2);

        int endX =
            saiRect.Left + 700;

        int endY =
            saiRect.Top + 500;

        MouseHelper.Move(startX, startY);

        Thread.Sleep(30);

        MouseHelper.LeftDown();

        Thread.Sleep(50);

        // 拖一半
        MouseHelper.Move(
            endX,
            endY - 100);

        Thread.Sleep(20);

        // ★这里激活SAI★
        Activate(sai);

        Thread.Sleep(80);

        // 再移动一点
        MouseHelper.Move(
            endX,
            endY);

        Thread.Sleep(20);

        MouseHelper.LeftUp();

        Wait(100);

        PressEnter();
    }



}
