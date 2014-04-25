using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace SqlMetalPlus
{
    class NativeMethods
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SendMessageTimeout(IntPtr hWnd, int Msg,
                                                    int wParam, string lParam,
                                                    int fuFlags, int uTimeout,
                                                    out int lpdwResult);
        public const int HWND_BROADCAST = 0xffff;
        public const int WM_SETTINGCHANGE = 0x001A;
        public const int SMTO_NORMAL = 0x0000;
        public const int SMTO_BLOCK = 0x0001;
        public const int SMTO_ABORTIFHUNG = 0x0002;
        public const int SMTO_NOTIMEOUTIFNOTHUNG = 0x0008;

    }
}
