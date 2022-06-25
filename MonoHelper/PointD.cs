using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoHelper
{
    public class PointD
    {
        public double X = 0, Y = 0;

        public PointD()
        {

        }

        public PointD(double _X, double _Y)
        {
            X = _X;
            Y = _Y;
        }

        public static PointD operator +(PointD point1, PointD point2)
        {
            return new PointD(point1.X + point2.X, point1.Y + point2.Y);
        }

        public static PointD operator -(PointD point1, PointD point2)
        {
            return new PointD(point1.X - point2.X, point1.Y - point2.Y);
        }

        public static PointD operator *(PointD point, double multiplier)
        {
            return new PointD(point.X * multiplier, point.Y * multiplier);
        }

        public static PointD operator *(double multiplier, PointD point)
        {
            return new PointD(point.X * multiplier, point.Y * multiplier);
        }

        public PointD Turn(double angle)
        {
            double l = Math.Sqrt(X * X + Y * Y);
            double d = angle + (double)Math.Atan2(X, Y);
            return new PointD(Math.Sin(d) * l, Math.Cos(d) * l);
        }
    }
}
