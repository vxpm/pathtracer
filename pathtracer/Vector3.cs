using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace pathtracer
{
    public struct Vector3
    {
        public static Vector3 Zero { get { return new Vector3(0, 0, 0); } }
        public static Vector3 One { get { return new Vector3(1, 1, 1); } }

        public double x;
        public double y;
        public double z;

        public double Length { get { return Math.Sqrt(x * x + y * y + z * z); } }
        public double LengthSquared { get { return x * x + y * y + z * z; } }
        public Vector3 Normalized { get { return this / Length; } }

        public Vector3(double x, double y, double z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public override string ToString()
        {
            return $"({x}, {y}, {z})";
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 operator +(Vector3 a, Vector3 b)
            => new Vector3(a.x + b.x, a.y + b.y, a.z + b.z);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 operator -(Vector3 a, Vector3 b)
            => new Vector3(a.x - b.x, a.y - b.y, a.z - b.z);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 operator *(Vector3 a, Vector3 b)
            => new Vector3(a.x * b.x, a.y * b.y, a.z * b.z);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 operator /(Vector3 a, Vector3 b)
            => new Vector3(a.x / b.x, a.y / b.y, a.z / b.z);


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double operator %(Vector3 a, Vector3 b)
            => a.x * b.x + a.y * b.y + a.z * b.z;


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 operator *(Vector3 a, int b)
            => new Vector3(a.x * b, a.y * b, a.z * b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 operator *(int b, Vector3 a)
            => new Vector3(a.x * b, a.y * b, a.z * b);


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 operator /(Vector3 a, int b)
            => new Vector3(a.x / b, a.y / b, a.z / b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 operator /(int b, Vector3 a)
            => new Vector3(a.x / b, a.y / b, a.z / b);


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 operator *(Vector3 a, float b)
            => new Vector3(a.x * b, a.y * b, a.z * b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 operator *(float b, Vector3 a)
            => new Vector3(a.x * b, a.y * b, a.z * b);


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 operator /(Vector3 a, float b)
            => new Vector3(a.x / b, a.y / b, a.z / b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 operator /(float b, Vector3 a)
            => new Vector3(a.x / b, a.y / b, a.z / b);


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 operator *(Vector3 a, double b)
            => new Vector3(a.x * b, a.y * b, a.z * b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 operator *(double b, Vector3 a)
            => new Vector3(a.x * b, a.y * b, a.z * b);


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 operator /(Vector3 a, double b)
            => new Vector3(a.x / b, a.y / b, a.z / b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 operator /(double b, Vector3 a)
            => new Vector3(a.x / b, a.y / b, a.z / b);
    }
}
