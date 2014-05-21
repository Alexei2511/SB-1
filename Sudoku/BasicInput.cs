using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Sudoku
{
    class BasicInput
    {
        private const uint KEYEVENTF_KEYUP = 0x2;

        [DllImport("user32.dll")]
        public static extern void SetCursorPos(int x, int y);
        [DllImport("User32.dll")]
        static extern void mouse_event(MouseFlags dwFlags, int dx, int dy, int dwData, UIntPtr dwExtraIn);
        [DllImport("User32.dll")]
        static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, int dwExtraInfo);
        
        public void MouseLeftClick(int x, int y)
        {
            mouse_event(MouseFlags.Absolute | MouseFlags.LeftDown, x, y, 0, UIntPtr.Zero);
            mouse_event(MouseFlags.Absolute | MouseFlags.LeftUp, x, y, 0, UIntPtr.Zero);
        }

        public void MouseMove(int x, int y)
        {
            mouse_event(MouseFlags.Absolute | MouseFlags.Move, x, y, 0, UIntPtr.Zero);
        }

        public void KeyPress(byte key)
        {
            keybd_event(key, 0, 0, 0);
            keybd_event(key, 0, KEYEVENTF_KEYUP, 0);
        }
    }
}
