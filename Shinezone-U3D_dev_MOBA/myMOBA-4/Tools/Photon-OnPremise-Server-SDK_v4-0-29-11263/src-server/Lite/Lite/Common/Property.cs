// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Property.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   The property.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Lite.Common
{
    using System;

    /// <summary>
    /// The property.
    /// </summary>
    /// <typeparam name="TKey">
    /// The property key type.
    /// </typeparam>
    [Serializable]
    public class Property<TKey>
    {
        /// <summary>
        /// The value.
        /// </summary>
        private object value;

        /// <summary>
        /// Initializes a new instance of the <see cref="Property{TKey}"/> class.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        public Property(TKey key, object value)
        {
            this.Key = key;
            this.Value = value;
        }

        /// <summary>
        /// The property changed.
        /// </summary>
        public event EventHandler PropertyChanged;

        /// <summary>
        /// Gets Key.
        /// </summary>
        public TKey Key { get; private set; }

        /// <summary>
        /// Gets or sets Value.
        /// </summary>
        public object Value
        {
            get
            {
                return this.value;
            }

            set
            {
                if (this.value != value)
                {
                    this.value = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Invokes the <see cref="PropertyChanged"/> event. 
        /// </summary>
        private void RaisePropertyChanged()
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, EventArgs.Empty);
            }
        }
    }
}