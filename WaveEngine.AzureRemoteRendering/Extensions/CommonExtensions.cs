// Copyright © Wave Engine S.L. All rights reserved. Use is subject to license terms.

using Microsoft.Azure.RemoteRendering;
using WaveEngine.Common.Graphics;
using WaveEngine.Mathematics;
using ARRMatrix4x4 = Microsoft.Azure.RemoteRendering.Matrix4x4;
using ARRQuaternion = Microsoft.Azure.RemoteRendering.Quaternion;
using WaveMatrix4x4 = WaveEngine.Mathematics.Matrix4x4;
using WaveQuaternion = WaveEngine.Mathematics.Quaternion;

namespace WaveEngine.AzureRemoteRendering
{
    /// <summary>
    /// Wave Engine specific extensions for common types.
    /// </summary>
    public static class CommonExtensions
    {
        /// <summary>
        /// Converts a <see cref="Vector2"/> into a remote <see cref="Float2"/>.
        /// </summary>
        /// <param name="input">The vector to be converted.</param>
        /// <param name="output">The converted vector.</param>
        public static void ToRemote(this Vector2 input, out Float2 output)
        {
            output = new Float2(input.X, input.Y);
        }

        /// <summary>
        /// Converts a <see cref="Vector2"/> into a remote <see cref="Float2"/>.
        /// </summary>
        /// <param name="input">The vector to be converted.</param>
        /// <returns>The converted vector.</returns>
        public static Float2 ToRemote(this Vector2 input)
        {
            input.ToRemote(out Float2 output);
            return output;
        }

        /// <summary>
        /// Converts a remote <see cref="Float2"/> into a <see cref="Vector2"/>.
        /// </summary>
        /// <param name="input">The vector to be converted.</param>
        /// <param name="output">The converted vector.</param>
        public static void ToWave(this Float2 input, out Vector2 output)
        {
            output.X = input.X;
            output.Y = input.Y;
        }

        /// <summary>
        /// Converts a remote <see cref="Float2"/> into a <see cref="Vector2"/>.
        /// </summary>
        /// <param name="input">The vector to be converted.</param>
        /// <returns>The converted vector.</returns>
        public static Vector2 ToWave(this Float2 input)
        {
            input.ToWave(out Vector2 output);
            return output;
        }

        /// <summary>
        /// Converts a <see cref="Vector3"/> into a remote <see cref="Float3"/>.
        /// </summary>
        /// <param name="input">The vector to be converted.</param>
        /// <param name="output">The converted vector.</param>
        public static void ToRemote(this Vector3 input, out Float3 output)
        {
            output = new Float3(input.X, input.Y, input.Z);
        }

        /// <summary>
        /// Converts a <see cref="Vector3"/> into a remote <see cref="Float3"/>.
        /// </summary>
        /// <param name="input">The vector to be converted.</param>
        /// <returns>The converted vector.</returns>
        public static Float3 ToRemoteFloat(this Vector3 input)
        {
            input.ToRemote(out Float3 output);
            return output;
        }

        /// <summary>
        /// Converts a <see cref="Vector3"/> into a remote <see cref="Double3"/>.
        /// </summary>
        /// <param name="input">The vector to be converted.</param>
        /// <param name="output">The converted vector.</param>
        public static void ToRemote(this Vector3 input, out Double3 output)
        {
            output = new Double3(input.X, input.Y, input.Z);
        }

        /// <summary>
        /// Converts a <see cref="Vector3"/> into a remote <see cref="Double3"/>.
        /// </summary>
        /// <param name="input">The vector to be converted.</param>
        /// <returns>The converted vector.</returns>
        public static Double3 ToRemoteDouble(this Vector3 input)
        {
            input.ToRemote(out Double3 output);
            return output;
        }

        /// <summary>
        /// Converts a remote <see cref="Float3"/> into a <see cref="Vector3"/>.
        /// </summary>
        /// <param name="input">The vector to be converted.</param>
        /// <param name="output">The converted vector.</param>
        public static void ToWave(this Float3 input, out Vector3 output)
        {
            output.X = input.X;
            output.Y = input.Y;
            output.Z = input.Z;
        }

        /// <summary>
        /// Converts a remote <see cref="Float3"/> into a <see cref="Vector3"/>.
        /// </summary>
        /// <param name="input">The vector to be converted.</param>
        /// <returns>The converted vector.</returns>
        public static Vector3 ToWave(this Float3 input)
        {
            input.ToWave(out Vector3 output);
            return output;
        }

