using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sudoku
{
    class Input
    {
        private Screen screen;
        private double cenX;
        private double cenY;
        private const uint KEYEVENTF_KEYUP = 0x2;

        [DllImport("user32.dll")]
        public static extern void SetCursorPos(int x, int y);
        [DllImport("User32.dll")]
        static extern void mouse_event(MouseFlags dwFlags, int dx, int dy, int dwData, UIntPtr dwExtraIn);
        [DllImport("User32.dll")]
        static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, int dwExtraInfo);

        public Input()
        {
            screen = Screen.PrimaryScreen;
            cenX = 65535 / screen.Bounds.Width;
            cenY = 65535 / screen.Bounds.Height;
        }

        /// <summary>
        /// Нажатие левой кнопки мышию
        /// </summary>
        /// <param name="x">Координата x</param>
        /// <param name="y">Координата y</param>
        public void MouseLeftClick(int x, int y)
        {
            mouse_event(MouseFlags.Absolute | MouseFlags.LeftDown, x, y, 0, UIntPtr.Zero);
            mouse_event(MouseFlags.Absolute | MouseFlags.LeftUp, x, y, 0, UIntPtr.Zero);
        }

        private void mouseMove(int x, int y)
        {
            mouse_event(/* MouseFlags.Absolute | */ MouseFlags.Move, x, y, 0, UIntPtr.Zero);
        }
       

        /// <summary>
        /// Нажатие клавиши клавиатуры.
        /// </summary>
        /// <param name="key">Код клавиши</param>
        public void KeyPress(byte key)
        {
            keybd_event(key, 0, 0, 0);
            keybd_event(key, 0, KEYEVENTF_KEYUP, 0);
        }

        /// <summary>
        /// Передвижение мыши.
        /// </summary>
        /// <param name="startX">Начальная позиция по x</param>
        /// <param name="startY">Начальная позиция по y</param>
        /// <param name="finishX">Конечная позиция по x</param>
        /// <param name="finishY">Конечная позиция по y</param>
        /// <param name="speed">Миллисекунд на пиксель</param>
        public void MouseMove(int startX, int startY, int finishX, int finishY, int speed)
        {
            double dx = (double)(finishX - startX) / Math.Sqrt((startX - finishX) * (startX - finishX) + (startY - finishY) * (startY - finishY)) * speed;
            double dy = (double)(finishY - startY) / Math.Sqrt((startX - finishX) * (startX - finishX) + (startY - finishY) * (startY - finishY)) * speed;
            double x = startX;
            double y = startY;
            while (true)
            {
                dx = (double)(finishX - Cursor.Position.X) / Math.Sqrt((Cursor.Position.X - finishX) * (Cursor.Position.X - finishX) + (Cursor.Position.Y - finishY) * (Cursor.Position.Y - finishY)) * speed;
                dy = (double)(finishY - Cursor.Position.Y) / Math.Sqrt((Cursor.Position.X - finishX) * (Cursor.Position.X - finishX) + (Cursor.Position.Y - finishY) * (Cursor.Position.Y - finishY)) * speed;
                if (Math.Sqrt((Cursor.Position.X - finishX) * (Cursor.Position.X - finishX) + (Cursor.Position.Y - finishY) * (Cursor.Position.Y - finishY)) - Math.Sqrt(dx * dx + dy * dy) <= Math.Sqrt(dx * dx + dy * dy))
                {
                    mouseMove((int)(dx), (int)(dy));
                    break;
                }
                mouseMove((int)(dx), (int)(dy));
                Thread.Sleep(10);
            }
        }
    }
}