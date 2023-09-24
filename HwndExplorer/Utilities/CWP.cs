using System;

namespace HwndExplorer.Utilities
{
    [Flags]
    public enum CWP
    {
        CWP_ALL = 0,
        CWP_SKIPINVISIBLE = 1,
        CWP_SKIPDISABLED = 2,
        CWP_SKIPTRANSPARENT = 4,
    }
}
