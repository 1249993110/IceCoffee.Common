using IceCoffee.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IceCoffee.Common.GIS
{
    /// <summary>
    /// 坐标转换
    /// </summary>
    public static class CoordinateTransTool
    {
        // 椭球参数
        private const double pi = 3.14159265358979324;
        private const double a = 6378245.0;
        private const double ee = 0.00669342162296594323;

        /// <summary>
        /// WGS84坐标转高德坐标
        /// </summary>
        /// <param name="in_lng"></param>
        /// <param name="wg_lat"></param>
        /// <param name="out_lng"></param>
        /// <param name="out_lat"></param>
        public static void WGS84_to_GCJ02(double in_lng, double wg_lat, out double out_lng, out double out_lat)
        {
            if (OutOfChina(wg_lat, in_lng))
            {
                out_lng = in_lng;
                out_lat = wg_lat;
                return;
            }
            double dLat = TransformLat(in_lng - 105.0, wg_lat - 35.0);
            double dLng = TransformLng(in_lng - 105.0, wg_lat - 35.0);
            double radLat = wg_lat / 180.0 * pi;
            double magic = Math.Sin(radLat);
            magic = 1 - ee * magic * magic;
            double sqrtMagic = Math.Sqrt(magic);
            dLat = (dLat * 180.0) / ((a * (1 - ee)) / (magic * sqrtMagic) * pi);
            dLng = (dLng * 180.0) / (a / sqrtMagic * Math.Cos(radLat) * pi);
            out_lng = in_lng + dLng;
            out_lat = wg_lat + dLat;
        }

        /// <summary>
        /// WGS84坐标转高德坐标
        /// </summary>
        /// <param name="lat"></param>
        /// <param name="lng"></param>
        public static double[] WGS84_to_GCJ02(double lng, double lat)
        {
            double[] lnglat = new double[2];
            WGS84_to_GCJ02(lng, lat, out lnglat[0], out lnglat[1]);
            return lnglat;
        }

        /// <summary>
        /// WGS84坐标转高德坐标
        /// </summary>
        public static double[] WGS84_to_GCJ02(double[] lnglat)
        {
            return WGS84_to_GCJ02(lnglat[0], lnglat[1]);
        }

        /// <summary>
        /// WGS84坐标转高德坐标
        /// </summary>
        public static string[] WGS84_to_GCJ02(string[] _lnglat)
        {
            return WGS84_to_GCJ02(_lnglat[0], _lnglat[1]);
        }

        /// <summary>
        /// WGS84坐标转高德坐标
        /// </summary>
        public static string[] WGS84_to_GCJ02(string lng, string lat)
        {
            double _lng, _lat;
            WGS84_to_GCJ02(lng.ToDouble(), lat.ToDouble(), out _lng, out _lat);
            return new string[] { _lng.ToString("f6"), _lat.ToString("f6") };
        }

        /// <summary>
        /// WGS84坐标转高德坐标
        /// </summary>
        public static void WGS84_to_GCJ02(string in_lng, string in_lat, out string out_lng, out string out_lat)
        {
            double _lng, _lat;
            WGS84_to_GCJ02(in_lng.ToDouble(), in_lat.ToDouble(), out _lng, out _lat);
            out_lng = _lng.ToString("f6");
            out_lat = _lat.ToString("f6");
        }

        /// <summary>
        /// 坐标是否在中国境内
        /// </summary>
        /// <param name="lat"></param>
        /// <param name="lng"></param>
        /// <returns></returns>
        public static bool OutOfChina(double lat, double lng)
        {
            if (lng < 72.004 || lng > 137.8347)
                return true;
            if (lat < 0.8293 || lat > 55.8271)
                return true;
            return false;
        }

        private static double TransformLat(double x, double y)
        {
            double ret = -100.0 + 2.0 * x + 3.0 * y + 0.2 * y * y + 0.1 * x * y + 0.2 * Math.Sqrt(Math.Abs(x));
            ret += (20.0 * Math.Sin(6.0 * x * pi) + 20.0 * Math.Sin(2.0 * x * pi)) * 2.0 / 3.0;
            ret += (20.0 * Math.Sin(y * pi) + 40.0 * Math.Sin(y / 3.0 * pi)) * 2.0 / 3.0;
            ret += (160.0 * Math.Sin(y / 12.0 * pi) + 320 * Math.Sin(y * pi / 30.0)) * 2.0 / 3.0;
            return ret;
        }

        private static double TransformLng(double x, double y)
        {
            double ret = 300.0 + x + 2.0 * y + 0.1 * x * x + 0.1 * x * y + 0.1 * Math.Sqrt(Math.Abs(x));
            ret += (20.0 * Math.Sin(6.0 * x * pi) + 20.0 * Math.Sin(2.0 * x * pi)) * 2.0 / 3.0;
            ret += (20.0 * Math.Sin(x * pi) + 40.0 * Math.Sin(x / 3.0 * pi)) * 2.0 / 3.0;
            ret += (150.0 * Math.Sin(x / 12.0 * pi) + 300.0 * Math.Sin(x / 30.0 * pi)) * 2.0 / 3.0;
            return ret;
        }
    }
}
