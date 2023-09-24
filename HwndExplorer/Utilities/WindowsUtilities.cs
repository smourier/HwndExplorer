using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32;

namespace HwndExplorer.Utilities
{
    public static class WindowsUtilities
    {
        private static readonly Lazy<Process?> _currentProcess = new(Process.GetCurrentProcess, true);
        public static Process? CurrentProcess => _currentProcess.Value;

        private static readonly Lazy<Process?> _parentProcess = new(GetParentProcess, true);
        public static Process? ParentProcess => _parentProcess.Value; // can be null

        public static void OpenFile(string fileName)
        {
            if (fileName == null)
                throw new ArgumentNullException(nameof(fileName));

            var psi = new ProcessStartInfo
            {
                FileName = fileName,
                UseShellExecute = true
            };
            Process.Start(psi);
        }

        public static void OpenUrl(Uri uri) => OpenUrl(uri?.ToString());
        public static void OpenUrl(string? url)
        {
            if (url == null)
                return;

            Process.Start(url);
        }

        public static void OpenExplorer(string directoryPath)
        {
            if (directoryPath == null)
                return;

            if (!IOUtilities.PathIsDirectory(directoryPath))
                return;

            // see http://support.microsoft.com/kb/152457/en-us
            Process.Start("explorer.exe", "/e,/root,/select," + directoryPath);
        }

        public static RegistryKey EnsureSubKey(this RegistryKey root, string name)
        {
            if (root == null)
                throw new ArgumentNullException(nameof(root));

            if (name == null)
                throw new ArgumentNullException(nameof(name));

            var key = root.OpenSubKey(name, true);
            if (key != null)
                return key;

            var parentName = Path.GetDirectoryName(name);
            if (string.IsNullOrEmpty(parentName))
                return root.CreateSubKey(name);

            using (var parentKey = root.EnsureSubKey(parentName))
            {
                return parentKey.CreateSubKey(Path.GetFileName(name));
            }
        }

        public static void CenterWindow(IntPtr handle) => CenterWindow(handle, IntPtr.Zero);
        public static void CenterWindow(IntPtr handle, IntPtr alternateOwner)
        {
            if (handle == IntPtr.Zero)
                throw new ArgumentException(null, nameof(handle));

            // determine owner window to center against
            var dwStyle = (WS)(long)GetWindowLong(handle, WL.GWL_STYLE);
            var hWndCenter = alternateOwner;
            if (alternateOwner == IntPtr.Zero)
            {
                if (dwStyle.HasFlag(WS.WS_CHILD))
                {
                    hWndCenter = GetParent(handle);
                }
                else
                {
                    hWndCenter = GetWindow(handle, GW.GW_OWNER);
                }
            }

            // get coordinates of the window relative to its parent
            var rcDlg = new RECT();
            _ = GetWindowRect(handle, ref rcDlg);
            var rcArea = new RECT();
            var rcCenter = new RECT();
            if (!dwStyle.HasFlag(WS.WS_CHILD))
            {
                // don't center against invisible or minimized windows
                if (hWndCenter != IntPtr.Zero)
                {
                    var dwAlternateStyle = (WS)(long)GetWindowLong(hWndCenter, WL.GWL_STYLE);
                    if (!dwAlternateStyle.HasFlag(WS.WS_VISIBLE) || dwAlternateStyle.HasFlag(WS.WS_MINIMIZE))
                    {
                        hWndCenter = IntPtr.Zero;
                    }
                }

                var mi = new MONITORINFO
                {
                    cbSize = Marshal.SizeOf(typeof(MONITORINFO))
                };

                // center within appropriate monitor coordinates
                if (hWndCenter == IntPtr.Zero)
                {
                    IntPtr hwDefault = GetActiveWindow();
                    _ = GetMonitorInfo(MonitorFromWindow(hwDefault, MFW.MONITOR_DEFAULTTOPRIMARY), ref mi);
                    rcCenter = mi.rcWork;
                    rcArea = mi.rcWork;
                }
                else
                {
                    _ = GetWindowRect(hWndCenter, ref rcCenter);
                    _ = GetMonitorInfo(MonitorFromWindow(hWndCenter, MFW.MONITOR_DEFAULTTONEAREST), ref mi);
                    rcArea = mi.rcWork;
                }
            }
            else
            {
                // center within parent client coordinates
                IntPtr hWndParent = GetParent(handle);
                _ = GetClientRect(hWndParent, ref rcArea);
                _ = GetClientRect(hWndCenter, ref rcCenter);
                _ = MapWindowPoints(hWndCenter, hWndParent, ref rcCenter, 2);
            }

            // find dialog's upper left based on rcCenter
            var xLeft = (rcCenter.left + rcCenter.right) / 2 - rcDlg.Width / 2;
            var yTop = (rcCenter.top + rcCenter.bottom) / 2 - rcDlg.Height / 2;

            // if the dialog is outside the screen, move it inside
            if (xLeft + rcDlg.Width > rcArea.right)
            {
                xLeft = rcArea.right - rcDlg.Width;
            }

            if (xLeft < rcArea.left)
            {
                xLeft = rcArea.left;
            }

            if (yTop + rcDlg.Height > rcArea.bottom)
            {
                yTop = rcArea.bottom - rcDlg.Height;
            }

            if (yTop < rcArea.top)
            {
                yTop = rcArea.top;
            }

            // map screen coordinates to child coordinates
            _ = SetWindowPos(handle, IntPtr.Zero, xLeft, yTop, -1, -1, SWP.SWP_NOSIZE | SWP.SWP_NOZORDER | SWP.SWP_NOACTIVATE);
        }

