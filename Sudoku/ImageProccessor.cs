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
        private int[,] sumMap;
        private int[,] map;
        private List<Area> areas;
        private int width;
        private int height;
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
            width = bmp.Width;
            height = bmp.Height;
            sumMap = new int[bmp.Width, bmp.Height];
            map = new int[bmp.Width, bmp.Height];
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
            for (int counter = 0; counter < rgbValues.Length - 3; counter += 3)
            {
                byte color_b = 0;
                byte isBlack = 0;
                if (rgbValues[counter] <= 255 && rgbValues[counter + 1] <= 100 && rgbValues[counter + 2] <= 100)
                {
                    color_b = 0;
                    isBlack = 1;
                }
                else
                {
                    color_b = 255;
                    isBlack = 0;
                }
                //int value = rgbValues[counter] + rgbValues[counter + 1] + rgbValues[counter + 2];

                //color_b = Convert.ToByte(value / 3);
                
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
            bmp.Save("E:\\GreyImg.bmp", ImageFormat.Bmp);
            return bmp;
        }

        public int[,] GetSumMap(Bitmap bmp)
        {

            DateTime st = DateTime.Now;
            width = bmp.Width;
            height = bmp.Height;
            sumMap = new int[bmp.Width, bmp.Height];
            map = new int[bmp.Width, bmp.Height];
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
            for (int counter = 0; counter < rgbValues.Length - 3; counter += 3)
            {
                if (rgbValues[counter] == 255)
                    curPixel = 0;
                else
                    curPixel = 1;
                if (i - 1 < 0 && j - 1 < 0)
                    sumMap[i, j] = curPixel;
                else if (i - 1 < 0)
                    sumMap[i, j] = sumMap[i, j - 1] + curPixel;
                else if (j - 1 < 0)
                    sumMap[i, j] = sumMap[i - 1, j] + curPixel;
                else
                    sumMap[i, j] = sumMap[i - 1, j] + sumMap[i, j - 1] + curPixel - sumMap[i - 1, j - 1];
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
            CreateBmp(0, 0, "Result");
            return sumMap;
        }

        public bool FindGameArea(int[,] map)
        {
            Area area = new Area();
            int sum = 0;
            int index = 0;
            Area subArea = new Area();
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    if (i - 291 < 0 && j - 19 < 0)
                    {
                        sum = map[i, j];
                    }
                    else if (i - 291 < 0)
                    {
                        sum = map[i, j] - map[i, j - 19];
                    }
                    else if (j - 291 < 0)
                    {
                        sum = map[i, j] - map[i - 291, j];
                    }
                    else
                        sum = map[i, j] - map[i - 291, j] - map[i, j - 19] + map[i - 291, j - 19];
                    if (sum == 739 && i >= 291 && j >= 19)
                    {
                        area.X = i - 290;
                        area.Y = j - 18;
                        area.SizeX = 291;
                        area.SizeY = 19;
                        areas.Add(area);
                        subArea = area;
                        subArea.SizeX = 91;
                        if (checkArea(subArea, 149))
                        {
                            this.map[area.X, area.Y] = 2;
                            CreateBmp(0, 0, "AA");
                            this.map[area.X, area.Y] = 0;
                        }
                        index++;
                    }
                }
            }
            return false;
        }

        private bool checkArea(Area area, int result)
        {
            int sum = 0;
            int s = 0;
            for (int i = area.X; i < area.X + area.SizeX; i++)
            {
                for (int j = area.Y; j < area.Y + area.SizeY; j++)
                {
                    sum += map[i, j];
                }
            }
            if (sum != result)
                return false;
            return true;
        }

        private int checkSubArea(int x, int y, int size)
        {
            int sum = 0;
            for (int i = x; i < x + size; i++)
            {
                for (int j = y; j < y + size; j++)
                {
                    sum += map[i, j];
                }
            }
            return sum;
        }

        public void CreateBmp(int x, int y, string name)
        {
            Bitmap bmp = new Bitmap(width, height);
            Color color = Color.Black;
            
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < bmp.Height; j++)
                {
                    if (map[i, j] == 1)
                        color = Color.Black;
                    else if (map[i, j] == 0)
                        color = Color.White;
                    else
                        color = Color.Red;

                    bmp.SetPixel(i, j, color);
                }
            }
            bmp.SetPixel(x, y, Color.Red);
            bmp.Save("E:\\" + name + ".bmp", ImageFormat.Bmp);
        }

        public void/*List<Area>*/ Proccess(Bitmap bmp)
        {
            Color pixel;
            areas = new List<Area>();
            sumMap = new int[bmp.Width, bmp.Height];
            int curPixel = 0;
            Area area = new Area();
            int sum = 0;
            for (int i = 0; i < bmp.Width; i++)
            {
                for (int j = 0; j < bmp.Height; j++)
                {
                    bmp.SetPixel(2, 100, Color.Green);
                    pixel = bmp.GetPixel(i, j);
                    if (pixel.B <= 255 && pixel.G <= 100 && pixel.R <= 100)
                    {
                        bmp.SetPixel(i, j, Color.Black);
                        curPixel = 1;
                    }
                    else
                    {
                        bmp.SetPixel(i, j, Color.White);
                        curPixel = 0;
                    }
                    if (i - 1 < 0 && j - 1 < 0)
                        sumMap[i, j] = curPixel;
                    else if (i - 1 < 0)
                        sumMap[i, j] = sumMap[i, j - 1] + curPixel;
                    else if (j - 1 < 0)
                        sumMap[i, j] = sumMap[i - 1, j] + curPixel;
                    else
                        sumMap[i, j] = sumMap[i - 1, j] + sumMap[i, j - 1] + curPixel - sumMap[i - 1, j - 1];
                    if (i - 16 < 0 && j - 16 < 0)
                    {
                        sum = sumMap[i, j];
                    }
                    else if (i - 16 < 0)
                    {
                        sum = sumMap[i, j] - sumMap[i, j - 16];
                    }
                    else if (j - 16 < 0)
                    {
                        sum = sumMap[i, j] - sumMap[i - 16, 16];
                    }
                    else
                        sum = sumMap[i, j] - sumMap[i - 16, j] - sumMap[i, j - 16] + sumMap[i - 16, j - 16];
                    if (sum >= 135 && sum <= 139)
                    {
                        area.X = i - 15;
                        area.Y = j - 15;
                        bmp.SetPixel(i - 15, j - 15, Color.Red);
                        area.SizeX = 16;
                        areas.Add(area);

                    }
                }
            }
            //getAreas(bmp.Width, bmp.Height);
            bmp.Save("E:\\procImg.bmp", ImageFormat.Bmp);
            return;
        }

        private double[] getArea(Area area)
        {
            double[] mas = new double[256];
            int k = 0;
            for (int i = area.X; i < area.SizeX; i++)
            {
                for (int j = area.Y; j < area.SizeX; j++)
                {
                    mas[k] = sumMap[i, j];
                    k++;
                }
            }
            return mas;
        }
    }
}
