using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pathtracer
{
    public struct Material
    {
        public Vector3 color;
        public double roughness;

        public Material(Vector3 color, double roughness)
        {
            this.color = color;
            this.roughness = roughness;
        }
    }
}