        /// <summary>
        /// Converts a remote <see cref="Double3"/> into a <see cref="Vector3"/>.
        /// </summary>
        /// <param name="input">The vector to be converted.</param>
        /// <param name="output">The converted vector.</param>
        public static void ToWave(this Double3 input, out Vector3 output)
        {
            output.X = (float)input.X;
            output.Y = (float)input.Y;
            output.Z = (float)input.Z;
        }

        /// <summary>
        /// Converts a remote <see cref="Double3"/> into a <see cref="Vector3"/>.
        /// </summary>
        /// <param name="input">The vector to be converted.</param>
        /// <returns>The converted vector.</returns>
        public static Vector3 ToWave(this Double3 input)
        {
            input.ToWave(out Vector3 output);
            return output;
        }

        /// <summary>
        /// Converts a <see cref="Vector4"/> into a remote <see cref="Float4"/>.
        /// </summary>
        /// <param name="input">The vector to be converted.</param>
        /// <param name="output">The converted vector.</param>
        public static void ToRemote(this Vector4 input, out Float4 output)
        {
            output = new Float4(input.X, input.Y, input.Z, input.W);
        }

        /// <summary>
        /// Converts a <see cref="Vector4"/> into a remote <see cref="Float4"/>.
        /// </summary>
        /// <param name="input">The vector to be converted.</param>
        /// <returns>The converted vector.</returns>
        public static Float4 ToRemote(this Vector4 input)
        {
            input.ToRemote(out Float4 output);
            return output;
        }

        /// <summary>
        /// Converts a remote <see cref="Float4"/> into a <see cref="Vector4"/>.
        /// </summary>
        /// <param name="input">The vector to be converted.</param>
        /// <param name="output">The converted vector.</param>
        public static void ToWave(this Float4 input, out Vector4 output)
        {
            output = new Vector4(input.X, input.Y, input.Z, input.W);
        }

        /// <summary>
        /// Converts a remote <see cref="Float4"/> into a <see cref="Vector4"/>.
        /// </summary>
        /// <param name="input">The vector to be converted.</param>
        /// <returns>The converted vector.</returns>
        public static Vector4 ToWave(this Float4 input)
        {
            input.ToWave(out Vector4 output);
            return output;
        }

        /// <summary>
        /// Converts a <see cref="WaveMatrix4x4"/> into a remote <see cref="ARRMatrix4x4"/>.
        /// </summary>
        /// <param name="input">The matrix to be converted.</param>
        /// <param name="output">The converted matrix.</param>
        public static void ToRemote(this WaveMatrix4x4 input, out ARRMatrix4x4 output)
        {
            var col0 = new Float4(input.M11, input.M12, input.M13, input.M14);
            var col1 = new Float4(input.M21, input.M22, input.M23, input.M24);
            var col2 = new Float4(input.M31, input.M32, input.M33, input.M34);
            var col3 = new Float4(input.M41, input.M42, input.M43, input.M44);

            output = new ARRMatrix4x4(col0, col1, col2, col3);
        }

        /// <summary>
        /// Converts a <see cref="WaveMatrix4x4"/> into a remote <see cref="ARRMatrix4x4"/>.
        /// </summary>
        /// <param name="input">The matrix to be converted.</param>
        /// <returns>The converted matrix.</returns>
        public static ARRMatrix4x4 ToRemote(this WaveMatrix4x4 input)
        {
            input.ToRemote(out ARRMatrix4x4 output);
            return output;
        }

        /// <summary>
        /// Converts a remote <see cref="ARRMatrix4x4"/> into a <see cref="WaveMatrix4x4"/>.
        /// </summary>
        /// <param name="input">The matrix to be converted.</param>
        /// <param name="output">The converted matrix.</param>
        public static void ToWave(this ARRMatrix4x4 input, out WaveMatrix4x4 output)
        {
            output.M11 = input.Column0.X;
            output.M12 = input.Column0.Y;
            output.M13 = input.Column0.Z;
            output.M14 = input.Column0.W;

            output.M21 = input.Column1.X;
            output.M22 = input.Column1.Y;
            output.M23 = input.Column1.Z;
            output.M24 = input.Column1.W;

            output.M31 = input.Column2.X;
            output.M32 = input.Column2.Y;
            output.M33 = input.Column2.Z;
            output.M34 = input.Column2.W;

            output.M41 = input.Column3.X;
            output.M42 = input.Column3.Y;
            output.M43 = input.Column3.Z;
            output.M44 = input.Column3.W;
        }