        public static IntPtr ActivateModalWindow(IntPtr hwnd) => ActivateWindow(GetModalWindow(hwnd));
        public static IntPtr ActivateWindow(IntPtr hwnd) => ModalWindowUtil.ActivateWindow(hwnd);
        public static IntPtr GetModalWindow(IntPtr owner) => ModalWindowUtil.GetModalWindow(owner);

        private sealed class ModalWindowUtil
        {
            private int _maxOwnershipLevel;
            private IntPtr _maxOwnershipHandle;

            private bool EnumChildren(IntPtr hwnd, IntPtr lParam)
            {
                var level = 1;
                if (IsWindowVisible(hwnd) && IsOwned(lParam, hwnd, ref level) && level > _maxOwnershipLevel)
                {
                    _maxOwnershipHandle = hwnd;
                    _maxOwnershipLevel = level;
                }
                return true;
            }

            private static bool IsOwned(IntPtr owner, IntPtr hwnd, ref int level)
            {
                var ownerHandle = GetWindow(hwnd, GW.GW_OWNER);
                if (ownerHandle == IntPtr.Zero)
                    return false;

                if (ownerHandle == owner)
                    return true;

                level++;
                return IsOwned(owner, ownerHandle, ref level);
            }

            public static IntPtr ActivateWindow(IntPtr hwnd)
            {
                if (hwnd == IntPtr.Zero)
                    return IntPtr.Zero;

                return SetActiveWindow(hwnd);
            }

            public static IntPtr GetModalWindow(IntPtr owner)
            {
                var util = new ModalWindowUtil();
                _ = EnumThreadWindows(GetCurrentThreadId(), util.EnumChildren, owner);
                return util._maxOwnershipHandle; // may be IntPtr.Zero
            }
        }

        public static RECT GetWindowRect(IntPtr handle)
        {
            var rc = new RECT();
            _ = GetWindowRect(handle, ref rc);
            return rc;
        }

        public static RECT GetClientRect(IntPtr handle)
        {
            var rc = new RECT();
            _ = GetClientRect(handle, ref rc);
            return rc;
        }

        public static IntPtr GetWindowLong(IntPtr handle, WL index)
        {
            // https://devblogs.microsoft.com/oldnewthing/20180906-00/?p=99665
            var unicode = IsWindowUnicode(handle);

            if (IntPtr.Size == 8)
                return unicode ? GetWindowLongPtrW(handle, index) : GetWindowLongPtrA(handle, index);

            return unicode ? GetWindowLongW(handle, index) : GetWindowLongA(handle, index);
        }

        public static Point ClientToScreen(IntPtr hwnd, Point point)
        {
            var pt = point;
            _ = ClientToScreen(hwnd, ref pt);
            return pt;
        }

        public static RECT ClientToScreen(IntPtr hwnd, RECT rc)
        {
            var lt = rc.LeftTop;
            _ = ClientToScreen(hwnd, ref lt);

            var rb = rc.RightBottom;
            _ = ClientToScreen(hwnd, ref rb);
            return new RECT(lt.X, lt.Y, rb.X, rb.Y);
        }

        public static RECT GetMonitorBounds(IntPtr handle)
        {
            var mi = new MONITORINFO
            {
                cbSize = Marshal.SizeOf(typeof(MONITORINFO))
            };

            _ = GetMonitorInfo(MonitorFromWindow(handle, MFW.MONITOR_DEFAULTTOPRIMARY), ref mi);
            return mi.rcMonitor;
        }

