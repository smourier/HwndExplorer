using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace HwndExplorer.Utilities
{
    [StructLayout(LayoutKind.Sequential)]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public struct WindowPlacement
    {
        [Browsable(false)]
        public int Length { get; set; }
        public WPF Flags { get; set; }
        public SW ShowCmd { get; set; }
        public int MinPositionX { get; set; }
        public int MinPositionY { get; set; }
        public int MaxPositionX { get; set; }
        public int MaxPositionY { get; set; }
        public int NormalPositionLeft { get; set; }
        public int NormalPositionTop { get; set; }
        public int NormalPositionRight { get; set; }
        public int NormalPositionBottom { get; set; }

        public bool IsMinimized => ShowCmd == SW.SW_SHOWMINIMIZED;
        public bool IsValid => Length == Marshal.SizeOf(typeof(WindowPlacement));

        public override string ToString()
        {
            var str = $"ShowCmd: {ShowCmd}";
            if (Flags != 0)
            {
                str += $" Flags {Flags}";
            }

            if (MinPositionX != -1 && MinPositionY != -1)
            {
                str += $" Min: {MinPositionX}x{MinPositionY}";
            }

            if (MaxPositionX != -1 && MaxPositionY != -1)
            {
                str += $" Max: {MaxPositionX}x{MaxPositionY}";
            }

            if (NormalPositionLeft != 0 && NormalPositionTop != 0 && NormalPositionRight != 0 && NormalPositionBottom != 0)
            {
                str += $" Normal: {NormalPositionLeft},{NormalPositionTop},{NormalPositionRight},{NormalPositionBottom}";
            }
            return str;
        }

        public void SetPlacement(IntPtr handle) => SetWindowPlacement(handle, ref this);
        public static WindowPlacement GetPlacement(IntPtr handle, bool throwOnError = false)
        {
            var wp = new WindowPlacement();
            if (handle == IntPtr.Zero)
                return wp;

            wp.Length = Marshal.SizeOf(typeof(WindowPlacement));
            if (!GetWindowPlacement(handle, ref wp))
            {
                if (throwOnError)
                    throw new Win32Exception(Marshal.GetLastWin32Error());

                return new WindowPlacement();
            }
            return wp;
        }

        [DllImport("user32", SetLastError = true)]
        private static extern bool SetWindowPlacement(IntPtr hWnd, ref WindowPlacement lpwndpl);

        [DllImport("user32", SetLastError = true)]
        private static extern bool GetWindowPlacement(IntPtr hWnd, ref WindowPlacement lpwndpl);
    }
}