        /// <summary>
        /// Converts a remote <see cref="ARRMatrix4x4"/> into a <see cref="WaveMatrix4x4"/>.
        /// </summary>
        /// <param name="input">The matrix to be converted.</param>
        /// <returns>The converted matrix.</returns>
        public static WaveMatrix4x4 ToWave(this ARRMatrix4x4 input)
        {
            input.ToWave(out WaveMatrix4x4 output);
            return output;
        }

        /// <summary>
        /// Converts a <see cref="WaveQuaternion"/> into a remote <see cref="ARRQuaternion"/>.
        /// </summary>
        /// <param name="input">The quaternion to be converted.</param>
        /// <param name="output">The converted quaternion.</param>
        public static void ToRemote(this WaveQuaternion input, out ARRQuaternion output)
        {
            output = new ARRQuaternion(input.X, input.Y, input.Z, input.W);
        }

        /// <summary>
        /// Converts a <see cref="WaveQuaternion"/> into a remote <see cref="ARRQuaternion"/>.
        /// </summary>
        /// <param name="input">The quaternion to be converted.</param>
        /// <returns>The converted quaternion.</returns>
        public static ARRQuaternion ToRemote(this WaveQuaternion input)
        {
            input.ToRemote(out ARRQuaternion output);
            return output;
        }

        /// <summary>
        /// Converts a remote <see cref="ARRQuaternion"/> into a <see cref="WaveQuaternion"/>.
        /// </summary>
        /// <param name="input">The quaternion to be converted.</param>
        /// <param name="output">The converted quaternion.</param>
        public static void ToWave(this ARRQuaternion input, out WaveQuaternion output)
        {
            output.X = input.X;
            output.Y = input.Y;
            output.Z = input.Z;
            output.W = input.W;
        }

        /// <summary>
        /// Converts a remote <see cref="ARRQuaternion"/> into a <see cref="WaveQuaternion"/>.
        /// </summary>
        /// <param name="input">The quaternion to be converted.</param>
        /// <returns>The converted quaternion.</returns>
        public static WaveQuaternion ToWave(this ARRQuaternion input)
        {
            input.ToWave(out WaveQuaternion output);
            return output;
        }

        /// <summary>
        /// Converts a <see cref="BoundingBox"/> into a remote <see cref="Bounds"/>.
        /// </summary>
        /// <param name="input">The bounding box to be converted.</param>
        /// <param name="output">The converted bounding box.</param>
        public static void ToRemote(this BoundingBox input, out Bounds output)
        {
            output = new Bounds(input.Min.ToRemoteDouble(), input.Max.ToRemoteDouble());
        }

        /// <summary>
        /// Converts a <see cref="BoundingBox"/> into a remote <see cref="Bounds"/>.
        /// </summary>
        /// <param name="input">The bounding box to be converted.</param>
        /// <returns>The converted bounding box.</returns>
        public static Bounds ToRemote(this BoundingBox input)
        {
            input.ToRemote(out Bounds output);
            return output;
        }

        /// <summary>
        /// Converts a remote <see cref="Bounds"/> into a <see cref="BoundingBox"/>.
        /// </summary>
        /// <param name="input">The bounding box to be converted.</param>
        /// <param name="output">The converted bounding box.</param>
        public static void ToWave(this Bounds input, out BoundingBox output)
        {
            input.Min.ToWave(out output.Min);
            input.Max.ToWave(out output.Max);
        }

        /// <summary>
        /// Converts a remote <see cref="Bounds"/> into a <see cref="BoundingBox"/>.
        /// </summary>
        /// <param name="input">The bounding box to be converted.</param>
        /// <returns>The converted bounding box.</returns>
        public static BoundingBox ToWave(this Bounds input)
        {
            input.ToWave(out BoundingBox output);
            return output;
        }

