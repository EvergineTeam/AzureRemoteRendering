// Copyright © Evergine S.L. All rights reserved. Use is subject to license terms.

using System;
using Microsoft.Azure.RemoteRendering;
using Evergine.AzureRemoteRendering.Components;
using Evergine.Framework.Graphics;
using Evergine.Framework.Managers;
using ARREntity = Microsoft.Azure.RemoteRendering.Entity;
using EvergineEntity = Evergine.Framework.Entity;

namespace Evergine.AzureRemoteRendering
{
    /// <summary>
    /// Extensions methods that simplifies the creation of remote entities and components.
    /// </summary>
    public static class EntityExtensions
    {
        /// <summary>
        /// Finds a proxy <see cref="EvergineEntity"/> for a remote <see cref="ARREntity"/>.
        /// If no proxy entity already exists, a new one will be created.
        /// </summary>
        /// <param name="entityManager">The entity manager where the proxy entity is included.</param>
        /// <param name="remoteEntity">The remote entity.</param>
        /// <param name="mode">Whether the proxy components will be created.</param>
        /// <param name="recursive">Whether to create proxy entities for children of the remote entity.</param>
        /// <returns>A proxy <see cref="EvergineEntity"/> for a remote <see cref="ARREntity"/>.</returns>
        public static EvergineEntity FindOrCreateProxyEntity(this EntityManager entityManager, ARREntity remoteEntity, ARRCreationMode mode, bool recursive = false)
        {
            var proxyEntity = remoteEntity.GetExistingProxyEntity();
            if (proxyEntity == null)
            {
                proxyEntity = entityManager.CreateProxyEntity(remoteEntity, mode, false);
            }
            else if (mode == ARRCreationMode.CreateProxyComponents)
            {
                proxyEntity.CreateARRComponentsFromRemoteEntity(remoteEntity);
            }

            if (recursive)
            {
                foreach (var remoteChild in remoteEntity.Children)
                {
                    entityManager.FindOrCreateProxyEntity(remoteChild, mode, true);
                }
            }

            return proxyEntity;
        }

        /// <summary>
        /// Create a proxy <see cref="EvergineEntity"/> for a remote <see cref="ARREntity"/>.
        /// <para>
        ///
        /// When created, the path from the remote <see cref="ARREntity"/> to the remote scene root will have
        /// proxy entities created for it.
        /// These proxy entities must be created in order to appropriately set the
        /// <see cref="EvergineEntity.Parent"/> for the proxy entity.
        ///
        /// As a side effect, this means that, given a Remote hierarchy:
        ///
        /// ARR.Parent
        ///     ARR.Child
        ///
        /// Calling <see cref="CreateProxyEntity"/> on ARR.Child will also create a proxy entity for ARR.Parent.
        /// </para>
        /// </summary>
        /// <param name="entityManager">The entity manager where the proxy entity is included.</param>
        /// <param name="remoteEntity">The remote entity.</param>
        /// <param name="mode">Whether the proxy components will be created.</param>
        /// <param name="recursive">Whether to create proxy entities for children of the remote entity.</param>
        /// <returns>A proxy <see cref="EvergineEntity"/> for a remote <see cref="ARREntity"/>.</returns>
        public static EvergineEntity CreateProxyEntity(this EntityManager entityManager, ARREntity remoteEntity, ARRCreationMode mode, bool recursive = false)
        {
            if (!remoteEntity.Valid)
            {
                return null;
            }

            if (ARREntitySync.TryGetSyncComponent(remoteEntity, out _))
            {
                throw new Exception("A proxy entity for this remote entity already exists!");
            }

            var proxyEntity = new EvergineEntity();
            proxyEntity.AddComponent(new Transform3D());

            var remoteParentEntity = remoteEntity.Parent;
            if (remoteParentEntity != null)
            {
                var proxyParentEntity = GetExistingProxyEntity(remoteParentEntity);
                if (proxyParentEntity == null)
                {
                    proxyParentEntity = entityManager.CreateProxyEntity(remoteParentEntity, ARRCreationMode.DoNotCreateProxyComponents, false);
                }

                proxyParentEntity.AddChild(proxyEntity);
            }
            else
            {
                entityManager.Add(proxyEntity);
            }

            var sync = proxyEntity.FindComponent<ARREntitySync>();
            if (sync == null)
            {
                sync = new ARREntitySync();
                proxyEntity.AddComponent(sync);
            }

            sync.Bind(remoteEntity, true);

            if (mode == ARRCreationMode.CreateProxyComponents)
            {
                proxyEntity.CreateARRComponentsFromRemoteEntity(remoteEntity);
            }

            if (recursive)
            {
                foreach (var remoteChild in remoteEntity.Children)
                {
                    entityManager.CreateProxyEntity(remoteChild, mode, true);
                }
            }

            return proxyEntity;
        }

