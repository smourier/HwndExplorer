using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;

namespace HwndExplorer.Utilities
{
    public class Win32Window : IEquatable<Win32Window>
    {
        private static readonly Lazy<Win32Window> _desktop = new(() => new Win32Window(WindowsUtilities.GetDesktopWindow()), true);
        private static readonly Lazy<Win32Window> _shell = new(() => new Win32Window(WindowsUtilities.GetShellWindow()), true);

        public static Win32Window Shell => _shell.Value;
        public static IEnumerable<Win32Window> TopLevelWindows => WindowsUtilities.EnumerateTopLevelWindows().Select(FromHandle).Where(w => w is not null)!;
        public static Win32Window Desktop => _desktop.Value;
        public static Win32Window? Foreground => FromHandle(WindowsUtilities.GetForegroundWindow());
        public static Win32Window? Focus => FromHandle(WindowsUtilities.GetFocus());
        public static Win32Window? Active => FromHandle(WindowsUtilities.GetActiveWindow());
        public static Win32Window? Console => FromHandle(WindowsUtilities.GetConsoleWindow());

        public static Win32Window? FindWindow(string? className = null, string? windowName = null) => FromHandle(WindowsUtilities.FindWindow(className, windowName));
        public static Win32Window? FromPoint(Point point) => FromHandle(WindowsUtilities.WindowFromPoint(point));
        public static Win32Window? FromPhysicalPoint(Point point) => FromHandle(WindowsUtilities.WindowFromPhysicalPoint(point));
        public static IEnumerable<Win32Window> FromProcess(int processId) => WindowsUtilities.EnumerateProcessWindows(processId).Select(h => FromHandle(h)).Where(p => p is not null)!;
        public static IEnumerable<Win32Window> FromProcess(Process process) => WindowsUtilities.EnumerateProcessWindows(process).Select(h => FromHandle(h)).Where(p => p is not null)!;

        public static Win32Window? FromHandle(IntPtr handle)
        {
            if (handle == IntPtr.Zero)
                return null;

            if (!WindowsUtilities.IsWindow(handle))
                return null;

            return new Win32Window(handle);
        }

        private readonly Lazy<Process> _process;

        public Win32Window(IntPtr handle)
        {
            _process = new Lazy<Process>(() => Process.GetProcessById(ProcessId));
            Handle = handle;
        }

        public IntPtr Handle { get; }
        public WS Styles { get => (WS)(long)WindowsUtilities.GetWindowLong(Handle, WL.GWL_STYLE); }

        [DisplayName("Extended Styles")]
        public WS_EX ExtendedStyles { get => (WS_EX)(long)WindowsUtilities.GetWindowLong(Handle, WL.GWL_EXSTYLE); }

        [Browsable(false)]
        public int Int32Handle => (int)(long)Handle;

        [DisplayName("Handle (hex)")]
        public string HandleAsHex => "0x" + Int32Handle.ToString("X8");

        [DisplayName("Is Window")]
        public bool IsWindow => WindowsUtilities.IsWindow(Handle);

        [DisplayName("Is Visible")]
        public bool IsVisible => WindowsUtilities.IsWindowVisible(Handle);

        [DisplayName("Is Unicode")]
        public bool IsUnicode => WindowsUtilities.IsWindowUnicode(Handle);

        [DisplayName("Is Zoomed")]
        public bool IsZoomed => WindowsUtilities.IsZoomed(Handle);

        [DisplayName("Is Iconic")]
        public bool IsIconic => WindowsUtilities.IsIconic(Handle);

        [DisplayName("Is Hung App")]
        public bool IsHungApp => WindowsUtilities.IsHungAppWindow(Handle);

        [DisplayName("Is Active")]
        public bool IsActive => Handle == WindowsUtilities.GetActiveWindow();

        [DisplayName("Is Foreground")]
        public bool IsForeground => Handle == WindowsUtilities.GetForegroundWindow();

        [DisplayName("Has Focus")]
        public bool HasFocus => Handle == WindowsUtilities.GetFocus();