        /// <summary>
        /// Converts a <see cref="Color"/> into a remote <see cref="Color4"/>.
        /// </summary>
        /// <param name="input">The color to be converted.</param>
        /// <param name="output">The converted color.</param>
        public static void ToRemote(this Color input, out Color4 output)
        {
            output = new Color4(input.R / 255f, input.G / 255f, input.B / 255f, input.A / 255f);
        }

        /// <summary>
        /// Converts a <see cref="Color"/> into a remote <see cref="Color4"/>.
        /// </summary>
        /// <param name="input">The color to be converted.</param>
        /// <returns>The converted color.</returns>
        public static Color4 ToRemoteColor4(this Color input)
        {
            input.ToRemote(out Color4 output);
            return output;
        }

        /// <summary>
        /// Converts a remote <see cref="Color4"/> into a <see cref="Color"/>.
        /// </summary>
        /// <param name="input">The color to be converted.</param>
        /// <param name="output">The converted color.</param>
        public static void ToWave(this Color4 input, out Color output)
        {
            output.R = (byte)(input.R * 255);
            output.G = (byte)(input.G * 255);
            output.B = (byte)(input.B * 255);
            output.A = (byte)(input.A * 255);
        }

        /// <summary>
        /// Converts a remote <see cref="Color4"/> into a <see cref="Color"/>.
        /// </summary>
        /// <param name="input">The color to be converted.</param>
        /// <returns>The converted color.</returns>
        public static Color ToWave(this Color4 input)
        {
            input.ToWave(out Color output);
            return output;
        }

        /// <summary>
        /// Converts a <see cref="Color"/> into a remote <see cref="Color4Ub"/>.
        /// </summary>
        /// <param name="input">The color to be converted.</param>
        /// <param name="output">The converted color.</param>
        public static void ToRemote(this Color input, out Color4Ub output)
        {
            output.Bytes = 0;
            output.Channels.R = input.R;
            output.Channels.G = input.G;
            output.Channels.B = input.B;
            output.Channels.A = input.A;
        }

        /// <summary>
        /// Converts a <see cref="Color"/> into a remote <see cref="Color4Ub"/>.
        /// </summary>
        /// <param name="input">The color to be converted.</param>
        /// <returns>The converted color.</returns>
        public static Color4Ub ToRemoteColor4Ub(this Color input)
        {
            input.ToRemote(out Color4Ub output);
            return output;
        }

        /// <summary>
        /// Converts a remote <see cref="Color4Ub"/> into a <see cref="Color"/>.
        /// </summary>
        /// <param name="input">The color to be converted.</param>
        /// <param name="output">The converted color.</param>
        public static void ToWave(this Color4Ub input, out Color output)
        {
            output.R = input.Channels.R;
            output.G = input.Channels.G;
            output.B = input.Channels.B;
            output.A = input.Channels.A;
        }

        /// <summary>
        /// Converts a remote <see cref="Color4Ub"/> into a <see cref="Color"/>.
        /// </summary>
        /// <param name="input">The color to be converted.</param>
        /// <returns>The converted color.</returns>
        public static Color ToWave(this Color4Ub input)
        {
            input.ToWave(out Color output);
            return output;
        }

        /// <summary>
        /// Converts a <see cref="RayStep"/> into a remote <see cref="RayCast"/>.
        /// </summary>
        /// <param name="input">The ray step to be converted.</param>
        /// <param name="output">The converted ray cast.</param>
        /// <param name="maxDistance">The ray max distance.</param>
        /// <param name="hitCollection">The hit collection mode to set on the converted ray cast.</param>
        /// <param name="maxHits">The maximum collected hits to set on the converted ray cast.</param>
        /// <param name="collisionMask">The collision mask to set on the converted ray cast.</param>
        public static void ToRemote(this RayStep input, out RayCast output, double maxDistance, HitCollectionPolicy hitCollection = HitCollectionPolicy.ClosestHits, int maxHits = 1024, uint collisionMask = 0xFFFFFFFF)
        {
            input.Origin.ToRemote(out Double3 starPos);
            input.Terminus.ToRemote(out Double3 endPos);
            output = new RayCast(starPos, endPos, maxDistance, hitCollection, maxHits, collisionMask);
        }

