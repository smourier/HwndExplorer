using System;

namespace HwndExplorer.Utilities
{
    [Flags]
    public enum WPF
    {
        WPF_SETMINPOSITION = 0x1,
        WPF_RESTORETOMAXIMIZED = 0x2,
        WPF_ASYNCWINDOWPLACEMENT = 0x4,
    }
}