        [DisplayName("Is Top-Level")]
        public bool IsTopLevel => WindowsUtilities.IsTopLevelWindow(Handle);

        [DisplayName("Is Dialog")]
        public bool IsDialog => RealClassName == "#32770";

        [DisplayName("Is Desktop")]
        public bool IsDesktop => RealClassName == "#32769";

        [DisplayName("Is Menu")]
        public bool IsMenu => RealClassName == "#32768";

        [DisplayName("Is Enabled")]
        public bool IsEnabled => WindowsUtilities.IsWindowEnabled(Handle);

        [DisplayName("Class Name")]
        public string ClassName => WindowsUtilities.GetWindowClass(Handle);

        [DisplayName("Real Class Name")]
        public string RealClassName => WindowsUtilities.GetRealWindowClass(Handle);
        public string Text => WindowsUtilities.GetWindowText(Handle);

        [DisplayName("Module File Name")]
        public string ModuleFileName => WindowsUtilities.GetWindowModuleFileName(Handle);
        public WindowPlacement Placement { get => WindowPlacement.GetPlacement(Handle); set => value.SetPlacement(Handle); }
        public Win32Window? Owner => FromHandle(WindowsUtilities.GetOwnerWindow(Handle));

        [DisplayName("Root Owner")]
        public Win32Window? RootOwner => FromHandle(WindowsUtilities.GetRootOwnerWindow(Handle));
        public Win32Window? Root => FromHandle(WindowsUtilities.GetRootWindow(Handle));
        public Win32Window? Top => FromHandle(WindowsUtilities.GetTopWindow(Handle));

        [DisplayName("Last Active Popup")]
        public Win32Window? LastActivePopup => FromHandle(WindowsUtilities.GetLastActivePopup(Handle));
        public IReadOnlyList<WindowProperty> Properties => WindowsUtilities.EnumerateProperties(Handle);

        [DisplayName("Thread Id")]
        public int ThreadId => WindowsUtilities.GetWindowThreadId(Handle);

        [DisplayName("Process Id")]
        public int ProcessId => WindowsUtilities.GetWindowProcessId(Handle);
        public Process Process => _process.Value;
        public RECT Rect => WindowsUtilities.GetWindowRect(Handle);

        [DisplayName("Client Rect")]
        public RECT ClientRect => WindowsUtilities.GetClientRect(Handle);

        [DisplayName("Child Windows")]
        public IEnumerable<Win32Window> ChildWindows => WindowsUtilities.EnumerateChildWindows(Handle).Select(h => FromHandle(h)).Where(w => w is not null)!;

        [Browsable(false)]
        public IEnumerable<Win32Window> AllChildWindows
        {
            get
            {
                foreach (var child in ChildWindows)
                {
                    yield return child;
                    foreach (var gchild in child.AllChildWindows)
                    {
                        yield return gchild;
                    }
                }
            }
        }

        public Win32Window? Parent
        {
            get
            {
                var parent = WindowsUtilities.GetParentWindow(Handle);
                if (parent == Handle)
                    return null;

                return FromHandle(parent);
            }
        }

        [DisplayName("Parent Windows")]
        public IEnumerable<Win32Window> ParentWindows
        {
            get
            {
                var current = Parent;
                do
                {
                    var parent = current;
                    if (parent is null)
                        yield break;

                    yield return parent;
                    if (current is not null && current == parent)
                        break;

                    current = parent;
                }
                while (true);
            }
        }

        [DisplayName("Process Windows")]
        public IReadOnlyCollection<Win32Window> ProcessWindows
        {
            get
            {
                var list = new HashSet<Win32Window>();
                var p = Process;
                if (p != null)
                {
                    foreach (var child in FromProcess(p))
                    {
                        list.Add(child);
                        foreach (var child2 in child.AllChildWindows)
                        {
                            list.Add(child2);
                        }
                    }
                }
                return list;
            }
        }

