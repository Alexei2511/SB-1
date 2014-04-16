using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sudoku
{
    class ImageProccessor
    {
        private List<Area> areas;
        private Area icon;

        public ImageProccessor()
        {
            areas = new List<Area>();
        }

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
            // int numBytes = bmp.Width * bmp.Height * 3; 
            //На 3 умножаем - поскольку RGB цвет кодируется 3-мя байтами
            //Либо используем вместо Width - Stride
            int numBytes = bmpData.Stride * bmp.Height;
            int widthBytes = bmpData.Stride;
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
            bmp.Save("G:\\GreyImg.bmp", ImageFormat.Bmp);
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
            // int numBytes = bmp.Width * bmp.Height * 3; 
            //На 3 умножаем - поскольку RGB цвет кодируется 3-мя байтами
            //Либо используем вместо Width - Stride
            int numBytes = bmpData.Stride * bmp.Height;
            int widthBytes = bmpData.Stride;
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

        public bool FindGameArea(int[,] map, Bitmap bmp, int ac, int[,] m)
        {
            int sum = 0;
            int r = 0;
            int index = 0;
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
                            CreateBmp(m, "AA" + r, area.X, area.Y);
                            areas.Add(area);
                        }
                    }   
                }
            }
            return false;
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
                    asum = areaInfo.getSum(si, sj, ac);
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
            bmp.Save("G:\\" + name + ".bmp", ImageFormat.Bmp);
        }

      
    }
}
