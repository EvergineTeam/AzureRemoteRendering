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
            output.x = input.X;
            output.y = input.Y;
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
            output.X = input.x;
            output.Y = input.y;
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
            output.x = input.X;
            output.y = input.Y;
            output.z = input.Z;
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
            output.x = input.X;
            output.y = input.Y;
            output.z = input.Z;
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
            output.X = input.x;
            output.Y = input.y;
            output.Z = input.z;
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
            output.X = (float)input.x;
            output.Y = (float)input.y;
            output.Z = (float)input.z;
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
            output.x = input.X;
            output.y = input.Y;
            output.z = input.Z;
            output.w = input.W;
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
            output.X = input.x;
            output.Y = input.y;
            output.Z = input.z;
            output.W = input.w;
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
            output.column0.x = input.M11;
            output.column0.y = input.M12;
            output.column0.z = input.M13;
            output.column0.w = input.M14;

            output.column1.x = input.M21;
            output.column1.y = input.M22;
            output.column1.z = input.M23;
            output.column1.w = input.M24;

            output.column2.x = input.M31;
            output.column2.y = input.M32;
            output.column2.z = input.M33;
            output.column2.w = input.M34;

            output.column3.x = input.M41;
            output.column3.y = input.M42;
            output.column3.z = input.M43;
            output.column3.w = input.M44;
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
            output.M11 = input.column0.x;
            output.M12 = input.column0.y;
            output.M13 = input.column0.z;
            output.M14 = input.column0.w;

            output.M21 = input.column1.x;
            output.M22 = input.column1.y;
            output.M23 = input.column1.z;
            output.M24 = input.column1.w;

            output.M31 = input.column2.x;
            output.M32 = input.column2.y;
            output.M33 = input.column2.z;
            output.M34 = input.column2.w;

            output.M41 = input.column3.x;
            output.M42 = input.column3.y;
            output.M43 = input.column3.z;
            output.M44 = input.column3.w;
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
            output.x = input.X;
            output.y = input.Y;
            output.z = input.Z;
            output.w = input.W;
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
            output.X = input.x;
            output.Y = input.y;
            output.Z = input.z;
            output.W = input.w;
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
        /// Converts a <see cref="BoundingBox"/> into a remote <see cref="AABB3D"/>.
        /// </summary>
        /// <param name="input">The bounding box to be converted.</param>
        /// <param name="output">The converted bounding box.</param>
        public static void ToRemote(this BoundingBox input, out AABB3D output)
        {
            input.Min.ToRemote(out output.min);
            input.Max.ToRemote(out output.max);
        }

        /// <summary>
        /// Converts a <see cref="BoundingBox"/> into a remote <see cref="AABB3D"/>.
        /// </summary>
        /// <param name="input">The bounding box to be converted.</param>
        /// <returns>The converted bounding box.</returns>
        public static AABB3D ToRemote(this BoundingBox input)
        {
            input.ToRemote(out AABB3D output);
            return output;
        }

        /// <summary>
        /// Converts a remote <see cref="AABB3D"/> into a <see cref="BoundingBox"/>.
        /// </summary>
        /// <param name="input">The bounding box to be converted.</param>
        /// <param name="output">The converted bounding box.</param>
        public static void ToWave(this AABB3D input, out BoundingBox output)
        {
            input.min.ToWave(out output.Min);
            input.max.ToWave(out output.Max);
        }

        /// <summary>
        /// Converts a remote <see cref="AABB3D"/> into a <see cref="BoundingBox"/>.
        /// </summary>
        /// <param name="input">The bounding box to be converted.</param>
        /// <returns>The converted bounding box.</returns>
        public static BoundingBox ToWave(this AABB3D input)
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
            output.r = input.R / 255f;
            output.g = input.G / 255f;
            output.b = input.B / 255f;
            output.a = input.A / 255f;
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
            output.R = (byte)(input.r * 255);
            output.G = (byte)(input.g * 255);
            output.B = (byte)(input.b * 255);
            output.A = (byte)(input.a * 255);
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
            output.bytes = 0;
            output.channels.r = input.R;
            output.channels.g = input.G;
            output.channels.b = input.B;
            output.channels.a = input.A;
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
            output.R = input.channels.r;
            output.G = input.channels.g;
            output.B = input.channels.b;
            output.A = input.channels.a;
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
        /// <param name="hitCollection">The hit collection mode to set on the converted ray cast.</param>
        /// <param name="maxHits">The maximum collected hits to set on the converted ray cast.</param>
        /// <param name="collisionMask">The collision mask to set on the converted ray cast.</param>
        public static void ToRemote(this RayStep input, out RayCast output, HitCollectionPolicy hitCollection = HitCollectionPolicy.AllHits, uint maxHits = uint.MaxValue, uint collisionMask = 0xFFFFFFFF)
        {
            input.Origin.ToRemote(out Double3 starPos);
            input.Terminus.ToRemote(out Double3 endPos);
            output = new RayCast(starPos, endPos, hitCollection, maxHits, collisionMask);
        }

        /// <summary>
        /// Converts a <see cref="RayStep"/> into a remote <see cref="RayCast"/>.
        /// </summary>
        /// <param name="input">The ray step to be converted.</param>
        /// <param name="hitCollection">The hit collection mode to set on the converted ray cast.</param>
        /// <param name="maxHits">The maximum collected hits to set on the converted ray cast.</param>
        /// <param name="collisionMask">The collision mask to set on the converted ray cast.</param>
        /// <returns>The converted ray cast.</returns>
        public static RayCast ToRemote(this RayStep input, HitCollectionPolicy hitCollection = HitCollectionPolicy.AllHits, uint maxHits = uint.MaxValue, uint collisionMask = 0xFFFFFFFF)
        {
            input.ToRemote(out var output, hitCollection, maxHits, collisionMask);
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
        public static void ToRemote(this Ray input, out RayCast output, float distance, HitCollectionPolicy hitCollection = HitCollectionPolicy.AllHits, uint maxHits = uint.MaxValue, uint collisionMask = 0xFFFFFFFF)
        {
            input.Position.ToRemote(out Double3 starPos);
            input.GetPoint(distance, out var terminus);
            terminus.ToRemote(out Double3 endPos);
            output = new RayCast(starPos, endPos, hitCollection, maxHits, collisionMask);
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
        public static RayCast ToRemote(this Ray input, float distance, HitCollectionPolicy hitCollection = HitCollectionPolicy.AllHits, uint maxHits = uint.MaxValue, uint collisionMask = 0xFFFFFFFF)
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
