using System;
using System.Runtime.InteropServices;
using Microsoft.UI.Xaml;

namespace SmallSchedulingApp.Helpers
{
    public class SystemTrayHelper : IDisposable
    {
        private const int WM_LBUTTONUP = 0x0202;
        private const int WM_RBUTTONUP = 0x0205;
        private const uint WM_TRAYICON = 0x8000;
        private const uint NIM_ADD = 0x00000000;
        private const uint NIM_DELETE = 0x00000002;
        private const uint NIF_MESSAGE = 0x00000001;
        private const uint NIF_ICON = 0x00000002;
        private const uint NIF_TIP = 0x00000004;

        private IntPtr _windowHandle;
        private readonly MainWindow _mainWindow;
        private NOTIFYICONDATA _notifyIconData;
        private bool _isDisposed;

        public SystemTrayHelper(MainWindow mainWindow)
        {
            _mainWindow = mainWindow;
        }

        public void Initialize()
        {
            _windowHandle = WinRT.Interop.WindowNative.GetWindowHandle(_mainWindow);

            _notifyIconData = new NOTIFYICONDATA
            {
                cbSize = (uint)Marshal.SizeOf(typeof(NOTIFYICONDATA)),
                hWnd = _windowHandle,
                uID = 1,
                uFlags = NIF_ICON | NIF_MESSAGE | NIF_TIP,
                uCallbackMessage = WM_TRAYICON,
                hIcon = LoadIcon(IntPtr.Zero, (IntPtr)32512), // IDI_APPLICATION
                szTip = "Small Scheduling App"
            };

            Shell_NotifyIcon(NIM_ADD, ref _notifyIconData);

            // Subclass the window to receive messages
            _newWndProc = new WndProc(WndProcHandler);
            _oldWndProc = SetWindowLongPtr(_windowHandle, GWLP_WNDPROC, _newWndProc);
        }

        private delegate IntPtr WndProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);
        private WndProc? _newWndProc;
        private IntPtr _oldWndProc;
        private const int GWLP_WNDPROC = -4;

        private IntPtr WndProcHandler(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam)
        {
            if (msg == WM_TRAYICON)
            {
                ProcessTrayIconMessage(lParam);
            }

            return CallWindowProc(_oldWndProc, hWnd, msg, wParam, lParam);
        }

        public void ShowCalendarPopup()
        {
            if (_mainWindow.IsVisible)
            {
                _mainWindow.Hide();
            }
            else
            {
                PositionWindowNearTray();
                _mainWindow.Show();
            }
        }

        private void PositionWindowNearTray()
        {
            // Get screen dimensions
            var displayArea = Microsoft.UI.Windowing.DisplayArea.Primary;
            var workArea = displayArea.WorkArea;

            // Position at bottom-right (typical tray location)
            var appWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(
                Microsoft.UI.Win32Interop.GetWindowIdFromWindow(_windowHandle));

            var windowWidth = 450;
            var windowHeight = 550;

            var position = new Windows.Graphics.PointInt32
            {
                X = workArea.Width - windowWidth - 10,
                Y = workArea.Height - windowHeight - 50
            };

            appWindow.Move(position);
        }

        public void ProcessTrayIconMessage(IntPtr lParam)
        {
            var message = (uint)lParam.ToInt32() & 0xFFFF;

            if (message == WM_LBUTTONUP)
            {
                ShowCalendarPopup();
            }
            else if (message == WM_RBUTTONUP)
            {
                // Right-click could show context menu in future
                ShowCalendarPopup();
            }
        }

        public void Dispose()
        {
            if (_isDisposed) return;

            Shell_NotifyIcon(NIM_DELETE, ref _notifyIconData);
            _isDisposed = true;
            GC.SuppressFinalize(this);
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct NOTIFYICONDATA
        {
            public uint cbSize;
            public IntPtr hWnd;
            public uint uID;
            public uint uFlags;
            public uint uCallbackMessage;
            public IntPtr hIcon;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string szTip;
        }

        [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
        private static extern bool Shell_NotifyIcon(uint dwMessage, ref NOTIFYICONDATA lpData);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr LoadIcon(IntPtr hInstance, IntPtr lpIconName);

        [DllImport("user32.dll", EntryPoint = "SetWindowLongPtrW", SetLastError = true)]
        private static extern IntPtr SetWindowLongPtr(IntPtr hWnd, int nIndex, WndProc dwNewLong);

        [DllImport("user32.dll")]
        private static extern IntPtr CallWindowProc(IntPtr lpPrevWndFunc, IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);
    }
}
