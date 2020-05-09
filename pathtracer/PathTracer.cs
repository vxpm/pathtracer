using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace pathtracer
{
    public class PathTracer
    {
        // Image Properties
        const int width = 800;
        const int height = 400;
        const int sampleCount = 32;
        const int maxBounces = 16;

        Vector3[,] pixels;
        public Bitmap Image;

        Vector3 camera;
        Vector3[] imagePlane;

        readonly Vector3 backgroundColor = new Vector3(0, 0, 0);

        List<Shape> instancedShapes = new List<Shape>();

        void Setup()
        {
            imagePlane = new Vector3[4];
            imagePlane[0] = new Vector3(-2, 1, 0);  // TOP LEFT
            imagePlane[1] = new Vector3(2, 1, 0);   // TOP RIGHT
            imagePlane[2] = new Vector3(-2, -1, 0); // BOTTOM LEFT
            imagePlane[3] = new Vector3(2, -1, 0);  // BOTTOM RIGHT

            pixels = new Vector3[width, height];
            camera = new Vector3(0, 0, -2);

            instancedShapes.Clear();

            //  SCENE 1
            instancedShapes.Add(new Sphere(new Vector3(-1.5, -1, 2.25), 0.8, new Material(new Vector3(50, 255, 255), 1)));
            //instancedShapes.Add(new Sphere(new Vector3(-0.5, 1, 6), 1, new Vector3(25, 255, 20))); // test
            instancedShapes.Add(new Sphere(new Vector3(0, 0, 3), 1, new Material(new Vector3(255, 40, 20), 1)));
            instancedShapes.Add(new Sphere(new Vector3(2, -1, 3.5), 1, new Material(new Vector3(255, 255, 60), 1)));
            instancedShapes.Add(new Sphere(new Vector3(2.5, 2, 1), 2, new Material(new Vector3(500, 500, 500), 1), true));
            instancedShapes.Add(new Sphere(new Vector3(-3.5, 4, 2.5), 2, new Material(new Vector3(25, 500, 25), 1), true));

            instancedShapes.Add(new Sphere(new Vector3(-20, -1, 25), 1, new Material(new Vector3(500, 500, 599), 1), true));

            instancedShapes.Add(new Sphere(new Vector3(0, -10002.8, 2), 10000, new Material(new Vector3(255, 255, 255), 1)));

            //instancedShapes.Add(new InfinitePlane(new Vector3(0, 0, 3.5), new Vector3(0, 0, 1).Normalized, new Vector3(255, 255, 255), false));
            //instancedShapes.Add(new InfinitePlane(new Vector3(0, -2, 0), new Vector3(0, -1, 0).Normalized, new Vector3(255, 255, 255), false));
            //instancedShapes.Add(new Sphere(new Vector3(0, -8, 3), 2, new Vector3(255, 255, 255), true));

        }

        public async void Render(Bitmap outputImage = null)
        {
            int renderingPixels = 0;
            Setup();

            for(int _y = 0; _y < height; _y++)
            {
                for (int _x = 0; _x < width; _x++)
                {
                    while (renderingPixels > 23)
                        await Task.Yield();

#pragma warning disable CS4014 // Como esta chamada não é esperada, a execução do método atual continua antes de a chamada ser concluída

                    Interlocked.Increment(ref renderingPixels);
                    int x = _x;
                    int y = _y;

                    Task.Run(() =>
                    {
                        Vector3 topX, bottomX, imagePlanePoint, dir;
                        Vector3[] samples = new Vector3[sampleCount];

                        for (int i = 0; i < sampleCount; i++)
                        {
                            topX = imagePlane[0].Lerp(imagePlane[1], (x + StaticRandom.NextDouble() / 2) / width);
                            bottomX = imagePlane[2].Lerp(imagePlane[3], (x + StaticRandom.NextDouble() / 2) / width);
                            imagePlanePoint = topX.Lerp(bottomX, (y + StaticRandom.NextDouble() / 2) / height);
                            dir = (imagePlanePoint - camera).Normalized;

                            List<Vector3> colorsHit = new List<Vector3>();

                            // if hit a light before reaching max. bounces
                            if (TraceRay(camera, dir, colorsHit, null))
                            {
                                samples[i] = ComputeSample(colorsHit);
                            } else
                            {
                                // if it doesn't hit any light, make it hit a ambient light and compute the sample
                                colorsHit.Add(new Vector3(20, 20, 20));
                                samples[i] = ComputeSample(colorsHit);
                            }
                        }

                        Vector3 finalColor = Vector3.Zero;
                        for (int i = 0; i < sampleCount; i++)
                        {
                            finalColor += samples[i];
                        }
                        finalColor /= sampleCount;

                        pixels[x, y] = finalColor;

                        Interlocked.Decrement(ref renderingPixels);
                    });
#pragma warning restore CS4014 // Como esta chamada não é esperada, a execução do método atual continua antes de a chamada ser concluída
                }

                Console.WriteLine($"{_y}/{height}");
            }

            Bitmap render = new Bitmap(width, height);
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Vector3 c = pixels[x, y];
                    render.SetPixel(x, y, Color.FromArgb(RoundToByte(c.x), RoundToByte(c.y), RoundToByte(c.z)));
                }
            }

            render.Save("out.png", System.Drawing.Imaging.ImageFormat.Png);

            outputImage = render;
        }

        public byte RoundToByte(double n)
        {
            return Convert.ToByte(Math.Floor(Math.Min(Math.Max(n, 0), 255)));
        }

        // returns bool indicating whether it hit a light
        // -> doesn't check for intersections with the origin shape
        bool TraceRay(Vector3 origin, Vector3 direction, List<Vector3> colorsHit, Shape originShape)
        {
            if (colorsHit.Count > maxBounces + 1)
                return false;

            Shape closestShape = null;
            Vector3 closestShapeHitPos = Vector3.Zero;

            double smallestDistance = 999999;
            for (int i = 0; i < instancedShapes.Count; i++)
            {
                if (instancedShapes[i] != originShape)
                {
                    Vector3 hitPos;
                    bool hitShape = instancedShapes[i].GetIntersection(origin, direction, out hitPos);

                    if (hitShape)
                    {
                        double dist = (hitPos - origin).LengthSquared;
                        if (dist < smallestDistance)
                        {
                            closestShape = instancedShapes[i];
                            closestShapeHitPos = hitPos;
                            smallestDistance = dist;
                        }
                    }
                }
            }

            if (closestShape != null)
            {
                if (closestShape.isLight)
                {
                    colorsHit.Add(closestShape.material.color);
                    return true;
                }
                else
                {
                    colorsHit.Add(closestShape.material.color);

                    Vector3 dir = Vector3.One.Normalized;
                    Vector3 normal = closestShape.GetNormal(closestShapeHitPos);
                    bool sameHemisphere = false;

                    while (!sameHemisphere)
                    {
                        dir = new Vector3(StaticRandom.NextDouble(), StaticRandom.NextDouble(), StaticRandom.NextDouble()).Normalized;

                        if (dir % normal >= 0)
                            sameHemisphere = true;
                    }

                    return TraceRay(closestShapeHitPos, dir, colorsHit, closestShape);
                }
            }

            return false;
        }

        Vector3 ComputeSample(List<Vector3> colorsHit)
        {
            // last color in list is always expected to be the color of the light
            Vector3 sampleColor = colorsHit.Last() / 255.0;
            for (int k = colorsHit.Count - 2; k >= 0; k--)
            {
                sampleColor *= (colorsHit[k] / 255.0);
            }

            return sampleColor * 255.0;
        }
    }
}