        public IntPtr GetLong(WL index) => WindowsUtilities.GetWindowLong(Handle, index);
        public bool Show(SW command) => WindowsUtilities.ShowWindow(Handle, command);
        public bool ShowAsync(SW command) => WindowsUtilities.ShowWindowAsync(Handle, command);
        public bool Hide() => Show(SW.SW_HIDE);
        public bool HideAsync() => ShowAsync(SW.SW_HIDE);
        public Point? PhysicalToLogicalPoint(Point point) => WindowsUtilities.PhysicalToLogicalPoint(Handle, point);
        public Point? LogicalToPhysicalPoint(Point point) => WindowsUtilities.LogicalToPhysicalPoint(Handle, point);
        public void SwitchTo(bool useAltCtlTab) => WindowsUtilities.SwitchToThisWindow(Handle, useAltCtlTab);
        public bool BringToTop() => WindowsUtilities.BringWindowToTop(Handle);
        public Win32Window? GetChildWindowFromPoint(Point point, CWP flags = CWP.CWP_ALL) => FromHandle(WindowsUtilities.ChildWindowFromPoint(Handle, point, flags));
        public bool Move(int x, int y, int width, int height, bool repaint = false) => WindowsUtilities.MoveWindow(Handle, x, y, width, height, repaint);
        public bool Move(int x, int y) => SetWindowPos(IntPtr.Zero, x, y, -1, -1, SWP.SWP_NOSIZE | SWP.SWP_NOZORDER | SWP.SWP_NOACTIVATE);
        public bool Update() => WindowsUtilities.UpdateWindow(Handle);
        public void Resize(int width, int height) => SetWindowPos(IntPtr.Zero, 0, 0, width, height, SWP.SWP_NOMOVE | SWP.SWP_NOZORDER | SWP.SWP_NOACTIVATE);
        public Point ScreenToClient(Point point) { WindowsUtilities.ScreenToClient(Handle, ref point); return point; }
        public Point ClientToScreen(Point point) => WindowsUtilities.ClientToScreen(Handle, point);
        public bool SetWindowPos(IntPtr hWndInsertAfter, int x, int y, int cx, int cy, SWP flags) => WindowsUtilities.SetWindowPos(Handle, hWndInsertAfter, x, y, cx, cy, flags);
        public bool SetForeground() => WindowsUtilities.SetForegroundWindow(Handle);
        public IntPtr SetActive() => WindowsUtilities.SetActiveWindow(Handle);
        public IntPtr SetFocus() => WindowsUtilities.SetFocus(Handle);
        public bool IsParentOf(IntPtr child) => WindowsUtilities.IsParentWindow(Handle, child);
        public bool IsChildOf(IntPtr parent) => WindowsUtilities.IsParentWindow(parent, Handle);
        public IntPtr SetParent(IntPtr parentHandle) => WindowsUtilities.SetParent(Handle, parentHandle);

        public IReadOnlyList<Point> MapPoints(IntPtr handleTo, IEnumerable<Point> points)
        {
            var list = new List<Point>();
            if (points != null)
            {
                var pts = points.ToArray();
                if (WindowsUtilities.MapWindowPoints(Handle, handleTo, pts, pts.Length) != 0)
                {
                    list.AddRange(pts);
                }
            }
            return list.AsReadOnly();
        }

        public override string ToString()
        {
            var s = Int32Handle.ToString("X8");
            var cls = ClassName;
            if (!string.IsNullOrWhiteSpace(cls))
            {
                s += " | " + cls;
            }

            var text = Text;
            if (!string.IsNullOrWhiteSpace(text))
            {
                s += " | " + text;
            }
            return s;
        }

        public override int GetHashCode() => Int32Handle;
        public override bool Equals(object? obj) => Equals(obj as Win32Window);
        public bool Equals(Win32Window? other) => other is not null && Handle != IntPtr.Zero && other.Handle == Handle;

        public static bool operator ==(Win32Window item1, Win32Window item2)
        {
            if (item1 is null)
                return item2 is null;

            return item1.Equals(item2);
        }

        public static bool operator !=(Win32Window item1, Win32Window item2)
        {
            if (item1 is null)
                return item2 is not null;

            return !item1.Equals(item2);
        }
    }
}