        public static IntPtr GetParentWindow(IntPtr handle) => GetAncestor(handle, GA.GA_PARENT);
        public static IntPtr GetRootWindow(IntPtr handle) => GetAncestor(handle, GA.GA_ROOT);
        public static IntPtr GetRootOwnerWindow(IntPtr handle) => GetAncestor(handle, GA.GA_ROOTOWNER);
        public static IntPtr GetOwnerWindow(IntPtr handle) => GetWindow(handle, GW.GW_OWNER);

        public static void ReplaceWindowText(IntPtr handle, string text, bool canBeUndone = true)
        {
            var len = GetWindowTextLengthW(handle);
            _ = SendMessage(handle, EM_SETSEL, len, len);
            _ = SendMessage(handle, EM_REPLACESEL, canBeUndone ? 1 : 0, text ?? string.Empty);
            len = GetWindowTextLengthW(handle);
            if (len > 0)
            {
                _ = SendMessage(handle, EM_SETSEL, len - 1, len);
            }
        }

        public static string GetWindowText(IntPtr handle)
        {
            var len = GetWindowTextLengthW(handle);
            var sb = new StringBuilder(len + 2);
            _ = GetWindowText(handle, sb, sb.Capacity);
            return sb.ToString();
        }

        public static IntPtr ChildWindowFromPoint(IntPtr handle, Point point, CWP flags = CWP.CWP_ALL) => ChildWindowFromPointEx(handle, point, flags);
        public static bool IsParentWindow(IntPtr parent, IntPtr child) => EnumerateParentWindows(child).Any(p => p == parent);
        public static IEnumerable<IntPtr> EnumerateParentWindows(IntPtr handle)
        {
            if (handle == IntPtr.Zero)
                yield break;

            var current = handle;
            do
            {
                var parent = GetParentWindow(current);
                if (parent == IntPtr.Zero)
                    yield break;

                yield return parent;
                current = parent;
            }
            while (true);
        }

        private enum GA
        {
            GA_PARENT = 1,
            GA_ROOT = 2,
            GA_ROOTOWNER = 3,
        }

        private enum GW
        {
            GW_HWNDFIRST = 0,
            GW_HWNDLAST = 1,
            GW_HWNDNEXT = 2,
            GW_HWNDPREV = 3,
            GW_OWNER = 4,
            GW_CHILD = 5,
            GW_ENABLEDPOPUP = 6,
        }

        private const int QS_ALLINPUT = 0x00001CFF;
        private const int EM_SETSEL = 0xB1;
        private const int EM_REPLACESEL = 0xC2;

