using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sudoku
{
    class ImageProccessor
    {
        private Area icon;

        public ImageProccessor()
        {
        }

        public Bitmap GetScreenshot()
        {
            Graphics gr;
            Bitmap bmp = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height,
                PixelFormat.Format24bppRgb);
            gr = Graphics.FromImage(bmp);
            gr.CopyFromScreen(Screen.PrimaryScreen.Bounds.X, Screen.PrimaryScreen.Bounds.Y,
                0, 0, Screen.PrimaryScreen.Bounds.Size, CopyPixelOperation.SourceCopy);
            return bmp;
        }

        public Bitmap SetBlack(Bitmap bmp)
        {
            // Задаём формат Пикселя.
            PixelFormat pxf = PixelFormat.Format24bppRgb;

            // Получаем данные картинки.
            Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
            //Блокируем набор данных изображения в памяти
            BitmapData bmpData = bmp.LockBits(rect, ImageLockMode.ReadWrite, pxf);

            // Получаем адрес первой линии.
            IntPtr ptr = bmpData.Scan0;

            // Задаём массив из Byte и помещаем в него надор данных.
             int numBytes = bmp.Width * bmp.Height * 3; 
            //На 3 умножаем - поскольку RGB цвет кодируется 3-мя байтами
            //Либо используем вместо Width - Stride
            //int numBytes = bmpData.Stride * bmp.Height;
            //int widthBytes = bmpData.Stride;
            byte[] rgbValues = new byte[numBytes];

            // Копируем значения в массив.
            Marshal.Copy(ptr, rgbValues, 0, numBytes);

            int i = 0;
            int j = 0;
            // Перебираем пикселы по 3 байта на каждый и меняем значения
            for (int counter = 0; counter < rgbValues.Length; counter += 3)
            {
                byte color_b = 0;
                byte isBlack = 0;
                if (rgbValues[counter] <= 255 && rgbValues[counter + 1] <= 130 && rgbValues[counter + 2] <= 130)
                {
                    color_b = 0;
                    isBlack = 1;
                }
                else
                {
                    color_b = 255;
                    isBlack = 0;
                }
                rgbValues[counter] = color_b;
                rgbValues[counter + 1] = color_b;
                rgbValues[counter + 2] = color_b;
                if (i == bmp.Width - 1)
                {
                    i = 0;
                    j++;
                }
                else
                    i++;

            }
            // Копируем набор данных обратно в изображение
            Marshal.Copy(rgbValues, 0, ptr, numBytes);

            // Разблокируем набор данных изображения в памяти.
            bmp.UnlockBits(bmpData);
            bmp.Save("D:\\GreyImg.bmp", ImageFormat.Bmp);
            return bmp;
        }

        public int[,] GetMap(Bitmap bmp)
        {
            DateTime st = DateTime.Now;
            int[,] map = new int[bmp.Width, bmp.Height];
            // Задаём формат Пикселя.
            PixelFormat pxf = PixelFormat.Format24bppRgb;

            // Получаем данные картинки.
            Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
            //Блокируем набор данных изображения в памяти
            BitmapData bmpData = bmp.LockBits(rect, ImageLockMode.ReadWrite, pxf);

            // Получаем адрес первой линии.
            IntPtr ptr = bmpData.Scan0;

            // Задаём массив из Byte и помещаем в него надор данных.
             int numBytes = bmp.Width * bmp.Height * 3; 
            //На 3 умножаем - поскольку RGB цвет кодируется 3-мя байтами
            //Либо используем вместо Width - Stride
            //int numBytes = bmpData.Stride * bmp.Height;
            //int widthBytes = bmpData.Stride;
            byte[] rgbValues = new byte[numBytes];

            // Копируем значения в массив.
            Marshal.Copy(ptr, rgbValues, 0, numBytes);

            int i = 0;
            int j = 0;
            byte curPixel = 0;
            // Перебираем пикселы по 3 байта на каждый и меняем значения
            for (int counter = 0; counter < rgbValues.Length; counter += 3)
            {
                if (rgbValues[counter] == 255)
                    curPixel = 0;
                else
                    curPixel = 1;
                map[i, j] = curPixel;
                if (i == bmp.Width - 1)
                {
                    i = 0;
                    j++;
                }
                else
                    i++;
            }
            // Копируем набор данных обратно в изображение
            Marshal.Copy(rgbValues, 0, ptr, numBytes);

            // Разблокируем набор данных изображения в памяти.
            bmp.UnlockBits(bmpData);
            CreateBmp(map, "QQ1");
            return map;
        }
        /*if (i == bmp.Width - 1)
                {
                    i = 0;
                    j++;
                }
                else
                    i++;*/

        public int[,] GetSumMap(int[,] map)
        {
            int[,] sumMap = new int[map.GetLength(0), map.GetLength(1)];
            for (int i = 0; i < map.GetLength(0); i++ )
            {
                for (int j = 0; j < map.GetLength(1); j++)
                {
                    if (i - 1 < 0 && j - 1 < 0)
                        sumMap[i, j] = map[i,j];
                    else if (i - 1 < 0)
                        sumMap[i, j] = sumMap[i, j - 1] + map[i, j];
                    else if (j - 1 < 0)
                        sumMap[i, j] = sumMap[i - 1, j] + map[i, j];
                    else
                        sumMap[i, j] = sumMap[i - 1, j] + sumMap[i, j - 1] + map[i, j] - sumMap[i - 1, j - 1];
                }
            }
            return sumMap;
        }

        public Area FindGameArea(int[,] map, Bitmap bmp, int ac, int[,] m)
        {
            int sum = 0;
            int r = 0;
            Area area = new Area();
            AreaInfo areaInfo = GetBmpInfo(bmp);
            for (int i = areaInfo.Width + 1; i < map.GetLength(0); i++)
            {
                for (int j = areaInfo.Width + 1; j < map.GetLength(1); j++)
                {
                    sum = map[i, j] - map[i - areaInfo.Width - 1, j] - map[i, j - areaInfo.Height - 1] + map[i - areaInfo.Width - 1, j - areaInfo.Height - 1];
                    if (sum == areaInfo.Sum && i >= areaInfo.Width && j >= areaInfo.Height)
                    {
                        area.X = i - areaInfo.Width + 1;
                        area.Y = j - areaInfo.Height + 1;
                        if (checkArea(area, map, areaInfo, ac))
                        {
                            Area a = new Area();
                            a.X = area.X + 2;
                            a.Y = area.Y + 45;
                            a.SizeX = 538;
                            a.SizeY = 538;
                            CreateBmp(m, "AA" + r, a.X, a.Y);
                            return a;
                        }
                    }   
                }
            }
            return default(Area);
        }

        public int[,] FindDigits(int[,] map, Area area)
        {
            int[,] arr = new int[9, 9];
            Bitmap bmp;
            int sum = 0;
            AreaInfo info;
            for (int k = 1; k < 10; k++)
            {
                bmp = new Bitmap(k + ".bmp");
                info = GetBmpInfo(bmp);
                for (int i = area.X; i < area.X + area.SizeX; i += 60)
                {
                    for (int j = area.Y; j < area.Y + area.SizeY; j += 60)
                    {
                        sum = map[i + info.Width - 1, j + info.Height - 1] - map[i + info.Width - 1, j] - map[i, j + info.Height - 1] + map[i, j];
                        if (sum == info.Sum)
                        {
                            if (k == 4 || k == 5)
                            {
                                Area a = new Area();
                                a.X = i;
                                a.Y = j;
                                a.SizeX = info.Width;
                                a.SizeY = info.Height;
                                if (checkArea(a, map, info, 28))
                                    arr[(i - area.X) / 60, (j - area.Y) / 60] = k;
                                continue;
                            }
                            arr[(i - area.X) / 60, (j - area.Y) / 60] = k;
                        }
                    }
                }
            }
            int[,] dest = new int[9, 9];
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    dest[i, j] = arr[j, i];
                }
            }
            return dest;
        }

        private AreaInfo GetBmpInfo(Bitmap bmp)
        {
            AreaInfo ai = new AreaInfo();
            ai.Height = bmp.Height;
            ai.Width = bmp.Width;
            int[,] map = GetMap(SetBlack(bmp));
            CreateBmp(map, "QQ");
            int[,] summap = GetSumMap(map);
            ai.SumMap = summap;
            ai.Sum = summap[bmp.Width - 1, bmp.Height - 1];
            return ai;
        }

        private bool checkArea(Area area, int[,] map, AreaInfo areaInfo, int ac)
        {
            int si = ac - 1;
            int sj = ac - 1;
            int sum = 0;
            for (int i = area.X - 1 + ac; i < area.X + areaInfo.Width; i += ac)
            {
                for (int j = area.Y - 1 + ac; j < area.Y + areaInfo.Height; j += ac)
                {
                    sum = map[i, j] - map[i - ac, j] - map[i, j - ac] + map[i - ac, j - ac];
                    int asum = 0;
                    //asum = areaInfo.getSum(si, sj, ac);
                    if (si - ac < 0 && sj - ac < 0)
                    {
                        asum = areaInfo.SumMap[si, sj];
                    }
                    else if (si - ac < 0)
                    {
                        asum = areaInfo.SumMap[si, sj] - areaInfo.SumMap[si, sj - ac];
                    }
                    else if (sj - ac < 0)
                    {
                        asum = areaInfo.SumMap[si, sj] - areaInfo.SumMap[si - ac, sj];
                    }
                    else
                        asum = areaInfo.SumMap[si, sj] - areaInfo.SumMap[si - ac, sj] - areaInfo.SumMap[si, sj - ac] + areaInfo.SumMap[si - ac, sj - ac];
                    
                    if (sum != asum)
                        return false;
                    sj += ac;
                }
                si += ac;
                sj = ac - 1;
            }
            return true;
        }

        public void CreateBmp(int[,] map, string name, int ri = -1, int rj = -1)
        {
            Bitmap bmp = new Bitmap(map.GetLength(0), map.GetLength(1));
            Color color = Color.Black;

            for (int i = 0; i < map.GetLength(0); i++)
            {
                for (int j = 0; j < map.GetLength(1); j++)
                {
                    if (map[i, j] == 1)
                        color = Color.Black;
                    else if (map[i, j] == 0)
                        color = Color.White;
                    //color = Color.FromArgb(map[i, j], map[i, j], map[i, j]);
                    bmp.SetPixel(i, j, color);
                }
            }
            if (ri != -1 && rj != -1)
                bmp.SetPixel(ri, rj, Color.Red);
            bmp.Save("D:\\" + name + ".bmp", ImageFormat.Bmp);
        }

      
    }
}
