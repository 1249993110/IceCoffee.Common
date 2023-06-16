namespace IceCoffee.Common
{
    /// <summary>
    /// Excel Functions
    /// </summary>
    public static class ExcelFunctions
    {
        /// <summary>
        /// 排位分数
        /// </summary>
        /// <param name="array">已排序一维数组</param>
        /// <param name="value">搜索的值</param>
        /// <returns></returns>
        /// <exception cref="Exception">如果找不到value</exception>
        public static double PercentRank(double[] array, double value)
        {
            if (value < array[0])
            {
                return 0.0;
            }
#if NET6_0_OR_GREATER
            if (value >= array[^1])
            {
                return 1.0;
            }
#else
            if (value >= array[array.Length - 1])
            {
                return 1.0;
            }
#endif
            int i = Array.BinarySearch(array, value);
            if (i >= 0)
            {
                int num = i;
                while (num > 0 && array[num - 1] == array[num])
                {
                    num--;
                }

                return (double)num / (double)(array.Length - 1);
            }

            throw new Exception($"The specified element: {value} is not included in the collection!");
        }
    }
}