        [StructLayout(LayoutKind.Sequential)]
        private struct GUITHREADINFO
        {
            public int cbSize;
            public int flags;
            public IntPtr hwndActive;
            public IntPtr hwndFocus;
            public IntPtr hwndCapture;
            public IntPtr hwndMenuOwner;
            public IntPtr hwndMoveSize;
            public IntPtr hwndCaret;
            public int left;
            public int top;
            public int right;
            public int bottom;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MEMORYSTATUSEX
        {
            public int dwLength;
            public int dwMemoryLoad;
            public ulong ullTotalPhys;
            public ulong ullAvailPhys;
            public ulong ullTotalPageFile;
            public ulong ullAvailPageFile;
            public ulong ullTotalVirtual;
            public ulong ullAvailVirtual;
            public ulong ullAvailExtendedVirtual;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct PROCESS_BASIC_INFORMATION
        {
            public IntPtr Reserved1;
            public IntPtr PebBaseAddress;
            public IntPtr Reserved2_0;
            public IntPtr Reserved2_1;
            public IntPtr UniqueProcessId;
            public IntPtr InheritedFromUniqueProcessId;
        }

        private delegate bool PropEnumProcEx(IntPtr arg1, IntPtr arg2, IntPtr arg3, IntPtr arg4);
        private delegate bool EnumThreadWndProc(IntPtr hwnd, IntPtr lParam);
        private delegate bool EnumChildProc(IntPtr handle, IntPtr lParam);
        private delegate bool EnumWindowsProc(IntPtr handle, IntPtr lParam);

        public static DialogResult MessageBoxTimeout(this IWin32Window hwnd, string text, string caption, int buttons, int milliseconds) => MessageBoxTimeout((hwnd?.Handle).GetValueOrDefault(), text, caption, buttons, milliseconds);
        public static DialogResult MessageBoxTimeout(IntPtr hwnd, string text, string caption, int buttons, int milliseconds)
        {
            if (hwnd == IntPtr.Zero)
            {
                hwnd = GetForegroundWindow();
            }
            return MessageBoxTimeout(hwnd, text, caption, buttons, 0, milliseconds);
        }

        public static int GetQueueCount() => GetQueueStatus(QS_ALLINPUT);

        public static int GetMemoryLoadPercent()
        {
            var status = new MEMORYSTATUSEX
            {
                dwLength = Marshal.SizeOf<MEMORYSTATUSEX>()
            };
            _ = GlobalMemoryStatusEx(ref status);
            return status.dwMemoryLoad;
        }

        public static Process? GetParentProcess() => GetParentProcess((CurrentProcess?.Handle).GetValueOrDefault());
        public static Process? GetParentProcess(int id)
        {
            try
            {
                var process = Process.GetProcessById(id);
                return GetParentProcess((process?.Handle).GetValueOrDefault());
            }
            catch
            {
                // continue
                return null;
            }
        }

        public static Process? GetParentProcess(IntPtr handle)
        {
            if (handle == IntPtr.Zero)
                return null;

            var pbi = new PROCESS_BASIC_INFORMATION();
            var status = NtQueryInformationProcess(handle, 0, ref pbi, Marshal.SizeOf(pbi), out _);
            if (status == 0)
            {
                try
                {
                    return Process.GetProcessById((int)pbi.InheritedFromUniqueProcessId.ToInt64());
                }
                catch
                {
                    // continue
                }
            }
            return null;
        }

        [DllImport("ntdll.dll")]
        private static extern int NtQueryInformationProcess(IntPtr processHandle, int processInformationClass, ref PROCESS_BASIC_INFORMATION processInformation, int processInformationLength, out int returnLength);

        [DllImport("kernel32", SetLastError = true)]
        private static extern bool GlobalMemoryStatusEx(ref MEMORYSTATUSEX lpBuffer);

        [DllImport("user32", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern DialogResult MessageBoxTimeout(IntPtr hWnd, string lpText, string lpCaption, int uType, short wLanguageId, int dwMilliseconds);

        [DllImport("dwmapi")]
        private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref bool attrValue, int attrSize);

        [DllImport("user32")]
        private static extern bool ScreenToClient(IntPtr hwnd, ref Point pt);

        [DllImport("user32", SetLastError = true)]
        private static extern bool MoveWindow(IntPtr handle, int x, int y, int width, int height, bool repaint);

        [DllImport("user32")]
        private static extern IntPtr ChildWindowFromPointEx(IntPtr handle, Point pt, CWP flags);

        [DllImport("user32")]
        private static extern void SwitchToThisWindow(IntPtr handle, bool useAltCtlTab);

        [DllImport("user32", SetLastError = true)]
        private static extern bool BringWindowToTop(IntPtr handle);

        [DllImport("user32")]
        private static extern IntPtr SetActiveWindow(IntPtr handle);

        [DllImport("user32")]
        public static extern bool SetForegroundWindow(IntPtr handle);

        [DllImport("user32")]
        private static extern bool AllowSetForegroundWindow(int processId);

        [DllImport("kernel32", CharSet = CharSet.Unicode)]
        private static extern int GlobalGetAtomName(ushort nAtom, StringBuilder lpBuffer, int nSize);

        [DllImport("kernel32", CharSet = CharSet.Unicode)]
        private static extern int GetAtomName(ushort nAtom, StringBuilder lpBuffer, int nSize);

        [DllImport("kernel32")]
        private static extern int GetQueueStatus(int flags);

        [DllImport("user32", CharSet = CharSet.Unicode)]
        private static extern bool EnumPropsEx(IntPtr handle, PropEnumProcEx lpEnumFunc, IntPtr lParam);

        [DllImport("user32")]
        private static extern bool LogicalToPhysicalPoint(IntPtr handle, ref Point lpPoint);

        [DllImport("user32")]
        private static extern bool PhysicalToLogicalPoint(IntPtr handle, ref Point lpPoint);

        [DllImport("user32")]
        private static extern int GetGUIThreadInfo(int threadId, ref GUITHREADINFO info);

        [DllImport("user32")]
        private static extern int GetWindowThreadProcessId(IntPtr handle, out int processId);

        [DllImport("user32")]
        private static extern IntPtr GetTopWindow(IntPtr handle);

        [DllImport("user32")]
        private static extern IntPtr GetLastActivePopup(IntPtr handle);

        [DllImport("user32")]
        private static extern IntPtr GetAncestor(IntPtr handle, GA gaFlags);

        [DllImport("user32")]
        private static extern IntPtr GetWindow(IntPtr handle, GW uCmd);

        [DllImport("user32", CharSet = CharSet.Unicode)]
        private static extern int GetWindowText(IntPtr handle, StringBuilder lpString, int nMaxCount);

        [DllImport("user32", CharSet = CharSet.Unicode)]
        private static extern int GetWindowModuleFileName(IntPtr handle, StringBuilder pszFileName, int cchFileNameMax);

        [DllImport("user32", CharSet = CharSet.Unicode)]
        private static extern int GetClassName(IntPtr handle, StringBuilder lpClassName, int nMaxCount);

        [DllImport("user32", CharSet = CharSet.Unicode)]
        private static extern int RealGetWindowClass(IntPtr hwnd, StringBuilder ptszClassName, int cchClassNameMax);

        [DllImport("user32")]
        private static extern int GetWindowTextLengthW(IntPtr handle);

        [DllImport("user32")]
        private static extern bool IsHungAppWindow(IntPtr handle);

        [DllImport("user32")]
        private static extern bool IsWindowVisible(IntPtr handle);

        [DllImport("user32")]
        private static extern bool IsWindowEnabled(IntPtr handle);

        [DllImport("kernel32")]
        private static extern int GetCurrentThreadId();

        [DllImport("user32")]
        private static extern bool EnumThreadWindows(int dwThreadId, EnumThreadWndProc lpEnumFunc, IntPtr lParam);

        [DllImport("user32")]
        private static extern IntPtr GetActiveWindow();

        [DllImport("kernel32")]
        private static extern bool AllocConsole();

        [DllImport("user32")]
        private static extern bool IsWindowUnicode(IntPtr handle);

        [DllImport("user32", EntryPoint = "GetWindowLongPtrW")]
        private static extern IntPtr GetWindowLongPtrW(IntPtr hWnd, WL nIndex);

        [DllImport("user32", EntryPoint = "GetWindowLongPtrA")]
        private static extern IntPtr GetWindowLongPtrA(IntPtr hWnd, WL nIndex);

        [DllImport("user32", EntryPoint = "GetWindowLongW")]
        private static extern int GetWindowLongW(IntPtr hWnd, WL nIndex);

        [DllImport("user32", EntryPoint = "GetWindowLongA")]
        private static extern int GetWindowLongA(IntPtr hWnd, WL nIndex);

        [DllImport("user32", SetLastError = true)]
        private static extern bool PostMessage(IntPtr handle, int msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32")]
        private static extern IntPtr SendMessage(IntPtr handle, int msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32", CharSet = CharSet.Unicode)]
        private static extern IntPtr SendMessage(IntPtr handle, int msg, IntPtr wParam, [MarshalAs(UnmanagedType.LPWStr)] string lParam);

        [DllImport("user32")]
        private static extern int MapWindowPoints(IntPtr hWndFrom, IntPtr hWndTo, ref RECT rect, int cPoints);

        [DllImport("user32")]
        private static extern int MapWindowPoints(IntPtr hWndFrom, IntPtr hWndTo, [In, Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 3)] Point[] lpPoints, int cPoints);

        [DllImport("user32")]
        private static extern bool UpdateWindow(IntPtr handle);

        [DllImport("user32")]
        private static extern IntPtr GetParent(IntPtr hWnd);

        [DllImport("user32", CharSet = CharSet.Unicode)]
        private static extern bool GetMonitorInfo(IntPtr hmonitor, ref MONITORINFO info);

        [DllImport("user32", SetLastError = true)]
        private static extern bool SetWindowPos(IntPtr handle, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, SWP flags);

        [DllImport("user32")]
        private static extern IntPtr MonitorFromWindow(IntPtr handle, MFW flags);

        [DllImport("user32", SetLastError = true)]
        private static extern bool GetWindowRect(IntPtr handle, ref RECT rect);

        [DllImport("user32", SetLastError = true)]
        private static extern bool GetClientRect(IntPtr handle, ref RECT rect);

        [DllImport("user32")]
        private static extern bool ClientToScreen(IntPtr hwnd, ref Point point);

        [DllImport("user32")]
        private static extern IntPtr GetForegroundWindow();
    }
}
