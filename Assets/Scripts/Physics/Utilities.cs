namespace Physics
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    internal static class Utilities
    {
        static Utilities()
        {
            atmosphericDensityCurve = CreateAtmosphericDensityCurve();
        }

        private static AnimationCurve CreateAtmosphericDensityCurve()
        {
            Keyframe[] keyframes = // Values from Wolfram Alpha
            {
                new Keyframe(0, 1.226f),
                new Keyframe(100, 1.2f),
                new Keyframe(1000, 1.1f),
                new Keyframe(2000, 1.0f),
                new Keyframe(3000, 0.91f),
                new Keyframe(4000, 0.81f),
                new Keyframe(5000, 0.74f),
                new Keyframe(6000, 0.66f),
                new Keyframe(7000, 0.59f),
                new Keyframe(8000, 0.53f),
                new Keyframe(9000, 0.47f),
                new Keyframe(10000, 0.41f),
                new Keyframe(20000, 0.089f),
                new Keyframe(40000, 0.004f)
            };

            var curve = new AnimationCurve(keyframes);

            for (int i = 0; i < curve.length; i++) // Smooth out the curve
            {
                curve.SmoothTangents(i, 0.5f);
            }

            return curve;
        }

        private static AnimationCurve atmosphericDensityCurve;

        public static float AtmosphericDensity(Vector3 position)
        {
            float altitude = Vector3.Dot(position, Vector3.up);

            altitude = Mathf.Max(altitude, 0.0f); // Treat below sea level as sea level

            return atmosphericDensityCurve.Evaluate(altitude);
        }

        public static Vector3 AverageVector(Vector3[] vertices)
        {
            Vector3 total = Vector3.zero;

            for (int i = 0; i < vertices.Length; i++)
            {
                total += vertices[i];
            }

            return total / vertices.Length;
        }

        public static Vector3 CalculateCenterOfMass(Mesh mesh)
        {
            Vector3 average = AverageVector(mesh.vertices);

            Vector3 sum = Vector3.zero;
            float totalVolume = 0.0f;

            for (int i = 0; i < mesh.triangles.Length / 3; i++)
            {
                Vector3 a = mesh.vertices[mesh.triangles[i * 3 + 0]];
                Vector3 b = mesh.vertices[mesh.triangles[i * 3 + 1]];
                Vector3 c = mesh.vertices[mesh.triangles[i * 3 + 2]];
                Vector3 d = average;

                float volume = (1f / 6f) * Mathf.Abs(Vector3.Dot(b - a, Vector3.Cross(c - a, d - a)));

                Vector3 center = a + b + c + d;
                center /= 4;

                totalVolume += volume;
                sum += center * volume;
            }

            return sum / totalVolume;
        }

        public static Vector3 weightedAverage<T>(IEnumerable<T> data, Func<T, float> weightFunction, Func<T, Vector3> pointFunction)
        {
            float sumWeight = 0.0f;
            Vector3 sumPoint = default(Vector3);
            
            foreach(T dataEntry in data)
            {
                float weight = weightFunction(dataEntry);
                Vector3 point = pointFunction(dataEntry);

                sumPoint += point * weight;
                sumWeight += weight;
            }

            return sumPoint / sumWeight;
        }
    }

    internal struct Impulse
    {
        public Vector3 position;
        public Vector3 force;
    }
}