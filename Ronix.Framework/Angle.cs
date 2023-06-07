using System;
using System.Globalization;

namespace Ronix.Framework
{
    public struct Angle
    {
        private const double DegToRad = Math.PI / 180d;
        
        private const double RadToDeg = 180d / Math.PI;

        private const double TwoPi = 2 * Math.PI;

        public Angle(double radians) : this()
        {
            var rad = radians % TwoPi;
            if (rad < 0)
                rad += TwoPi;
            Radians = rad;
        }

        public static Angle FromRadians(double radians)
        {
            return new Angle(radians);
        }

        public static Angle FromDegrees(double degrees)
        {
            return new Angle(degrees * DegToRad);
        }

        public double Radians { get; }

        public double Degrees => Radians * RadToDeg;

        public override bool Equals(object obj)
        {
            if (!(obj is Angle))
                return false;
            var angle = (Angle)obj;
            return Radians.Equals(angle.Radians);
        }

        public override int GetHashCode()
        {
            return Radians.GetHashCode();
        }

        public bool Equals(Angle other)
        {
            return Radians.Equals(other.Radians);
        }

        public override string ToString()
        {
            return Radians.ToString(CultureInfo.InvariantCulture);
        }

        public static bool operator ==(Angle left, Angle right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Angle left, Angle right)
        {
            return !left.Equals(right);
        }

        public static Angle operator +(Angle left, Angle right)
        {
            return new Angle(left.Radians + right.Radians);
        }

        public static Angle operator -(Angle left, Angle right)
        {
            return new Angle(left.Radians - right.Radians);
        }

        public static Angle operator -(Angle angle)
        {
            return new Angle(-angle.Radians);
        }

        public static Angle operator *(double factor, Angle angle)
        {
            return new Angle(angle.Radians * factor);
        }

        public static Angle operator *(Angle angle, double factor)
        {
            return new Angle(angle.Radians * factor);
        }

        public static Angle operator /(double factor, Angle angle)
        {
            return new Angle(angle.Radians / factor);
        }

        public static Angle operator /(Angle angle, double factor)
        {
            return new Angle(angle.Radians / factor);
        }

        public Angle MirrorHorizontal()
        {
            return new Angle(-Radians);
        }

        public Angle MirrorVertical()
        {
            return new Angle(Math.PI - Radians);
        }

        public Angle MirrorCentral()
        {
            return new Angle(Radians + Math.PI);
        }

        public double Sin()
        {
            return Math.Sin(Radians);
        }

        public double Cos()
        {
            return Math.Cos(Radians);
        }
        
        public double Tan()
        {
            return Math.Tan(Radians);
        }
    }
}