        /// <summary>
        /// Removes the proxy entity bound to this remote entity without destroying the remote entity.
        /// </summary>
        /// <param name="entityManager">The entity manager where the proxy entity is included.</param>
        /// <param name="remoteEntity">The remote entity.</param>
        /// <param name="removeFlags">Flags to indicate how to proceed with parent entities of the proxy entity.</param>
        public static void RemoveProxyEntity(this EntityManager entityManager, ARREntity remoteEntity, ARRRemoveProxyEntityFlags removeFlags)
        {
            var proxyEntity = remoteEntity.GetExistingProxyEntity();
            if (proxyEntity == null)
            {
                return;
            }

            ARREntitySync sync;
            EvergineEntity previous = null;
            if (removeFlags.HasFlag(ARRRemoveProxyEntityFlags.DestroyEmptyParents))
            {
                sync = proxyEntity.FindComponent<ARREntitySync>();
                while (proxyEntity.Parent?.NumChildren == 1)
                {
                    previous = proxyEntity;
                    proxyEntity = proxyEntity.Parent;

                    // Unbind on our way up the hierarchy for performance, keep the last sync bound in case keepRemoteRoot is true.
                    var syncNext = proxyEntity.FindComponent<ARREntitySync>();
                    if (syncNext == null)
                    {
                        break;
                    }

                    // Unbind first entity recursively since it doesn't need to be leaf
                    sync.Unbind(sync.RemoteEntity == remoteEntity);
                    sync = syncNext;
                }
            }

            if (removeFlags.HasFlag(ARRRemoveProxyEntityFlags.KeepRemoteRoot))
            {
                proxyEntity = previous;
            }
            else
            {
                proxyEntity?.FindComponent<ARREntitySync>().Unbind(true);
            }

            if (proxyEntity != null)
            {
                entityManager.Remove(proxyEntity);
            }
        }

        /// <summary>
        /// Get an existing proxy <see cref="EvergineEntity"/> for an remote <see cref="ARREntity"/>.
        /// If no proxy entity has been mapped to this remote entity then null.
        /// </summary>
        /// <param name="remoteEntity">The remote <see cref="ARREntity"/>.</param>
        /// <returns>An proxy <see cref="EvergineEntity"/> if exists; otherwise, <c>null</c>.</returns>
        public static EvergineEntity GetExistingProxyEntity(this ARREntity remoteEntity)
        {
            if (ARREntitySync.TryGetSyncComponent(remoteEntity, out var syncComponent))
            {
                return syncComponent.Owner;
            }

            return null;
        }

        /// <summary>
        /// Create proxy components for the proxy entity for all ARR remote Components on the remote entity.
        /// </summary>
        /// <param name="proxyEntity">The local proxy <see cref="EvergineEntity"/>.</param>
        /// <param name="remoteEntity">The remote <see cref="ARREntity"/>.</param>
        public static void CreateARRComponentsFromRemoteEntity(this EvergineEntity proxyEntity, ARREntity remoteEntity)
        {
            foreach (var comp in remoteEntity.Components)
            {
                switch (comp.Type)
                {
                    case ObjectType.MeshComponent:
                        proxyEntity.BindARRComponent<ARRMeshComponent>(comp);
                        break;
                    case ObjectType.CutPlaneComponent:
                        proxyEntity.BindARRComponent<ARRCutPlaneComponent>(comp);
                        break;
                    case ObjectType.HierarchicalStateOverrideComponent:
                        proxyEntity.BindARRComponent<ARRHierarchicalStateOverrideComponent>(comp);
                        break;
                    case ObjectType.PointLightComponent:
                        proxyEntity.BindARRComponent<ARRPointLightComponent>(comp);
                        break;
                    case ObjectType.SpotLightComponent:
                        proxyEntity.BindARRComponent<ARRSpotLightComponent>(comp);
                        break;
                    case ObjectType.DirectionalLightComponent:
                        proxyEntity.BindARRComponent<ARRDirectionalLightComponent>(comp);
                        break;
                    default:
                        throw new InvalidOperationException("Unrecognized component type in ARR to Evergine translation.");
                }
            }
        }

        /// <summary>
        /// Initialize an ARR proxy binding with an already existing ARR Component.
        /// </summary>
        /// <typeparam name="TRemoteComponent">The type of the ARR component.</typeparam>
        /// <param name="proxyEntity">The local proxy <see cref="EvergineEntity"/>.</param>
        /// <param name="remoteComponent">The remote component instance to be binded.</param>
        /// <returns>
        /// <c>true</c> if the remote component is successfully binded to the proxy entity; otherwise, <c>false</c>.
        /// </returns>
        public static bool BindARRComponent<TRemoteComponent>(this EvergineEntity proxyEntity, ComponentBase remoteComponent)
            where TRemoteComponent : ARRComponentBase, new()
        {
            var proxyComponent = proxyEntity.FindComponent<TRemoteComponent>();
            if (proxyComponent != null)
            {
                return proxyComponent.Bind(remoteComponent);
            }
            else
            {
                proxyComponent = new TRemoteComponent() { BindMode = ARRComponentBindMode.FromRemoteToProxy };
                if (proxyComponent.Bind(remoteComponent))
                {
                    proxyEntity.AddComponent(proxyComponent);
                    return true;
                }
            }

            return false;
        }
    }
}
