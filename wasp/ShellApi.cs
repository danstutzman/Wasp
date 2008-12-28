using System;
using System.Text;
using System.Runtime.InteropServices;

namespace Wasp {
    public class ShellApi {

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT {
            public Int32 Left;
            public Int32 Top;
            public Int32 Right;
            public Int32 Bottom;

            public RECT(Int32 left, Int32 right, Int32 top, Int32 bottom) {
                Left = left;
                Top = top;
                Right = right;
                Bottom = bottom;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct APPBARDATA {
            public UInt32 cbSize;
            public IntPtr hWnd;
            public UInt32 uCallbackMessage;
            public UInt32 uEdge;
            public RECT rc;
            public Int32 lParam;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT {
            public Int32 x;
            public Int32 y;
        }

        // Sends an appbar message to the system. 
        [DllImport("shell32.dll")]

        public static extern UInt32 SHAppBarMessage(
            UInt32 dwMessage,	     // Appbar message value to send.
            ref APPBARDATA pData	 // Address of an APPBARDATA structure.
        ); /// The content of the structure depends on the value set in the dwMessage parameter. 

        // The RegisterWindowMessage function defines a new window message that is guaranteed to be unique throughout 
        // the system. The message value can be used when sending or posting messages. 
        [DllImport("user32.dll")]
        public static extern UInt32 RegisterWindowMessage(
            [MarshalAs(UnmanagedType.LPTStr)]
            String lpString
        );	 // Pointer to a null-terminated string that specifies the message to be registered. 

        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(ref POINT lpPoint);
    }
}