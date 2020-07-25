using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        ////隐式转换
        //public static implicit operator IntPoint(System.Windows.Point point)
        //{
        //    return new IntPoint(point.X, point.Y);
        //}
    }
}
