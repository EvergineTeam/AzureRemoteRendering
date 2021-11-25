// Copyright © Wave Engine S.L. All rights reserved. Use is subject to license terms.

using Microsoft.Azure.RemoteRendering;
using Evergine.Framework;

namespace Evergine.AzureRemoteRendering.Components
{
    /// <summary>
    ///  Wraps a remote <see cref="MeshComponent"/> in a proxy <see cref="Component"/> for interoperability.
    ///
    /// <para>
    /// MeshComponents attach a Mesh to an Entity so that the mesh is rendered in the scene.
    /// Changing the material on the mesh component through SetMaterial will update the visual
    /// characteristics of the mesh.
    /// </para>
    /// </summary>
    public class ARRMeshComponent : ARRTypedComponentBase<MeshComponent>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ARRMeshComponent"/> class.
        /// </summary>
        public ARRMeshComponent()
            : base(ObjectType.MeshComponent)
        {
        }
    }
}
