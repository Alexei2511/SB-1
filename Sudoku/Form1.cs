using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sudoku
{
    public partial class Form1 : Form
    {
        ImageProccessor ip;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        private void button1_Click(object sender, EventArgs e)
        {
            richTextBox1.Clear();
            Stopwatch t1 = new Stopwatch();
            t1.Start();
            ip = new ImageProccessor();
            Bitmap bmp = new Bitmap("Icon.bmp");
            int[,] map = ip.GetMap(ip.SetBlack(ip.GetScreenshot()));
            int[,] sumMap = ip.GetSumMap(map);
            Area gameArea = ip.FindGameArea(sumMap, bmp, 4, map);
            int[,] arr = ip.FindDigits(sumMap, gameArea);
            int[] dest = new int[81];
            int k = 0;
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    dest[k] = arr[i, j];
                    k++;
                }
            }
            GameMaker gm = new GameMaker();
            int[] a = gm.testSudoku(dest, 3, 1);
            k = 0;
            t1.Stop();
            MessageBox.Show(t1.ElapsedMilliseconds.ToString());
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    richTextBox1.AppendText(a[k] + " ");
                    k++;
                }
                richTextBox1.AppendText("\n");
            }
            gm.Draw(gameArea, a);
        }

        private void drawRect(ref Bitmap bmp, int x, int y, int size)
        {
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    if (i == 0 || i == size - 1)
                    {
                        bmp.SetPixel(x + i, y + j, Color.Red);
                    }
                    if (j == 0 || j == size - 1)
                    {
                        bmp.SetPixel(x + i, y + j, Color.Red);
                    }
                }
            }
            bmp.SetPixel(x, y, Color.Blue);
            bmp.SetPixel(x + size - 1, y + size - 1, Color.Blue);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ip = new ImageProccessor();
            ip.GetScreenshot().Save(textBox1.Text, ImageFormat.Bmp);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Input i = new Input();
            i.MouseMove(Cursor.Position.X, Cursor.Position.Y, Cursor.Position.X + 100, Cursor.Position.Y + 100, 10);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            ip = new ImageProccessor();
            ip.SetBlack(ip.GetScreenshot()).Save(textBox2.Text, ImageFormat.Bmp);
        }
    }
}
