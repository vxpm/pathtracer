using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pathtracer
{
    public static class Util
    {
        public static double Lerp(double a, double b, double t)
        {
            return (1 - t) * a + t * b;
        }

        public static Vector3 Lerp(this Vector3 a, Vector3 b, double t)
        {
            Vector3 result = Vector3.Zero;
            result.x = Lerp(a.x, b.x, t);
            result.y = Lerp(a.y, b.y, t);
            result.z = Lerp(a.z, b.z, t);

            return result;
        }
    }
}
