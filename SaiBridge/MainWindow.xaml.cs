using Microsoft.Win32;
using SaiBridge.Automation;
using SaiBridge.Models;
using SaiBridge.Services;
using SaiBridge.Views;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace SaiBridge;

public partial class MainWindow : System.Windows.Window
{
    private Settings settings = new();

    public MainWindow()
    {
        InitializeComponent();


        UiAutomationHelper.Logger =
            msg =>
            {
                LogBox.AppendText(msg);
                LogBox.ScrollToEnd();
            };


        LoadSettings();
    }
    private void LoadSettings()
    {
        settings = SettingsService.Load();

        SaiPath.Text = settings.SaiPath;
        CspPath.Text = settings.CspPath;
        PsdCachePath.Text = settings.PsdCachePath;
        WorkspacePath.Text = settings.WorkspacePath;
    }
    protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
    {
        settings.SaiPath = SaiPath.Text;
        settings.CspPath = CspPath.Text;
        settings.PsdCachePath = PsdCachePath.Text;
        settings.WorkspacePath = WorkspacePath.Text;

        SettingsService.Save(settings);

        base.OnClosing(e);
    }

    // ==========================================================
    // SAI
    // ==========================================================

    private void BrowseSai_Click(object sender, RoutedEventArgs e)
    {
        OpenFileDialog dialog = new();

        dialog.Title = "请选择 SAI2";
        dialog.Filter = "可执行程序 (*.exe)|*.exe";

        if (dialog.ShowDialog() == true)
        {
            SaiPath.Text = dialog.FileName;

            settings.SaiPath = dialog.FileName;
            SettingsService.Save(settings);

            LogBox.AppendText($"已选择 SAI2：{dialog.FileName}\n");
            LogBox.ScrollToEnd();

            StatusText.Text = "状态：SAI2 已配置";
        }
    }

    private void OpenSaiFolder_Click(object sender, RoutedEventArgs e)
    {
        if (File.Exists(SaiPath.Text))
        {
            Process.Start("explorer.exe", $"/select,\"{SaiPath.Text}\"");
        }
    }

    // ==========================================================
    // CSP
    // ==========================================================

    private void BrowseCsp_Click(object sender, RoutedEventArgs e)
    {
        OpenFileDialog dialog = new();

        dialog.Title = "请选择 Clip Studio Paint";
        dialog.Filter = "可执行程序 (*.exe)|*.exe";

        if (dialog.ShowDialog() == true)
        {
            CspPath.Text = dialog.FileName;

            settings.CspPath = dialog.FileName;
            SettingsService.Save(settings);

            LogBox.AppendText($"已选择 CSP：{dialog.FileName}\n");
            LogBox.ScrollToEnd();

            StatusText.Text = "状态：CSP 已配置";
        }
    }

    private void OpenCspFolder_Click(object sender, RoutedEventArgs e)
    {
        if (File.Exists(CspPath.Text))
        {
            Process.Start("explorer.exe", $"/select,\"{CspPath.Text}\"");
        }
    }

    // ==========================================================
    // PSD缓存
    // ==========================================================

    private void BrowsePsd_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new OpenFolderDialog();

        dialog.Title = "选择 PSD 缓存目录";

