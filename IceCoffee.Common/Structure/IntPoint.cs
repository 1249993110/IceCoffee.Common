namespace IceCoffee.Common.Structure
{
    /// <summary>
    /// 整数点
    /// </summary>
    public struct IntPoint
    {
        public int X;
        public int Y;

        public IntPoint(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }

        public IntPoint(double x, double y)
        {
            this.X = (int)x;
            this.Y = (int)y;
        }

        /// <summary>
        /// 隐式转换
        /// </summary>
        /// <param name="point"></param>
        public static implicit operator IntPoint(System.Drawing.Point point)
        {
            return new IntPoint(point.X, point.Y);
        }
    }
}