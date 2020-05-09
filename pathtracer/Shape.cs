using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pathtracer
{
    public abstract class Shape
    {
        public Vector3 position { get;  set; }
        public Material material { get;  set; }
        public bool isLight { get; set; }

        public abstract bool GetIntersection(Vector3 origin, Vector3 direction, out Vector3 hitPos);
        public abstract Vector3 GetNormal(Vector3 point);
    }
}
