﻿// <auto-generated/>
#pragma warning disable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

using ObjectId = System.UInt32;

namespace Microsoft.Azure.RemoteRendering
{
    /// <summary>
    ///  Constants for remote rendering.
    /// </summary>
    public static class Constants
    {
        /// <summary>
        /// Invalid object id.
        /// </summary>
        public const ObjectId ObjectId_Invalid = 0xffffffff;

        public const int ApiPort = 50051;

        public static bool IsValid(float v)
        {
            return !(float.IsNaN(v) || float.IsInfinity(v));
        }
        public static bool IsValid(double v)
        {
            return !(double.IsNaN(v) || double.IsInfinity(v));
        }
    }

    public partial struct Float2
    {
        public Float2(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        public bool IsValid()
        {
            return Constants.IsValid(x) && Constants.IsValid(y);
        }
    }

    public partial struct Float3
    {
        public Float3(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public bool IsValid()
        {
            return Constants.IsValid(x) && Constants.IsValid(y) && Constants.IsValid(z);
        }
    }

    public partial struct Float4
    {
        public Float4(float x, float y, float z, float w)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }

        public bool IsValid()
        {
            return Constants.IsValid(x) && Constants.IsValid(y) && Constants.IsValid(z) && Constants.IsValid(w);
        }
    }

    public partial struct Double3
    {
        public Double3(double x, double y, double z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public bool IsValid()
        {
            return Constants.IsValid(x) && Constants.IsValid(y) && Constants.IsValid(z);
        }
    }

    public partial struct Color4
    {
        public Color4(float r, float g, float b, float a)
        {
            this.r = r;
            this.g = g;
            this.b = b;
            this.a = a;
        }

        public bool IsValid()
        {
            return Constants.IsValid(r) && Constants.IsValid(g) && Constants.IsValid(b) && Constants.IsValid(a);
        }

    }

    public partial struct Color4Ub
    {
        public Color4Ub(byte r, byte g, byte b, byte a)
        {
            this.bytes = 0;
            this.channels.r = r;
            this.channels.g = g;
            this.channels.b = b;
            this.channels.a = a;
        }

        public Color4Ub(int bytes)
        {
            this.channels.r = 0;
            this.channels.g = 0;
            this.channels.b = 0;
            this.channels.a = 0;
            this.bytes = bytes;
        }
    }


    public partial struct Quaternion
    {
        public Quaternion(float x, float y, float z, float w)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }

        public bool IsValid()
        {
            return Constants.IsValid(x) && Constants.IsValid(y) && Constants.IsValid(z) && Constants.IsValid(w);
        }
    }

    public partial struct LoadTextureFromSASParams
    {
        public LoadTextureFromSASParams(string url, TextureType type)
        {
            TextureUrl = url;
            TextureType = type;
        }
    }

    public partial struct LoadTextureParams
    {
        public LoadTextureParams(string storageContainer, string blobName, string assetPath, TextureType type)
        {
            Blob.AssetPath = assetPath;
            Blob.StorageAccountName = storageContainer;
            Blob.BlobContainerName = blobName;
            TextureType = type;
        }
    }

    public partial struct LoadModelFromSASParams
    {
        public LoadModelFromSASParams(string url, Entity parent = null)
        {
            ModelUrl = url;
            Parent = parent;
        }
    }

    public partial struct LoadModelParams
    {
        public LoadModelParams(string storageContainer, string blobName, string assetPath, Entity parent = null )
        {
            Blob.AssetPath = assetPath;
            Blob.StorageAccountName = storageContainer;
            Blob.BlobContainerName = blobName;
            Parent = parent;
        }        
    }


    public partial struct AABB3D
    {
        public AABB3D(Double3 minIn, Double3 maxIn)
        {
            this.min = minIn;
            this.max = maxIn;
        }

        public bool IsValid()
        {
            return min.IsValid() && max.IsValid() &&
                min.x <= max.x && min.y <= max.y && min.z <= max.z;
        }
    }
    public partial struct RayCast
    {
        // TODO: Put back in non-double3 typedef
        public RayCast(Double3 startPos, Double3 dir, double maxDistance, HitCollectionPolicy hitCollection = HitCollectionPolicy.AllHits, uint maxHits = uint.MaxValue, uint collisionMask = 0xFFFFFFFF)
        {
            this.StartPos = startPos;
            this.EndPos = new Double3(startPos.x + dir.x * maxDistance, startPos.y + dir.y * maxDistance, startPos.z + dir.z * maxDistance);
            this.HitCollection = hitCollection;
            this.MaxHits = maxHits;
            this.CollisionMask = collisionMask;
        }

        public RayCast(Double3 startPos, Double3 endPos, HitCollectionPolicy hitCollection = HitCollectionPolicy.AllHits, uint maxHits = uint.MaxValue, uint collisionMask = 0xFFFFFFFF)
        {
            this.StartPos = startPos;
            this.EndPos = endPos;
            this.HitCollection = hitCollection;
            this.MaxHits = maxHits;
            this.CollisionMask = collisionMask;
        }
    }


    public partial struct RayCastHit
    {
        public Entity HitEntity { get { return this.HitObject; } }
    }

