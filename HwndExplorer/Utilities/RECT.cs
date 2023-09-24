using System.Drawing;
using System.Runtime.InteropServices;

namespace HwndExplorer.Utilities
{
    [StructLayout(LayoutKind.Sequential)]
    public partial struct RECT
    {
        public int left;
        public int top;
        public int right;
        public int bottom;

        public RECT(int left, int top, int right, int bottom)
        {
            this.left = left;
            this.top = top;
            this.right = right;
            this.bottom = bottom;
        }

        public readonly int Width => right - left;
        public readonly int Height => bottom - top;
        public readonly Point LeftTop => new(left, top);
        public readonly Point RightBottom => new(right, bottom);

        public override readonly string ToString() => left + "," + top + "," + right + "," + bottom;
    }
}