        if (dialog.ShowDialog() == true)
        {
            PsdCachePath.Text = dialog.FolderName;

            settings.PsdCachePath = dialog.FolderName;
            SettingsService.Save(settings);

            LogBox.AppendText($"PSD缓存目录：{dialog.FolderName}\n");
            LogBox.ScrollToEnd();

            StatusText.Text = "状态：PSD缓存目录已配置";
        }
    }

    private void OpenPsdCache_Click(object sender, RoutedEventArgs e)
    {
        if (Directory.Exists(PsdCachePath.Text))
        {
            Process.Start("explorer.exe", PsdCachePath.Text);
        }
    }

    // ==========================================================
    // 工程目录
    // ==========================================================

    private void BrowseWorkspace_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new OpenFolderDialog();

        dialog.Title = "选择默认 SAI 工程目录";

        if (dialog.ShowDialog() == true)
        {
            WorkspacePath.Text = dialog.FolderName;

            settings.WorkspacePath = dialog.FolderName;
            SettingsService.Save(settings);

            LogBox.AppendText($"工程目录：{dialog.FolderName}\n");
            LogBox.ScrollToEnd();

            StatusText.Text = "状态：工程目录已配置";
        }
    }

    private void OpenWorkspace_Click(object sender, RoutedEventArgs e)
    {
        if (Directory.Exists(WorkspacePath.Text))
        {
            Process.Start("explorer.exe", WorkspacePath.Text);
        }
    }

    // ==========================================================
    // 主功能（暂时占位）
    // ==========================================================

    private void SendToCsp_Click(object sender, RoutedEventArgs e)
    {
        Process? sai = SaiAutomation.FindSaiProcess(SaiPath.Text);

        if (sai == null)
        {
            LogBox.AppendText("× 未找到 SAI2\n");
            LogBox.ScrollToEnd();
            return;
        }

        // 激活 SAI
        SaiAutomation.Activate(sai);

        SaiAutomation.Wait(120);

        // 打开另存为
        SaiAutomation.PressCtrlShiftS();

        SaiAutomation.Wait(300);

        // 查找另存为窗口
        IntPtr dialog =
            SaiAutomation.FindSaveDialog(sai);

        if (dialog == IntPtr.Zero)
        {
            LogBox.AppendText("× 没有找到保存窗口\n");
            LogBox.ScrollToEnd();
            return;
        }



        // 获取窗口坐标
        var rect = MouseHelper.GetRect(dialog);

        LogBox.AppendText(
            $"√ 保存窗口：Left={rect.Left} Top={rect.Top} Right={rect.Right} Bottom={rect.Bottom}\n");



        // 扫描控件


        LogBox.AppendText(
            "√ 控件扫描完成\n");

        LogBox.ScrollToEnd();



        // 测试路径节点
        SaiAutomation.TestAutomationPath(dialog);

        LogBox.AppendText(
            "√ UIAutomation测试完成\n");

        LogBox.ScrollToEnd();



        // 自动填写路径
        SaiAutomation.SetPath(
            dialog,
            WorkspacePath.Text);


        SaiAutomation.Wait(120);



        SaiAutomation.Wait(120);

        SaiAutomation.PressEnter();

        SaiAutomation.Wait(300);


        // 处理覆盖确认
        SaiAutomation.ConfirmOverwrite();

        LogBox.AppendText(
            "√ 自动填写完成\n");

        LogBox.ScrollToEnd();

        // 导出PSD
        SaiAutomation.ExportPsdWorkflow(
            sai,
            PsdCachePath.Text);

        string? newestPsd =
    FileService.GetNewestPsd(
        PsdCachePath.Text);

        if (newestPsd == null)
        {
            LogBox.AppendText(
                "× 没找到PSD\n");

            return;
        }

        LogBox.AppendText(
     $"√ 最新PSD：{newestPsd}\n");

        LogBox.ScrollToEnd();



        if (newestPsd == null)
        {
            LogBox.AppendText("× 没找到PSD\n");
            return;
        }
        //=========================
        // 启动CSP
        //=========================

        Process? csp =
            SaiAutomation.FindSaiProcess(
                CspPath.Text);

        if (csp == null)
        {
            csp = Process.Start(CspPath.Text);

            Thread.Sleep(5000);

            csp =
                SaiAutomation.FindSaiProcess(
                    CspPath.Text);
        }

        if (csp == null)
        {
            LogBox.AppendText(
                "× CSP启动失败\n");

            return;
        }

        SaiAutomation.Activate(csp);

        SaiAutomation.Wait(250);

        // 打开文件
        SaiAutomation.PressCtrlO();

        SaiAutomation.Wait(250);

        IntPtr openDialog =
            SaiAutomation.FindForegroundWindow();

        SaiAutomation.OpenFileInCsp(
            openDialog,
            newestPsd);

    }






    // ==========================================================
    // 返回 SAI
    // ==========================================================

    private void ReturnToSai_Click(object sender, RoutedEventArgs e)
    {
        //=========================
        // 找到最新PSD
        //=========================

        string? newestPsd =
            FileService.GetNewestPsd(
                PsdCachePath.Text);

        if (newestPsd == null)
        {
            LogBox.AppendText("× 没找到PSD\n");
            return;
        }

        //=========================
        // 保存CSP
        //=========================

        Process? csp =
            SaiAutomation.FindSaiProcess(
                CspPath.Text);

        if (csp == null)
        {
            LogBox.AppendText("× CSP未启动\n");
            return;
        }

        SaiAutomation.Activate(csp);

        SaiAutomation.Wait(80);

        SaiAutomation.PressCtrlS();

        SaiAutomation.Wait(300);

        //=========================
        // 打开SAI
        //=========================

        Process? sai =
            SaiAutomation.FindSaiProcess(
                SaiPath.Text);

        if (sai == null)
        {
            LogBox.AppendText("× 未找到SAI\n");
            return;
        }



        //=========================
        // Explorer定位PSD
        //=========================

        SaiAutomation.OpenExplorerSelect(
     newestPsd);

        SaiAutomation.Wait(250);

        SaiAutomation.DragSelectedFileToSai(
    sai);

        // 如果拖完以后需要把 SAI 放到前面，再激活
        SaiAutomation.Activate(sai);

        SaiAutomation.Wait(300);

    }

    private void Settings_Click(
    object sender,
    RoutedEventArgs e)
    {
        ThemeService.ToggleTheme();

        StatusText.Text =
            ThemeService.IsDark
                ? "状态：已切换到深色模式"
                : "状态：已切换到浅色模式";
    }


    private void Minimize_Click(object sender, RoutedEventArgs e)
    {
        WindowState = WindowState.Minimized;
    }

    private void Maximize_Click(object sender, RoutedEventArgs e)
    {
        if (WindowState == WindowState.Maximized)
        {
            WindowState = WindowState.Normal;
        }
        else
        {
            WindowState = WindowState.Maximized;
        }
    }

    private void Close_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }

    private void TitleBar_MouseLeftButtonDown(
     object sender,
     MouseButtonEventArgs e)
    {
        // 双击标题栏
        if (e.ClickCount == 2)
        {
            if (ResizeMode != ResizeMode.NoResize)
            {
                WindowState =
                    WindowState == WindowState.Maximized
                    ? WindowState.Normal
                    : WindowState.Maximized;
            }

            return;
        }

        // 左键拖动
        if (e.ButtonState == MouseButtonState.Pressed)
        {
            DragMove();
        }
    }



}