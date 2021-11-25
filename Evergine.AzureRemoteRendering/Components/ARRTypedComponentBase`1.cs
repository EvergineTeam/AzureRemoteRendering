// Copyright © Wave Engine S.L. All rights reserved. Use is subject to license terms.

using Microsoft.Azure.RemoteRendering;
using Evergine.Framework;
using System;

namespace Evergine.AzureRemoteRendering.Components
{
    /// <summary>
    /// Base class for all typed ARR Components that have been wrapped in a proxy <see cref="Component"/>.
    /// </summary>
    /// <typeparam name="TRemoteComponent">The ARR remote component type.</typeparam>
    public abstract class ARRTypedComponentBase<TRemoteComponent> : ARRComponentBase
    where TRemoteComponent : ComponentBase
    {
        /// <summary>
        /// Gets the binded remote component instance.
        /// </summary>
        public new TRemoteComponent RemoteComponent => base.RemoteComponent as TRemoteComponent;

        /// <summary>
        /// Initializes a new instance of the <see cref="ARRTypedComponentBase{TRemoteComponent}"/> class.
        /// </summary>
        /// <param name="type">The ARR component type.</param>
        protected ARRTypedComponentBase(ObjectType type)
            : base(type)
        {
        }

        /// <summary>
        /// Internal method used to simplify the proxy getter properties binded to remote component properties.
        /// </summary>
        /// <typeparam name="TLocal">The type of the local proxy property.</typeparam>
        /// <param name="overrideValue">The current override value set by the component.</param>
        /// <param name="getRemoteValue">A method that returns the current value of the remote property.</param>
        /// <returns>The current value of the property.</returns>
        protected TLocal GetProperty<TLocal>(TLocal overrideValue, Func<TLocal> getRemoteValue)
            where TLocal : class
        {
            if (overrideValue != null)
            {
                return overrideValue;
            }

            TLocal value = null;
            if (this.IsComponentValid)
            {
                value = getRemoteValue();
            }

            return value;
        }

        /// <summary>
        /// Internal method used to simplify the proxy getter properties binded to remote component properties.
        /// </summary>
        /// <typeparam name="TLocal">The type of the local proxy property.</typeparam>
        /// <param name="overrideValue">The current override value set by the component.</param>
        /// <param name="defaultValue">The default value of the property set by the ARR API.</param>
        /// <param name="getRemoteValue">A method that returns the current value of the remote property.</param>
        /// <returns>The current value of the property.</returns>
        protected TLocal GetProperty<TLocal>(TLocal? overrideValue, TLocal defaultValue, Func<TLocal> getRemoteValue)
            where TLocal : struct
        {
            if (overrideValue.HasValue)
            {
                return overrideValue.Value;
            }

            var value = defaultValue;
            if (this.IsComponentValid)
            {
                value = getRemoteValue();
            }

            return value;
        }

        /// <summary>
        /// Internal method used to simplify the proxy setter properties binded to remote component properties.
        /// </summary>
        /// <typeparam name="TLocal">The type of the local proxy property.</typeparam>
        /// <param name="value">The new value for the property.</param>
        /// <param name="overrideValue">The current override value set by the component.</param>
        /// <param name="setRemoteValue">A method that sets a new value on the remote property.</param>
        protected void SetProperty<TLocal>(TLocal value, ref TLocal overrideValue, Action<TLocal> setRemoteValue)
            where TLocal : class
        {
            overrideValue = null;
            if (this.IsComponentValid)
            {
                setRemoteValue(value);
            }
            else
            {
                overrideValue = value;
            }
        }

        /// <summary>
        /// Internal method used to simplify the proxy setter properties binded to remote component properties.
        /// </summary>
        /// <typeparam name="TLocal">The type of the local proxy property.</typeparam>
        /// <param name="value">The new value for the property.</param>
        /// <param name="overrideValue">The current override value set by the component.</param>
        /// <param name="defaultValue">The default value of the property set by the ARR API.</param>
        /// <param name="setRemoteValue">A method that sets a new value on the remote property.</param>
        protected void SetProperty<TLocal>(TLocal value, ref TLocal? overrideValue, TLocal defaultValue, Action<TLocal> setRemoteValue)
            where TLocal : struct
        {
            overrideValue = null;
            if (this.IsComponentValid)
            {
                setRemoteValue(value);
            }
            else if (!value.Equals(defaultValue))
            {
                overrideValue = value;
            }
        }

        /// <summary>
        /// Internal method used override remote component properties when the remote component is created
        /// using <see cref="ARRComponentBindMode.FromProxyToRemote"/>.
        /// </summary>
        /// <typeparam name="TLocal">The type of the local proxy property.</typeparam>
        /// <param name="overrideValue">The current override value set by the component.</param>
        /// <param name="proxyPropertySetter">A method that sets a new value on the local proxy property.</param>
        protected void OverrideRemoteProperty<TLocal>(TLocal overrideValue, Action<TLocal> proxyPropertySetter)
            where TLocal : class
        {
            if (overrideValue != null)
            {
                proxyPropertySetter(overrideValue);
            }
        }

        /// <summary>
        /// Internal method used override remote component properties when the remote component is created
        /// using <see cref="ARRComponentBindMode.FromProxyToRemote"/>.
        /// </summary>
        /// <typeparam name="TLocal">The type of the local proxy property.</typeparam>
        /// <param name="overrideValue">The current override value set by the component.</param>
        /// <param name="proxyPropertySetter">A method that sets a new value on the local proxy property.</param>
        protected void OverrideRemoteProperty<TLocal>(TLocal? overrideValue, Action<TLocal> proxyPropertySetter)
            where TLocal : struct
        {
            if (overrideValue.HasValue)
            {
                proxyPropertySetter(overrideValue.Value);
            }
        }
    }
}
