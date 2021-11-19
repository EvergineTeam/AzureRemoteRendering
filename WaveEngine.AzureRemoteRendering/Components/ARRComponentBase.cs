// Copyright © Wave Engine S.L. All rights reserved. Use is subject to license terms.

using Microsoft.Azure.RemoteRendering;
using System;
using System.Diagnostics;
using WaveEngine.Framework;

namespace WaveEngine.AzureRemoteRendering.Components
{
    /// <summary>
    /// Base class for all ARR Components that have been wrapped in a proxy <see cref="Component"/>
    /// for interoperability.
    /// </summary>
    public class ARRComponentBase : Component
    {
        /// <summary>
        /// The <see cref="AzureRemoteRenderingService"/> dependency used by this component.
        /// </summary>
        [BindService]
        protected AzureRemoteRenderingService arrService;

        /// <summary>
        /// The <see cref="ARREntitySync"/> component dependency that contains the remote entity
        /// containing the <see cref="RemoteComponent"/>.
        /// </summary>
        [BindComponent]
        protected ARREntitySync arrEntitySync;

        private ObjectType objectType;

        private ARRComponentBindMode bindMode;

        /// <summary>
        /// Gets or sets a value indicating whether the remote component will be created and binded automatically
        /// by the proxy component.
        /// </summary>
        public ARRComponentBindMode BindMode
        {
            get => this.bindMode;
            set
            {
                this.bindMode = value;

                if (this.IsActivated)
                {
                    this.TryCreateFromProxy();
                }
            }
        }

        /// <summary>
        /// Gets the binded remote component instance.
        /// </summary>
        public ComponentBase RemoteComponent
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="RemoteComponent"/> in binded and valid.
        /// </summary>
        public bool IsComponentValid => this.RemoteComponent?.Valid == true;

        /// <summary>
        /// Occurs when <see cref="RemoteComponent"/> is changed.
        /// </summary>
        public event EventHandler RemoteComponentBinded;

        /// <summary>
        /// Initializes a new instance of the <see cref="ARRComponentBase"/> class.
        /// </summary>
        /// <param name="type">The ARR component type.</param>
        protected ARRComponentBase(ObjectType type)
        {
            if (type <= ObjectType.FirstComponent &&
               type >= ObjectType.LastComponent)
            {
                throw new InvalidOperationException($"The type {type} is not a valid ARR component type.");
            }

            this.objectType = type;
        }

        /// <inheritdoc />
        protected override void OnActivated()
        {
            base.OnActivated();

            this.arrService.ConnectionStatusChanged += this.ARRService_ConnectionStatusChanged;
            this.ARRService_ConnectionStatusChanged(this, this.arrService.ConnectionStatus);

            if (this.IsComponentValid)
            {
                this.RemoteComponent.Enabled = true;
            }
        }

        /// <inheritdoc />
        protected override void OnDeactivated()
        {
            base.OnDeactivated();

            this.arrService.ConnectionStatusChanged -= this.ARRService_ConnectionStatusChanged;

            if (this.IsComponentValid)
            {
                this.RemoteComponent.Enabled = false;
            }
        }

        /// <inheritdoc />
        protected override void OnDestroy()
        {
            base.OnDestroy();

            if (this.IsComponentValid)
            {
                this.RemoteComponent.Destroy();
                this.RemoteComponent = null;
            }
        }

        private void ARRService_ConnectionStatusChanged(object sender, ARRConnectionStatus status)
        {
            if (status == ARRConnectionStatus.Connected)
            {
                this.TryCreateFromProxy();
            }
            else
            {
                this.Unbind();
            }
        }

        private void TryCreateFromProxy()
        {
            if (this.bindMode == ARRComponentBindMode.FromProxyToRemote &&
                this.IsAttached &&
                this.RemoteComponent == null)
            {
                this.CreateRemoteComponent();
            }
        }

        /// <summary>
        /// Invoked when the remote component is binded.
        /// </summary>
        protected virtual void OnRemoteComponentBinded()
        {
            if (this.bindMode == ARRComponentBindMode.FromProxyToRemote)
            {
                this.OnRemoteComponentCreated();
            }

            this.RemoteComponentBinded?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Invoked when the remote component is created when using ProxyToRemote creationMode.
        /// </summary>
        protected virtual void OnRemoteComponentCreated()
        {
        }

        /// <summary>
        /// Binds to a remote component.
        /// </summary>
        /// <param name="component">The remote component instance to be binded.</param>
        /// <returns>
        /// <c>true</c> if the remote component is successfully binded to the proxy component; otherwise, <c>false</c>.
        /// </returns>
        public virtual bool Bind(ComponentBase component)
        {
            if (component?.Valid != true)
            {
                Debug.WriteLine($"Invalid component passed to {nameof(ARRComponentBase)}.{nameof(this.Bind)}!");
                return false;
            }

            if (component.Type != this.objectType)
            {
                Debug.WriteLine($"Invalid type passed to {nameof(ARRComponentBase)}.{nameof(this.Bind)}!");
                return false;
            }

            if (this.IsComponentValid && !this.RemoteComponent.Equals(component))
            {
                Debug.WriteLine("Initializing component twice!");
                return false;
            }

            this.RemoteComponent = component;
            this.IsEnabled = this.RemoteComponent.Enabled;
            this.OnRemoteComponentBinded();

            return true;
        }

        /// <summary>
        /// Unbinds from remote component.
        /// This enables destruction of the proxy <see cref="Component"/> without destroying
        /// the <see cref="RemoteComponent"/>.
        /// </summary>
        public virtual void Unbind()
        {
            this.RemoteComponent = null;
        }

        /// <summary>
        /// Creates and binds to a new remote component of the same type.
        /// </summary>
        /// <returns>
        /// <c>true</c> if the remote component is successfully created; otherwise, <c>false</c>.
        /// </returns>
        public virtual bool CreateRemoteComponent()
        {
            if (!this.IsAttached)
            {
                throw new InvalidOperationException("Component not attached");
            }

            if (this.IsComponentValid)
            {
                Debug.WriteLine("{0}", "Initializing component twice!");
                return false;
            }

            if (!this.arrService.IsCurrentSessionConnected)
            {
                return false;
            }

            var session = this.arrService.CurrentSession;
            if (!this.arrEntitySync.IsRemoteEntityValid)
            {
                this.arrEntitySync.CreateRemoteEntity(session);
            }

            var component = session.Connection.CreateComponent(this.objectType, this.arrEntitySync.RemoteEntity);
            this.Bind(component);

            return this.IsComponentValid;
        }
    }
}
