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
        const int width = 1600;
        const int height = 800;
        const int sampleCount = 512;
        const int maxBounces = 24;

        const int tasksToUse = 12;
        const int remainder = height % tasksToUse;
        const int rowsPerTask = (height - remainder) / tasksToUse;


        Vector3 camera;
        Vector3[] imagePlane;
        Vector3[,] pixels;

        readonly Vector3 ambientColor = new Vector3(20, 20, 20);

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
            instancedShapes.Add(new Sphere(new Vector3(0, 0, 3), 1, new Material(new Vector3(255, 40, 20), 1)));
            instancedShapes.Add(new Sphere(new Vector3(2, -1, 3.5), 1, new Material(new Vector3(255, 255, 60), 1)));
            instancedShapes.Add(new Sphere(new Vector3(0, -10002.8, 2), 10000, new Material(new Vector3(255, 255, 255), 1)));
            instancedShapes.Add(new Sphere(new Vector3(2.5, 2, 1), 2, new Material(new Vector3(500, 500, 500), 1), true));
            instancedShapes.Add(new Sphere(new Vector3(-3.5, 4, 2.5), 2, new Material(new Vector3(25, 500, 25), 1), true));
            instancedShapes.Add(new Sphere(new Vector3(-20, -1, 25), 1, new Material(new Vector3(500, 500, 599), 1), true));

        }

        public async void Render()
        {
            Setup();

            Task[] tasks = new Task[tasksToUse];
            for(int i = 0; i < tasksToUse - 1; i++)
            {
                int taslkIndex = i;
                tasks[i] = Task.Run(() => RenderRows(rowsPerTask * taslkIndex, rowsPerTask * (taslkIndex + 1)));
            }
            tasks[tasksToUse - 1] = Task.Run(() => RenderRows(rowsPerTask * (tasksToUse - 1), rowsPerTask * tasksToUse + remainder));

            await Task.WhenAll(tasks);

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
            Console.WriteLine("rendering complete! saved to out.png");
        }

        void RenderRows(int start, int end)
        {
            Console.WriteLine($"rendering: {start} -> {end}");

            for (int y = start; y < end; y++)
            {
                for (int x = 0; x < width; x++)
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
                        }
                        else
                        {
                            // if it doesn't hit any light, make it hit a ambient light and compute the sample
                            colorsHit.Add(ambientColor);
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
                }
            }

            Console.WriteLine($"complete: {start} -> {end}");
        }

        byte RoundToByte(double n)
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
