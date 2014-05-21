using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sudoku
{
    class GameMaker
    {

        //C++ TO C# CONVERTER NOTE: Overloaded method(s) are created above to convert the following method having default parameters:
        //ORIGINAL LINE: int testSudoku(int *mas0, int c = 3, bool debug = false, bool write_in_file = true, int max = 0, string fn="")
        public int[] testSudoku(int[] mas0, int c, int max)
        {
            int c2 = c * c, c3 = c * c * c, c4 = c * c * c * c;
            int i, j, k, l, mod, v, col, row, box;
            int[] mas = new int[c4];
            int[] test = new int[3 * c4];
            bool b;
            // заполняем таблицу
            for (i = 0; i < c4; i++)
                mas[i] = ((mas0[i] > 0) || (mas0[i] <= c2)) ? mas0[i] : 0;
            // проверка на валидность таблицы
            for (i = 0; i < 3 * c4; i++)
                test[i] = 1;
            for (i = 0; i < c4; i++)
            {
                v = mas[i] - 1;
                if (v >= 0)
                {
                    col = i % c2;
                    row = (int)(i / c2);
                    box = (int)(col / c) + (int)(row / c) * c;
                    test[col * c2 + v]--;
                    test[c4 + row * c2 + v]--;
                    test[2 * c4 + box * c2 + v]--;
                }
            }
            // если таблица невалидна вернуть -1
            for (i = 0; i < 3 * c4; i++)
                if (test[i] < 0)
                    return null;
            // заполняем таблицу неизменными базовыми числами и значениями по умолчанию
            for (i = 0; i < c4; i++)
                mas[i] = (mas0[i] != 0 && (mas0[i] >= -c2) && (mas0[i] <= c2)) ? mas0[i] : -c2;
            // итеративный обход полей таблицы
            // обход происходит в прямом порядке позиций 0,1,2,3,4,...,80
            //                и в прямом порядке чисел -9,-8,-7,-6,-5,...,-1
            for (i = 0, k = 0, mod = 0; i >= 0; )
            {
                if (i == c4)
                {
                    return mas;
                }
                else if (mas[i] > 0)
                {
                    // если текущий элемент на i-ой позиции - неизменный
                    if (mod != 0)
                        i--;
                    else
                        i++;
                }
                else
                {
                    // найти новый подходящий элемент на i-ой позиции
                    b = false;
                    col = i % c2;
                    row = (int)(i / c2);
                    box = (int)(col / c) + (int)(row / c) * c;
                    v = Math.Abs(mas[i]);
                    if (mod != 0)
                    {
                        // если переход в i-ую позицию произошел из i+1-ой, сменить значение элемента на i-ой позиции
                        v--;
                        // сделать прошлое значение доступным для следующего выбора
                        test[col * c2 + v] = test[c4 + row * c2 + v] = test[2 * c4 + box * c2 + v] = 1;
                    }
                    if (v != 0)
                        do
                        {
                            v--;
                            // проверка удовлетворяет ли искомый элемент условия выбора
                            if (test[col * c2 + v] != 0)
                                if (test[c4 + row * c2 + v] != 0)
                                    if (test[2 * c4 + box * c2 + v] != 0)
                                        b = true;
                        } while (!b && (v > 0));
                    if (b)
                    {
                        // удовлетворяет, перейти на i+1-ую позицию
                        test[col * c2 + v] = test[c4 + row * c2 + v] = test[2 * c4 + box * c2 + v] = 0;
                        mas[i] = -v - 1;
                        mod = 0;
                        i++;
                    }
                    else
                    {
                        // не удовлетворяет, откат на i-1-ую позицию
                        mod = 1;
                        mas[i] = -c2;
                        i--;
                    }
                }
            }
            // вернуть количество результатов
            return mas;
        }

        public void Draw(Area area, int[] mas)
        {
            Input input = new Input();
            int k = 0;
            int finishX = 0;
            int finishY = 0;
            int count = 0;
            int[,] arr = new int[9, 9];
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    arr[i, j] = mas[k];
                    k++;
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
            for (int i = 0; i < 9; i++)
            {
                finishX = area.X + (i * 60 + 30);
                for (int j = 0; j < 9; j++)
                {
                    if (dest[i, j] < 0)
                    {
                        finishY = area.Y + (j * 60 + 30);
                        input.MouseMove(Cursor.Position.X, Cursor.Position.Y, finishX, finishY, 10);
                        count = -dest[i, j];
                        for (int l = 0; l < 10 - count; l++)
                        {
                            input.MouseLeftClick(Cursor.Position.X, Cursor.Position.Y);
                        }
                    }
                    k++;
                }
            }
        }
    }
}
