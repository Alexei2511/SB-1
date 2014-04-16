namespace Sudoku
{
    class AreaInfo
    {
        public int Width;
        public int Height;
        public int Sum;
        public int[,] SumMap;


        public int getSum(int x, int y, int ac)
        {
            if (x - ac < 0 || y - ac < 0) 
                return 0;

            return SumMap[x, y] - SumMap[x - ac, y] -
                SumMap[x, y - ac] + SumMap[x - ac, y - ac];
        }

    }
}
