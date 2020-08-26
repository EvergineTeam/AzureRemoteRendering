// Copyright © Wave Engine S.L. All rights reserved. Use is subject to license terms.

using System;
using System.Collections.Generic;
using WaveEngine.Framework;
using Microsoft.Azure.RemoteRendering;
using WaveEngine.AzureRemoteRendering;
using WaveEngine.Framework.Graphics;
using System.Diagnostics;
using ARREntity = Microsoft.Azure.RemoteRendering.Entity;
using WaveEntity = WaveEngine.Framework.Entity;

namespace WaveEngine.AzureRemoteRendering.Components
{
    /// <summary>
    /// Ensures that the hierarchy and a position of a proxy <see cref="WaveEntity"/> and a remote
    /// <see cref="ARREntity"/> correlate and provides methods for binding Wave Engine and remote content.
    /// <para>
    /// This class is automatically generated when a <see cref="ARREntity"/> is converted to a
    /// <see cref="WaveEntity"/> with the extension methods <see cref="EntityExtensions.CreateProxyEntity"/>
    /// and <see cref="EntityExtensions.FindOrCreateProxyEntity"/>.
    ///
    /// More advanced manipulation can be done with <see cref="Bind(ARREntity, bool)"/> and
    /// <see cref="CreateRemoteEntity"/>.
    /// </para>
    /// </summary>
    public class ARREntitySync : Behavior
    {
        private static Dictionary<uint, WeakReference<WaveEntity>> proxiesByRemoteId = new Dictionary<uint, WeakReference<WaveEntity>>();

        /// <summary>
        /// The <see cref="Transform3D"/> component dependency used to synchronize remote and
        /// proxy transform.
        /// </summary>
        [BindComponent]
        protected Transform3D transform;