        /// <summary>
        /// Converts a <see cref="RayStep"/> into a remote <see cref="RayCast"/>.
        /// </summary>
        /// <param name="input">The ray step to be converted.</param>
        /// <param name="maxDistance">The ray max distance.</param>
        /// <param name="hitCollection">The hit collection mode to set on the converted ray cast.</param>
        /// <param name="maxHits">The maximum collected hits to set on the converted ray cast.</param>
        /// <param name="collisionMask">The collision mask to set on the converted ray cast.</param>
        /// <returns>The converted ray cast.</returns>
        public static RayCast ToRemote(this RayStep input, double maxDistance, HitCollectionPolicy hitCollection = HitCollectionPolicy.ClosestHits, int maxHits = 1024, uint collisionMask = 0xFFFFFFFF)
        {
            input.ToRemote(out var output, maxDistance, hitCollection, maxHits, collisionMask);
            return output;
        }

        /// <summary>
        /// Converts a remote <see cref="RayCast"/> into a <see cref="RayStep"/>.
        /// </summary>
        /// <param name="input">The ray cast to be converted.</param>
        /// <param name="output">The converted ray step.</param>
        public static void ToWave(this RayCast input, out RayStep output)
        {
            input.StartPos.ToWave(out var origin);
            input.EndPos.ToWave(out var terminus);
            output = new RayStep(origin, terminus);
        }

        /// <summary>
        /// Converts a remote <see cref="RayCast"/> into a <see cref="RayStep"/>.
        /// </summary>
        /// <param name="input">The ray cast to be converted.</param>
        /// <returns>The converted ray step.</returns>
        public static RayStep ToWaveRayStep(this RayCast input)
        {
            input.ToWave(out RayStep output);
            return output;
        }

        /// <summary>
        /// Converts a <see cref="Ray"/> into a remote <see cref="RayCast"/>.
        /// </summary>
        /// <param name="input">The ray to be converted.</param>
        /// <param name="output">The converted ray cast.</param>
        /// <param name="distance">The distance used to calculate the end position on the converted ray cast.</param>
        /// <param name="hitCollection">The hit collection mode to set on the converted ray cast.</param>
        /// <param name="maxHits">The maximum collected hits to set on the converted ray cast.</param>
        /// <param name="collisionMask">The collision mask to set on the converted ray cast.</param>
        public static void ToRemote(this Ray input, out RayCast output, float distance, HitCollectionPolicy hitCollection = HitCollectionPolicy.ClosestHits, int maxHits = 1024, uint collisionMask = 0xFFFFFFFF)
        {
            input.Position.ToRemote(out Double3 starPos);
            input.GetPoint(distance, out var terminus);
            terminus.ToRemote(out Double3 endPos);
            output = new RayCast(starPos, endPos, distance, hitCollection, maxHits, collisionMask);
        }

        /// <summary>
        /// Converts a <see cref="Ray"/> into a remote <see cref="RayCast"/>.
        /// </summary>
        /// <param name="input">The ray to be converted.</param>
        /// <param name="distance">The distance used to calculate the end position on the converted ray cast.</param>
        /// <param name="hitCollection">The hit collection mode to set on the converted ray cast.</param>
        /// <param name="maxHits">The maximum collected hits to set on the converted ray cast.</param>
        /// <param name="collisionMask">The collision mask to set on the converted ray cast.</param>
        /// <returns>The converted ray cast.</returns>
        public static RayCast ToRemote(this Ray input, float distance, HitCollectionPolicy hitCollection = HitCollectionPolicy.ClosestHits, int maxHits = 1024, uint collisionMask = 0xFFFFFFFF)
        {
            input.ToRemote(out var output, distance, hitCollection, maxHits, collisionMask);
            return output;
        }

        /// <summary>
        /// Converts a remote <see cref="RayCast"/> into a <see cref="Ray"/>.
        /// </summary>
        /// <param name="input">The ray cast to be converted.</param>
        /// <param name="output">The converted ray.</param>
        public static void ToWave(this RayCast input, out Ray output)
        {
            input.StartPos.ToWave(out var startPos);
            input.EndPos.ToWave(out var endPos);
            var direction = endPos - startPos;
            direction.Normalize();
            output = new Ray(startPos, direction);
        }

        /// <summary>
        /// Converts a remote <see cref="RayCast"/> into a <see cref="Ray"/>.
        /// </summary>
        /// <param name="input">The ray cast to be converted.</param>
        /// <returns>The converted ray.</returns>
        public static Ray ToWaveRay(this RayCast input)
        {
            input.ToWave(out Ray output);
            return output;
        }
    }
}
