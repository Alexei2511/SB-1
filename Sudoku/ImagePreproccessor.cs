using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sudoku
{
    class ImagePreproccessor
    {
        private int[,] map;
        private List<Area> areas;

        public Bitmap GetScreenshot()
        {
            Graphics gr;
            Bitmap bmp = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height,
                PixelFormat.Format32bppRgb);
            gr = Graphics.FromImage(bmp);
            gr.CopyFromScreen(Screen.PrimaryScreen.Bounds.X, Screen.PrimaryScreen.Bounds.Y,
                0, 0, Screen.PrimaryScreen.Bounds.Size, CopyPixelOperation.SourceCopy);
            return bmp;
        }

        public List<Area> Proccess(Bitmap bmp)
        {
            Color pixel;
            areas = new List<Area>();
            map = new int[bmp.Width, bmp.Height];
            int curPixel = 0;
            Area area;
            int sum = 0;
            for (int i = 0; i < bmp.Width; i++)
            {
                for (int j = 0; j < bmp.Height; j++)
                {
                    pixel = bmp.GetPixel(i, j);
                    if (pixel.B <= 255 && pixel.G <= 100 && pixel.R <= 100)
                    {
                        //bmp.SetPixel(i, j, Color.Black);
                        curPixel = 1;
                    }
                    else
                    {
                        //bmp.SetPixel(i, j, Color.White);
                        curPixel = 0;
                    }
                    if (i - 1 < 0 && j - 1 < 0)
                        map[i, j] = curPixel;
                    else if (i - 1 < 0)
                        map[i, j] = map[i, j - 1] + curPixel;
                    else if (j - 1 < 0)
                        map[i, j] = map[i - 1, j] + curPixel;
                    else
                        map[i, j] = map[i - 1, j] + map[i, j - 1] + curPixel - map[i - 1, j - 1];
                    if (i - 16 < 0 && j - 16 < 0)
                    {
                        sum = map[i, j];
                    }
                    else if (i - 16 < 0)
                    {
                        sum = map[i, j] - map[i, j - 16];
                    }
                    else if (j - 16 < 0)
                    {
                        sum = map[i, j] - map[i - 16, 16];
                    }
                    else
                        sum = map[i, j] - map[i - 16, j] - map[i, j - 16] + map[i - 16, j - 16];
                    if (sum >= 135 && sum <= 139)
                    {
                        area.X = i - 15;
                        area.Y = j - 15;
                        bmp.SetPixel(i - 15, j - 15, Color.Red);
                        area.Size = 16;
                        areas.Add(area);
                    }
                }
            }
            //getAreas(bmp.Width, bmp.Height);
            return areas;
        }

        private double[] getArea(Area area)
        {
            double[] mas = new double[256];
            int k = 0;
            for (int i = area.X; i < area.Size; i++)
            {
                for (int j = area.Y; j < area.Size; j++)
                {
                    mas[k] = map[i, j];
                    k++;
                }
            }
            return mas;
        }
    }
}