        /// <summary>
        /// Gets the remote entity associated with this <see cref="ARREntitySync"/>.
        /// </summary>
        public ARREntity RemoteEntity { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the remote entity associated with this <see cref="ARREntitySync"/> is valid.
        /// </summary>
        public bool IsRemoteEntityValid => this.RemoteEntity != null && this.RemoteEntity.Valid;

        /// <summary>
        /// Gets or sets a value indicating whether to automatically sync this component every frame.
        /// If <see cref="SyncEveryFrame"/> is <c>false</c>, the user is expected to call <see cref="SyncToRemote"/>
        /// when the entity should be synchronized.
        /// <para>
        /// A user may want to set this to <c>false</c> to improve performance.
        /// </para>
        /// </summary>
        public bool SyncEveryFrame { get; set; } = false;

        /// <summary>
        /// Bind this proxy entity to an existing remote entity.
        /// <para>
        /// This function assumes the user will fully bind proxy entities for the remote entity parent,
        /// if a parent exists.
        ///
        /// Trying to bind an already bound remote entity will throw an exception.
        /// </para>
        /// </summary>
        /// <param name="remoteEntity">The entity to bind.</param>
        /// <param name="syncRemoteToLocal">
        /// Whether or not to the sync the proxy entity to the remote entity properties.
        /// </param>
        public void Bind(ARREntity remoteEntity, bool syncRemoteToLocal)
        {
            if (this.IsRemoteEntityValid &&
                !this.RemoteEntity.Equals(remoteEntity))
            {
                throw new InvalidOperationException($"Trying to bind an already bound {nameof(ARREntitySync)}");
            }
            else if (!this.IsAttached)
            {
                throw new InvalidOperationException($"{nameof(ARREntitySync)} is not attached.");
            }
            else
            {
                if (!this.IsRemoteEntityValid)
                {
                    BindToLocalEntity(remoteEntity, this.Owner);
                    this.RemoteEntity = remoteEntity;
                }

                if (syncRemoteToLocal)
                {
                    this.SyncToLocal();
                }
            }
        }

        /// <summary>
        /// Unbind proxy <see cref="WaveEntity"/> from remote <see cref="ARREntity"/>.
        /// This enables destruction of proxy entities without destroying the remote entities.
        /// </summary>
        /// <param name="recursive">
        /// Call unbind recursively on children.
        /// Without unbinding children destroying proxy root <see cref="WaveEntity"/>
        /// will still destroy child remote entities bound to the child proxy entities.
        /// </param>
        public void Unbind(bool recursive = true)
        {
            if (this.RemoteEntity != null)
            {
                UnbindFromLocalEntity(this.RemoteEntity);
                this.RemoteEntity = null;
            }

            // Unbind components as well so they don't get destroyed with the proxy Entity
            foreach (var comp in this.Owner.FindComponents<ARRComponentBase>())
            {
                comp.Unbind();
            }

            if (recursive)
            {
                foreach (var child in this.Owner.ChildEntities)
                {
                    var sync = child.FindComponent<ARREntitySync>();
                    if (sync != null)
                    {
                        sync.Unbind(true);
                    }
                }
            }
        }

        /// <summary>
        /// Create a new remote <see cref="ARREntity"/> to associate with the proxy <see cref="WaveEntity"/>.
        /// <para>
        /// This function will throw if this <see cref="ARREntitySync"/> already has a remote entity.
        /// </para>
        /// </summary>
        /// <param name="session">
        /// The azure session used to create the remote <see cref="ARREntity"/>.
        /// </param>
        public void CreateRemoteEntity(AzureSession session)
        {
            if (session is null)
            {
                throw new ArgumentNullException(nameof(session));
            }

            if (session.ConnectionStatus != ConnectionStatus.Connected)
            {
                return;
            }

            if (!this.IsRemoteEntityValid)
            {
                this.RemoteEntity = session.Actions.CreateEntity();
            }
            else
            {
                throw new Exception($"Trying to create an already created {nameof(ARREntitySync)}");
            }

            if (this.IsRemoteEntityValid)
            {
                BindToLocalEntity(this.RemoteEntity, this.Owner);
                this.SyncToRemote();
            }
        }

        /// <inheritdoc />
        protected override void Update(TimeSpan gameTime)
        {
            if (this.SyncEveryFrame)
            {
                this.SyncToRemote();
            }
        }

        /// <inheritdoc />
        protected override void OnDestroy()
        {
            if (this.IsRemoteEntityValid)
            {
                this.RemoteEntity.Destroy();
            }
        }

        /// <summary>
        /// Synchronize local proxy <see cref="WaveEntity"/> transform and name
        /// with the remote <see cref="ARREntity"/>.
        /// </summary>
        public void SyncToRemote()
        {
            if (this.IsRemoteEntityValid)
            {
                ARREntity currentRemoteParent = null;
                bool useGlobalTransform = false;
                var parent = this.Owner.Parent;
                if (parent != null)
                {
                    var parentSync = parent.FindComponent<ARREntitySync>();
                    if (parentSync != null && parentSync.IsRemoteEntityValid)
                    {
                        currentRemoteParent = parentSync.RemoteEntity;
                    }
                    else
                    {
                        // Parent exists but is not a remote entity, compute global transform.
                        useGlobalTransform = true;
                    }
                }

                if (!object.Equals(this.RemoteEntity.Parent, currentRemoteParent))
                {
                    // Apply re-parent of entity.
                    this.RemoteEntity.Parent = currentRemoteParent;
                }

                if (useGlobalTransform)
                {
                    this.RemoteEntity.Position = this.transform.Position.ToRemoteDouble();
                    this.RemoteEntity.Rotation = this.transform.Orientation.ToRemote();
                    this.RemoteEntity.Scale = this.transform.Scale.ToRemoteFloat();
                }
                else
                {
                    this.RemoteEntity.Position = this.transform.LocalPosition.ToRemoteDouble();
                    this.RemoteEntity.Rotation = this.transform.LocalOrientation.ToRemote();
                    this.RemoteEntity.Scale = this.transform.LocalScale.ToRemoteFloat();
                }

                this.RemoteEntity.Name = this.Owner.Name;
            }
        }

        /// <summary>
        /// Synchronize remote <see cref="ARREntity"/> transform and <see cref="WaveEntity.Name"/> with the local
        /// proxy <see cref="WaveEntity"/>.
        /// </summary>
        private void SyncToLocal()
        {
            if (this.IsRemoteEntityValid &&
                this.transform != null)
            {
                this.transform.LocalPosition = this.RemoteEntity.Position.ToWave();
                this.transform.LocalOrientation = this.RemoteEntity.Rotation.ToWave();
                this.transform.LocalScale = this.RemoteEntity.Scale.ToWave();
                this.Owner.Name = this.RemoteEntity.Name;
            }
        }

        /// <summary>
        /// Tries to find an existing <see cref="ARREntitySync"/> proxy component associated
        /// with remote <see cref="ARREntity"/>.
        /// </summary>
        /// <param name="remoteEntity">The remote <see cref="ARREntity"/>.</param>
        /// <param name="syncComponent">The associated <see cref="ARREntitySync"/> proxy component.</param>
        /// <returns>
        /// <c>true</c> if an associated <see cref="ARREntitySync"/> proxy component is found;
        /// otherwise, <c>false</c>.
        /// </returns>
        public static bool TryGetSyncComponent(ARREntity remoteEntity, out ARREntitySync syncComponent)
        {
            if (proxiesByRemoteId.TryGetValue(remoteEntity.InteropId, out var weak) &&
                weak.TryGetTarget(out var localEntity) &&
                localEntity != null)
            {
                // Proxy local entity is alive and well.
                syncComponent = localEntity.FindComponent<ARREntitySync>();
            }
            else
            {
                syncComponent = null;
            }

            if (syncComponent == null)
            {
                // If the proxy entity was cached, it's either been garbage-collected, destroyed,
                // or its sync component has been destroyed, so we make sure to remove its cache entry.
                proxiesByRemoteId.Remove(remoteEntity.InteropId);
            }

            return syncComponent != null;
        }

        private static void BindToLocalEntity(ARREntity entity, WaveEntity proxyEntity)
        {
            if (!entity.Valid)
            {
                throw new Exception("Invalid remote entity passed to BindToLocalEntity");
            }

            if (TryGetSyncComponent(entity, out var target))
            {
                Debug.WriteLine($"{nameof(ARREntity)} (name:{entity.Name}, id:{entity.InteropId}) already bound to Entity, {nameof(BindToLocalEntity)} has failed");
                throw new Exception($"{nameof(ARREntity)} bound twice to Entities");
            }

            proxiesByRemoteId.Add(entity.InteropId, new WeakReference<WaveEntity>(proxyEntity));
        }

        private static void UnbindFromLocalEntity(ARREntity entity)
        {
            if (!proxiesByRemoteId.Remove(entity.InteropId))
            {
                throw new Exception($"{nameof(ARREntity)} passed to {nameof(UnbindFromLocalEntity)}, but no mapping exists.");
            }
        }
    }
}
