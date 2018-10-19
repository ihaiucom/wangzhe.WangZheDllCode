// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Property.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Photon.Hive.Plugin
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
        #region Constants and Fields

        /// <summary>
        /// The value.
        /// </summary>
        private object value;

        #endregion

        #region Constructors and Destructors

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

        #endregion

        #region Events

        /// <summary>
        /// The property changed.
        /// </summary>
        public event EventHandler PropertyChanged;

        #endregion

        #region Properties

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

        #endregion

        #region Methods

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

        #endregion
    }
}