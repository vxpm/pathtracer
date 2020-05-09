using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pathtracer
{
    class Sphere : Shape
    {
        public double radius;

        public Sphere(Vector3 position, double radius, Material material, bool isLight = false)
        {
            this.position = position;
            this.radius = radius;
            this.material = material;
            this.isLight = isLight;
        }

        public override bool GetIntersection(Vector3 origin, Vector3 direction, out Vector3 hitPos)
        {
            double b = (2 * (origin - position)) % direction;
            double c = Math.Pow((origin - position).Length, 2) - Math.Pow(radius, 2);

            double delta = Math.Pow(b, 2) - 4 * c;
            if (delta < 0)
            {
                hitPos = Vector3.Zero;
                return false;
            }

            double t1 = (-b + Math.Sqrt(delta)) / 2;
            double t2 = (-b - Math.Sqrt(delta)) / 2;
            double t;

            if (Math.Min(t1, t2) > 0)
                t = Math.Min(t1, t2);
            else if (Math.Max(t1, t2) > 0)
                t = Math.Max(t1, t2);
            else
            {
                hitPos = Vector3.Zero;
                return false;
            }

            hitPos = origin + direction * t;
            return true;
        }

        public override Vector3 GetNormal(Vector3 point)
        {
            return (point - position).Normalized;
        }
    }
}
