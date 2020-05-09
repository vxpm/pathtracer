using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pathtracer
{
    public class InfinitePlane : Shape
    {
        public Vector3 normal;

        public InfinitePlane(Vector3 position, Vector3 normal, Material material, bool isLight = false)
        {
            this.position = position;
            this.normal = normal;
            this.material = material;
            this.isLight = isLight;
        }

        public override bool GetIntersection(Vector3 origin, Vector3 direction, out Vector3 hitPos)
        {
            double denom = normal % direction;
            if (denom > 0)
            {
                Vector3 dif = position - origin;
                double t = (dif % normal) / denom;

                hitPos = origin + direction * t;
                return (t >= 0);
            }

            hitPos = Vector3.Zero;
            return false;
        }

        public override Vector3 GetNormal(Vector3 point)
        {
            return normal;
        }
    }
}
