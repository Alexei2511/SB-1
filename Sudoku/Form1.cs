using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
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
            Stopwatch t1 = new Stopwatch();
            t1.Start();
            ip = new ImageProccessor();
            ip.GetScreenshot().Save("E:\\Original.bmp", ImageFormat.Bmp);
            ip.FindGameArea(ip.GetSumMap(ip.SetBlack(ip.GetScreenshot())));
            //ip.CreateBmp(0, 0);
            //if (ip.FindIcon())
              //  MessageBox.Show("Find");
            //ip.FindIcon();
            //ip.CreateBmp(0, 0, 0);
            //    MessageBox.Show("Find.");
            /*Bitmap bmp = ip.GetScreenshot();*/
            t1.Stop();
            MessageBox.Show(t1.ElapsedMilliseconds.ToString());
            /*Stopwatch t1 = new Stopwatch();
            t1.Start();
            ip.Proccess(bmp);
            bmp.Save("Screen.bmp", ImageFormat.Bmp);
            t1.Stop();
            MessageBox.Show(t1.ElapsedMilliseconds.ToString());*/
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
    }
}
