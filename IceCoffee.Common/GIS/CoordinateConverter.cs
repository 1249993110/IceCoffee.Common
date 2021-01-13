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
    public static class CoordinateConverter
    {
        // 保留的小数位数
        private const string decimals = "f6";

        // 椭球参数-圆周率
        private const double pi = 3.14159265358979324;

        private const double x_pi = pi * 3000.0 / 180.0;

        // (北京54)椭球长半轴，卫星椭球坐标投影到平面地图坐标系的投影因子
        private const double a = 6378245.0;
        /*
            * Krasovsky 1940 (北京54)椭球长半轴第一偏心率平方
            * 计算方式：
            * 长半轴：
            * a = 6378245.0
            * 扁率：
            * 1/f = 298.3（变量相关计算为：(a-b)/a）
            * 短半轴：
            * b = 6356863.0188 (变量相关计算方法为：b = a * (1 - f))
            * 第一偏心率平方:
            * e2 = (a^2 - b^2) / a^2;
        */
        private const double ee = 0.00669342162296594323;
        // 地球半径
        private const double earthR = 6371004.0;

        /// <summary>
        /// 计算偏差
        /// </summary>
        /// <param name="in_lng"></param>
        /// <param name="in_lat"></param>
        /// <param name="dLng"></param>
        /// <param name="dLat"></param>
        private static void CalculateDev(double in_lng, double in_lat, out double dLng, out double dLat)
        {
            dLat = TransformLat(in_lng - 105.0, in_lat - 35.0);
            dLng = TransformLng(in_lng - 105.0, in_lat - 35.0);

            double radLat = in_lat / 180.0 * pi;
            double magic = Math.Sin(radLat);
            magic = 1 - ee * magic * magic;
            double sqrtMagic = Math.Sqrt(magic);
            dLat = (dLat * 180.0) / ((a * (1 - ee)) / (magic * sqrtMagic) * pi);
            dLng = (dLng * 180.0) / (a / sqrtMagic * Math.Cos(radLat) * pi);
        }

        #region 高德坐标转WGS84坐标
        /// <summary>
        /// 高德坐标转WGS84坐标
        /// </summary>
        /// <param name="in_lng"></param>
        /// <param name="in_lat"></param>
        /// <param name="out_lng"></param>
        /// <param name="out_lat"></param>
        public static void GCJ02_to_WGS84(double in_lng, double in_lat, out double out_lng, out double out_lat)
        {
            if (OutOfChina(in_lat, in_lng))
            {
                out_lng = in_lng;
                out_lat = in_lat;
                return;
            }

            CalculateDev(in_lng, in_lat, out out_lng, out out_lat);
            out_lng = in_lng - out_lng;
            out_lat = in_lat - out_lat;
        }

        /// <summary>
        /// 高德坐标转WGS84坐标
        /// </summary>
        /// <param name="lat"></param>
        /// <param name="lng"></param>
        public static double[] GCJ02_to_WGS84(double lng, double lat)
        {
            double[] lnglat = new double[2];
            GCJ02_to_WGS84(lng, lat, out lnglat[0], out lnglat[1]);
            return lnglat;
        }

        /// <summary>
        /// 高德坐标转WGS84坐标
        /// </summary>
        /// <param name="lnglat"></param>
        /// <returns></returns>
        public static double[] GCJ02_to_WGS84(double[] lnglat)
        {
            return GCJ02_to_WGS84(lnglat[0], lnglat[1]);
        }

        /// <summary>
        /// 高德坐标转WGS84坐标
        /// </summary>
        /// <param name="lng"></param>
        /// <param name="lat"></param>
        /// <returns></returns>
        public static string[] GCJ02_to_WGS84(string lng, string lat)
        {
            double _lng, _lat;
            GCJ02_to_WGS84(lng.ToDouble(), lat.ToDouble(), out _lng, out _lat);
            return new string[] { _lng.ToString(decimals), _lat.ToString(decimals) };
        }

        /// <summary>
        /// 高德坐标转WGS84坐标
        /// </summary>
        /// <param name="_lnglat"></param>
        /// <returns></returns>
        public static string[] GCJ02_to_WGS84(string[] _lnglat)
        {
            return GCJ02_to_WGS84(_lnglat[0], _lnglat[1]);
        }

        /// <summary>
        /// 高德坐标转WGS84坐标
        /// </summary>
        /// <param name="in_lng"></param>
        /// <param name="in_lat"></param>
        /// <param name="out_lng"></param>
        /// <param name="out_lat"></param>
        public static void GCJ02_to_WGS84(string in_lng, string in_lat, out string out_lng, out string out_lat)
        {
            double _lng, _lat;
            GCJ02_to_WGS84(in_lng.ToDouble(), in_lat.ToDouble(), out _lng, out _lat);
            out_lng = _lng.ToString(decimals);
            out_lat = _lat.ToString(decimals);
        }

        /// <summary>
        /// 高德坐标转WGS84坐标 精确(二分极限法)
        /// 默认设置的是精确到小数点后9位，这个值越小，越精确
        /// </summary>
        /// <param name="in_lng"></param>
        /// <param name="in_lat"></param>
        /// <param name="out_lng"></param>
        /// <param name="out_lat"></param>
        /// <param name="threshold"></param>
        public static void GCJ02_to_WGS84_Exact(double in_lng, double in_lat, out double out_lng, out double out_lat, double threshold = 0.000000001)
        {
            double dLng = 0.01, dLat = 0.01;
            double mLng = in_lng - dLng, mLat = in_lat - dLat;
            double pLng = in_lng + dLng, pLat = in_lat + dLat;
            double wgsLng, wgsLat;
            int i = 0;

            while (true)
            {
                wgsLng = (mLng + pLng) / 2;
                wgsLat = (mLat + pLat) / 2;

                GCJ02_to_WGS84(wgsLat, wgsLng, out out_lng, out out_lat);
                dLng = out_lng - in_lng;
                dLat = out_lat - in_lat;

                if ((Math.Abs(dLat) < threshold) && (Math.Abs(dLng) < threshold))
                    break;

                if (dLat > 0) 
                    pLat = wgsLat; 
                else
                    mLat = wgsLat;

                if (dLng > 0)
                    pLng = wgsLng;
                else
                    mLng = wgsLng;

                if (++i > 10000)
                    break;
            }
        }
        #endregion

        #region 高德坐标转百度坐标
        /// <summary>
        /// 高德坐标转百度坐标
        /// </summary>
        /// <param name="in_lng"></param>
        /// <param name="in_lat"></param>
        /// <param name="out_lng"></param>
        /// <param name="out_lat"></param>
        public static void GCJ02_to_BD09(double in_lng, double in_lat, out double out_lng, out double out_lat)
        {
            double x = in_lng, y = in_lat;
            double z = Math.Sqrt(x * x + y * y) + 0.00002 * Math.Sin(y * x_pi);
            double theta = Math.Atan2(y, x) + 0.000003 * Math.Cos(x * x_pi);
            out_lng = z * Math.Cos(theta) + 0.0065;
            out_lat = z * Math.Sin(theta) + 0.006;
        }

        /// <summary>
        /// 高德坐标转百度坐标
        /// </summary>
        /// <param name="lat"></param>
        /// <param name="lng"></param>
        public static double[] GCJ02_to_BD09(double lng, double lat)
        {
            double[] lnglat = new double[2];
            GCJ02_to_BD09(lng, lat, out lnglat[0], out lnglat[1]);
            return lnglat;
        }

        /// <summary>
        /// 高德坐标转百度坐标
        /// </summary>
        /// <param name="lnglat"></param>
        /// <returns></returns>
        public static double[] GCJ02_to_BD09(double[] lnglat)
        {
            return GCJ02_to_BD09(lnglat[0], lnglat[1]);
        }

        /// <summary>
        /// 高德坐标转百度坐标
        /// </summary>
        /// <param name="lng"></param>
        /// <param name="lat"></param>
        /// <returns></returns>
        public static string[] GCJ02_to_BD09(string lng, string lat)
        {
            double _lng, _lat;
            GCJ02_to_BD09(lng.ToDouble(), lat.ToDouble(), out _lng, out _lat);
            return new string[] { _lng.ToString(decimals), _lat.ToString(decimals) };
        }

        /// <summary>
        /// 高德坐标转百度坐标
        /// </summary>
        /// <param name="_lnglat"></param>
        /// <returns></returns>
        public static string[] GCJ02_to_BD09(string[] _lnglat)
        {
            return GCJ02_to_BD09(_lnglat[0], _lnglat[1]);
        }

        /// <summary>
        /// 高德坐标转百度坐标
        /// </summary>
        /// <param name="in_lng"></param>
        /// <param name="in_lat"></param>
        /// <param name="out_lng"></param>
        /// <param name="out_lat"></param>
        public static void GCJ02_to_BD09(string in_lng, string in_lat, out string out_lng, out string out_lat)
        {
            double _lng, _lat;
            GCJ02_to_BD09(in_lng.ToDouble(), in_lat.ToDouble(), out _lng, out _lat);
            out_lng = _lng.ToString(decimals);
            out_lat = _lat.ToString(decimals);
        }

        #endregion

        #region 百度坐标转高德坐标
        /// <summary>
        /// 百度坐标转高德坐标
        /// </summary>
        /// <param name="in_lng"></param>
        /// <param name="in_lat"></param>
        /// <param name="out_lng"></param>
        /// <param name="out_lat"></param>
        public static void BD09_to_GCJ02(double in_lng, double in_lat, out double out_lng, out double out_lat)
        {
            double x = in_lng - 0.0065, y = in_lat - 0.006;
            double z = Math.Sqrt(x * x + y * y) - 0.00002 * Math.Sin(y * x_pi);
            double theta = Math.Atan2(y, x) - 0.000003 * Math.Cos(x * x_pi);
            out_lng = z * Math.Cos(theta);
            out_lat = z * Math.Sin(theta);
        }

        /// <summary>
        /// 百度坐标转高德坐标
        /// </summary>
        /// <param name="lat"></param>
        /// <param name="lng"></param>
        public static double[] BD09_to_GCJ02(double lng, double lat)
        {
            double[] lnglat = new double[2];
            BD09_to_GCJ02(lng, lat, out lnglat[0], out lnglat[1]);
            return lnglat;
        }

        /// <summary>
        /// 百度坐标转高德坐标
        /// </summary>
        /// <param name="lnglat"></param>
        /// <returns></returns>
        public static double[] BD09_to_GCJ02(double[] lnglat)
        {
            return BD09_to_GCJ02(lnglat[0], lnglat[1]);
        }

        /// <summary>
        /// 百度坐标转高德坐标
        /// </summary>
        /// <param name="lng"></param>
        /// <param name="lat"></param>
        /// <returns></returns>
        public static string[] BD09_to_GCJ02(string lng, string lat)
        {
            double _lng, _lat;
            BD09_to_GCJ02(lng.ToDouble(), lat.ToDouble(), out _lng, out _lat);
            return new string[] { _lng.ToString(decimals), _lat.ToString(decimals) };
        }

        /// <summary>
        /// 百度坐标转高德坐标
        /// </summary>
        /// <param name="_lnglat"></param>
        /// <returns></returns>
        public static string[] BD09_to_GCJ02(string[] _lnglat)
        {
            return BD09_to_GCJ02(_lnglat[0], _lnglat[1]);
        }

        /// <summary>
        /// 百度坐标转高德坐标
        /// </summary>
        /// <param name="in_lng"></param>
        /// <param name="in_lat"></param>
        /// <param name="out_lng"></param>
        /// <param name="out_lat"></param>
        public static void BD09_to_GCJ02(string in_lng, string in_lat, out string out_lng, out string out_lat)
        {
            double _lng, _lat;
            BD09_to_GCJ02(in_lng.ToDouble(), in_lat.ToDouble(), out _lng, out _lat);
            out_lng = _lng.ToString(decimals);
            out_lat = _lat.ToString(decimals);
        }
        #endregion

        #region 百度坐标转WGS84坐标
        /// <summary>
        /// 百度坐标转WGS84坐标
        /// </summary>
        /// <param name="in_lng"></param>
        /// <param name="in_lat"></param>
        /// <param name="out_lng"></param>
        /// <param name="out_lat"></param>
        public static void BD09_to_WGS84(double in_lng, double in_lat, out double out_lng, out double out_lat)
        {
            BD09_to_GCJ02(in_lng, in_lat, out out_lng, out out_lat);
            GCJ02_to_WGS84(out_lng, out_lat, out out_lng, out out_lat);
        }
        #endregion

        #region WGS-84 to Web mercator
        /// <summary>
        /// WGS-84 to Web mercator
        /// </summary>
        /// <param name="in_lng"></param>
        /// <param name="in_lat"></param>
        /// <param name="out_lng"></param>
        /// <param name="out_lat"></param>
        public static void WGS84_to_Mercator(double in_lng, double in_lat, out double out_lng, out double out_lat)
        {
            out_lng = in_lng * 20037508.34 / 180.0;
            double y = Math.Log(Math.Tan((90.0 + in_lat) * pi / 360.0)) / (pi / 180.0);
            out_lat = y * 20037508.34 / 180.0;
        }

        /// <summary>
        /// WGS-84 to Web mercator
        /// </summary>
        /// <param name="lat"></param>
        /// <param name="lng"></param>
        public static double[] WGS84_to_Mercator(double lng, double lat)
        {
            double[] lnglat = new double[2];
            WGS84_to_Mercator(lng, lat, out lnglat[0], out lnglat[1]);
            return lnglat;
        }

        /// <summary>
        /// WGS-84 to Web mercator
        /// </summary>
        /// <param name="lnglat"></param>
        /// <returns></returns>
        public static double[] WGS84_to_Mercator(double[] lnglat)
        {
            return WGS84_to_Mercator(lnglat[0], lnglat[1]);
        }

        /// <summary>
        /// WGS-84 to Web mercator
        /// </summary>
        /// <param name="lng"></param>
        /// <param name="lat"></param>
        /// <returns></returns>
        public static string[] WGS84_to_Mercator(string lng, string lat)
        {
            double _lng, _lat;
            WGS84_to_Mercator(lng.ToDouble(), lat.ToDouble(), out _lng, out _lat);
            return new string[] { _lng.ToString(decimals), _lat.ToString(decimals) };
        }

        /// <summary>
        /// WGS-84 to Web mercator
        /// </summary>
        /// <param name="_lnglat"></param>
        /// <returns></returns>
        public static string[] WGS84_to_Mercator(string[] _lnglat)
        {
            return WGS84_to_Mercator(_lnglat[0], _lnglat[1]);
        }

        /// <summary>
        /// WGS-84 to Web mercator
        /// </summary>
        /// <param name="in_lng"></param>
        /// <param name="in_lat"></param>
        /// <param name="out_lng"></param>
        /// <param name="out_lat"></param>
        public static void WGS84_to_Mercator(string in_lng, string in_lat, out string out_lng, out string out_lat)
        {
            double _lng, _lat;
            WGS84_to_Mercator(in_lng.ToDouble(), in_lat.ToDouble(), out _lng, out _lat);
            out_lng = _lng.ToString(decimals);
            out_lat = _lat.ToString(decimals);
        }
        #endregion

        #region Web mercator to WGS-84
        /// <summary>
        /// Web mercator to WGS-84
        /// </summary>
        /// <param name="in_lng"></param>
        /// <param name="in_lat"></param>
        /// <param name="out_lng"></param>
        /// <param name="out_lat"></param>
        public static void Mercator_to_WGS84(double in_lng, double in_lat, out double out_lng, out double out_lat)
        {
            out_lng = in_lng / 20037508.34 * 180.0;
            double y = in_lat / 20037508.34 * 180.0;
            out_lat = 180.0 / pi * (2 * Math.Atan(Math.Exp(y * pi / 180.0)) - pi / 2);
        }

        /// <summary>
        /// Web mercator to WGS-84
        /// </summary>
        /// <param name="lat"></param>
        /// <param name="lng"></param>
        public static double[] Mercator_to_WGS84(double lng, double lat)
        {
            double[] lnglat = new double[2];
            Mercator_to_WGS84(lng, lat, out lnglat[0], out lnglat[1]);
            return lnglat;
        }

        /// <summary>
        /// Web mercator to WGS-84
        /// </summary>
        /// <param name="lnglat"></param>
        /// <returns></returns>
        public static double[] Mercator_to_WGS84(double[] lnglat)
        {
            return Mercator_to_WGS84(lnglat[0], lnglat[1]);
        }

        /// <summary>
        /// Web mercator to WGS-84
        /// </summary>
        /// <param name="lng"></param>
        /// <param name="lat"></param>
        /// <returns></returns>
        public static string[] Mercator_to_WGS84(string lng, string lat)
        {
            double _lng, _lat;
            Mercator_to_WGS84(lng.ToDouble(), lat.ToDouble(), out _lng, out _lat);
            return new string[] { _lng.ToString(decimals), _lat.ToString(decimals) };
        }

        /// <summary>
        /// Web mercator to WGS-84
        /// </summary>
        /// <param name="_lnglat"></param>
        /// <returns></returns>
        public static string[] Mercator_to_WGS84(string[] _lnglat)
        {
            return Mercator_to_WGS84(_lnglat[0], _lnglat[1]);
        }

        /// <summary>
        /// Web mercator to WGS-84
        /// </summary>
        /// <param name="in_lng"></param>
        /// <param name="in_lat"></param>
        /// <param name="out_lng"></param>
        /// <param name="out_lat"></param>
        public static void Mercator_to_WGS84(string in_lng, string in_lat, out string out_lng, out string out_lat)
        {
            double _lng, _lat;
            Mercator_to_WGS84(in_lng.ToDouble(), in_lat.ToDouble(), out _lng, out _lat);
            out_lng = _lng.ToString(decimals);
            out_lat = _lat.ToString(decimals);
        }
        #endregion

        #region WGS84坐标转高德坐标
        /// <summary>
        /// WGS84坐标转高德坐标
        /// </summary>
        /// <param name="in_lng"></param>
        /// <param name="in_lat"></param>
        /// <param name="out_lng"></param>
        /// <param name="out_lat"></param>
        public static void WGS84_to_GCJ02(double in_lng, double in_lat, out double out_lng, out double out_lat)
        {
            if (OutOfChina(in_lat, in_lng))
            {
                out_lng = in_lng;
                out_lat = in_lat;
                return;
            }

            CalculateDev(in_lng, in_lat, out out_lng, out out_lat);
            out_lng = in_lng + out_lng;
            out_lat = in_lat + out_lat;
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
        /// <param name="lnglat"></param>
        /// <returns></returns>
        public static double[] WGS84_to_GCJ02(double[] lnglat)
        {
            return WGS84_to_GCJ02(lnglat[0], lnglat[1]);
        }

        /// <summary>
        /// WGS84坐标转高德坐标
        /// </summary>
        /// <param name="lng"></param>
        /// <param name="lat"></param>
        /// <returns></returns>
        public static string[] WGS84_to_GCJ02(string lng, string lat)
        {
            double _lng, _lat;
            WGS84_to_GCJ02(lng.ToDouble(), lat.ToDouble(), out _lng, out _lat);
            return new string[] { _lng.ToString(decimals), _lat.ToString(decimals) };
        }

        /// <summary>
        /// WGS84坐标转高德坐标
        /// </summary>
        /// <param name="_lnglat"></param>
        /// <returns></returns>
        public static string[] WGS84_to_GCJ02(string[] _lnglat)
        {
            return WGS84_to_GCJ02(_lnglat[0], _lnglat[1]);
        }

        /// <summary>
        /// WGS84坐标转高德坐标
        /// </summary>
        /// <param name="in_lng"></param>
        /// <param name="in_lat"></param>
        /// <param name="out_lng"></param>
        /// <param name="out_lat"></param>
        public static void WGS84_to_GCJ02(string in_lng, string in_lat, out string out_lng, out string out_lat)
        {
            double _lng, _lat;
            WGS84_to_GCJ02(in_lng.ToDouble(), in_lat.ToDouble(), out _lng, out _lat);
            out_lng = _lng.ToString(decimals);
            out_lat = _lat.ToString(decimals);
        }
        #endregion

        #region WGS84坐标转百度坐标
        /// <summary>
        /// WGS84坐标转百度坐标
        /// </summary>
        /// <param name="in_lng"></param>
        /// <param name="in_lat"></param>
        /// <param name="out_lng"></param>
        /// <param name="out_lat"></param>
        public static void WGS84_to_BD09(double in_lng, double in_lat, out double out_lng, out double out_lat)
        {
            WGS84_to_GCJ02(in_lng, in_lat, out out_lng, out out_lat);
            GCJ02_to_BD09(out_lng, out_lat, out out_lng, out out_lat);
        }
        #endregion

        /// <summary>
        /// 得到两点之间的距离
        /// </summary>
        public static double GetDistance(double latA, double lonA, double latB, double lonB)
        {
            double x = Math.Cos(latA * pi / 180.0) * Math.Cos(latB * pi / 180.0) * Math.Cos((lonA - lonB) * pi / 180.0);
            double y = Math.Sin(latA * pi / 180.0) * Math.Sin(latB * pi / 180.0);
            double s = x + y;
            if (s > 1) s = 1;
            if (s < -1) s = -1;
            double alpha = Math.Acos(s);
            double distance = alpha * earthR;
            return distance;
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
