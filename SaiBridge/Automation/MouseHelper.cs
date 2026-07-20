using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace SaiBridge.Automation;

public static class MouseHelper
{
    [DllImport("user32.dll")]
    static extern bool SetCursorPos(int X, int Y);

    [DllImport("user32.dll")]
    static extern void mouse_event(
        uint dwFlags,
        uint dx,
        uint dy,
        uint dwData,
        UIntPtr dwExtraInfo);

    [DllImport("user32.dll")]
    static extern bool GetWindowRect(
        IntPtr hWnd,
        out RECT rect);

    private const uint LEFTDOWN = 0x0002;
    private const uint LEFTUP = 0x0004;

    [StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }

    public static RECT GetRect(IntPtr hwnd)
    {
        GetWindowRect(hwnd, out RECT rect);
        return rect;
    }

    public static void Move(int x, int y)
    {
        SetCursorPos(x, y);
    }

    public static void Click(int x, int y)
    {
        SetCursorPos(x, y);

        Thread.Sleep(40);

        mouse_event(LEFTDOWN, 0, 0, 0, UIntPtr.Zero);
        mouse_event(LEFTUP, 0, 0, 0, UIntPtr.Zero);

        Thread.Sleep(40);
    }

    public static void DoubleClick(int x, int y)
    {
        Click(x, y);
        Thread.Sleep(80);
        Click(x, y);
    }

    public static void LeftDown()
    {
        mouse_event(LEFTDOWN, 0, 0, 0, UIntPtr.Zero);
    }

    public static void LeftUp()
    {
        mouse_event(LEFTUP, 0, 0, 0, UIntPtr.Zero);
    }

    public static void Drag(
    int startX,
    int startY,
    int endX,
    int endY)
    {
        Move(startX, startY);

        Thread.Sleep(80);

        LeftDown();

        Thread.Sleep(80);

        Move(endX, endY);

        Thread.Sleep(150);

        LeftUp();

        Thread.Sleep(150);
    }


}