    // Allow the frontend account info to be serialized
    [Serializable]
    public partial struct AzureFrontendAccountInfo
    {
        public AzureFrontendAccountInfo(
            string accountDomain = "mixedreality.azure.com",
            string accountId = "",
            string accountKey = "",
            string authenticationToken = "",
            string accessToken = ""
            )
        {
            AccountDomain = accountDomain;
            AccountId = accountId;
            AccountKey = accountKey;
            AuthenticationToken = authenticationToken;
            AccessToken = accessToken;
        }
    }

    public partial struct AssetConversionParams
    {
        public AssetConversionParams(string modelName, string modelUrl, string assetContainerUrl, string renderingSettings = "", string materialOverrides = "")
        {
            ModelName = modelName;
            ModelUrl = modelUrl;
            AssetContainerUrl = assetContainerUrl;
            RenderingSettings = renderingSettings;
            MaterialOverrides = materialOverrides;
        }
    }

    public partial struct AssetConversionInputParams
    {
        public AssetConversionInputParams(string storageAccountName, string blobContainerName, string folderPath, string inputAssetPath)
        {
            BlobContainerInformation.StorageAccountName = storageAccountName;
            BlobContainerInformation.BlobContainerName = blobContainerName;
            BlobContainerInformation.FolderPath = folderPath;
            InputAssetPath = inputAssetPath;
        }
    }

    public partial struct AssetConversionOutputParams
    {
        public AssetConversionOutputParams(string storageAccountName, string blobContainerName, string folderPath, string outputAssetPath)
        {
            BlobContainerInformation.StorageAccountName = storageAccountName;
            BlobContainerInformation.BlobContainerName = blobContainerName;
            BlobContainerInformation.FolderPath = folderPath;
            OutputAssetPath = outputAssetPath;
        }
    }

    public partial struct AssetConversionInputSasParams
    {
        public AssetConversionInputSasParams(string storageAccountName, string blobContainerName, string folderPath, string inputAssetPath, string containerReadSas)
        {
            BlobContainerInformation.StorageAccountName = storageAccountName;
            BlobContainerInformation.BlobContainerName = blobContainerName;
            BlobContainerInformation.FolderPath = folderPath;
            InputAssetPath = inputAssetPath;
            ContainerReadListSas = containerReadSas;
        }
    }

    public partial struct AssetConversionOutputSasParams
    {
        public AssetConversionOutputSasParams(string storageAccountName, string blobContainerName, string folderPath, string outputAssetPath, string containerWriteSas)
        {
            BlobContainerInformation.StorageAccountName = storageAccountName;
            BlobContainerInformation.BlobContainerName = blobContainerName;
            BlobContainerInformation.FolderPath = folderPath;
            OutputAssetPath = outputAssetPath;
            ContainerWriteSas = containerWriteSas;
        }
    }

    public partial struct RenderingSessionUpdateParams
    {
        public RenderingSessionUpdateParams(UInt32 maxLeaseTimeHours, UInt32 maxLeaseTimeMinutes = 0)
        {
            MaxLease.hour = (int)maxLeaseTimeHours;
            MaxLease.minute = (int)maxLeaseTimeMinutes;
            MaxLease.second = 0;
        }
    }

    public partial struct RenderingSessionCreationParamsUnsafe
    {
        public RenderingSessionCreationParamsUnsafe(string size, UInt32 maxLeaseTimeHours, UInt32 maxLeaseTimeMinutes = 0)
        {
            MaxLease.hour = (int)maxLeaseTimeHours;
            MaxLease.minute = (int)maxLeaseTimeMinutes;
            MaxLease.second = 0;
            Size = size;
        }
    }

    public partial struct RenderingSessionCreationParams
    {
        public RenderingSessionCreationParams(RenderingSessionVmSize size, UInt32 maxLeaseTimeHours, UInt32 maxLeaseTimeMinutes = 0)
        {
            MaxLease.hour = (int)maxLeaseTimeHours;
            MaxLease.minute = (int)maxLeaseTimeMinutes;
            MaxLease.second = 0;
            Size = size;
        }
    }

    public partial struct RenderingSessionProperties
    {
        RenderingSessionProperties(RenderingSessionStatus _Status = RenderingSessionStatus.Unknown, RenderingSessionVmSize _Size = RenderingSessionVmSize.None)
        {
            this.Status = _Status;
            this.Size = _Size;
            this.Hostname = "";
            this.Message = "";
            this.SizeString = "";
            this.Id = "";
            this.ElapsedTime.hour = 0;
            this.ElapsedTime.minute = 0;
            this.ElapsedTime.second = 0;
            this.MaxLease.hour = 0;
            this.MaxLease.minute = 0;
            this.MaxLease.second = 0;
        }

    }

    public partial struct ConnectToRuntimeParams
    {
        public ConnectToRuntimeParams(ServiceRenderMode modeIn = ServiceRenderMode.Default, bool ignoreCertsIn = false)
        {
            mode = modeIn;
            ignoreCertificateValidation = ignoreCertsIn;
        }
    }

    /// <summary>
    /// Disable the corresponding UI field when not in Simulation mode.
    /// </summary>
    [AttributeUsage(AttributeTargets.All)]
    public sealed class EnableInSimulationAttribute : Attribute
    {
    }

    /// <summary>
    /// Disable the corresponding UI field when in Simulation mode.
    /// </summary>
    [AttributeUsage(AttributeTargets.All)]
    public sealed class DisableInSimulationAttribute : Attribute
    {
    }
}
