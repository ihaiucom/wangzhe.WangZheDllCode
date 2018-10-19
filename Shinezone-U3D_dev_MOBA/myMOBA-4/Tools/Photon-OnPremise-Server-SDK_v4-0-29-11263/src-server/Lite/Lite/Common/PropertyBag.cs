// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PropertyBag.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   The property bag.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Lite.Common
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// The property bag.
    /// </summary>
    /// <typeparam name="TKey">
    /// The property key type
    /// </typeparam>
    [Serializable]
    public class PropertyBag<TKey>
    {
        /// <summary>
        /// The dictionary.
        /// </summary>
        private readonly Dictionary<TKey, Property<TKey>> dictionary;

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyBag{TKey}"/> class.
        /// </summary>
        public PropertyBag()
        {
            this.dictionary = new Dictionary<TKey, Property<TKey>>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyBag{TKey}"/> class.
        /// </summary>
        /// <param name="values">
        /// The values.
        /// </param>
        public PropertyBag(IEnumerable<KeyValuePair<TKey, object>> values)
            : this()
        {
            foreach (KeyValuePair<TKey, object> item in values)
            {
                this.Set(item.Key, item.Value);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyBag{TKey}"/> class.
        /// </summary>
        /// <param name="values">
        /// The values.
        /// </param>
        public PropertyBag(IDictionary values)
            : this()
        {
            foreach (TKey key in values.Keys)
            {
                this.Set(key, values[key]);
            }
        }

        /// <summary>
        /// The property changed event.
        /// </summary>
        public event EventHandler<PropertyChangedEventArgs<TKey>> PropertyChanged;

        /// <summary>
        /// Gets the number of properties in this instance.
        /// </summary>
        public int Count
        {
            get
            {
                return this.dictionary.Count;
            }
        }

        public IDictionary<TKey, Property<TKey>> AsDictionary()
        {
            return this.dictionary;
        }

        /// <summary>
        /// The get all.
        /// </summary>
        /// <returns>
        /// A list of all properties
        /// </returns>
        public IList<Property<TKey>> GetAll()
        {
            var properties = new Property<TKey>[this.dictionary.Count];
            this.dictionary.Values.CopyTo(properties, 0);
            return properties;
        }

        /// <summary>
        /// Get all properties.
        /// </summary>
        /// <returns>
        /// A copy of all properties with keys
        /// </returns>
        public Hashtable GetProperties()
        {
            var result = new Hashtable(this.dictionary.Count);
            this.CopyPropertiesToHashtable(result);
            return result;
        }

        /// <summary>
        /// The get properties.
        /// </summary>
        /// <param name="propertyKeys">
        /// The property keys.
        /// </param>
        /// <returns>
        /// The values for the given <paramref name="propertyKeys"/>
        /// </returns>
        public Hashtable GetProperties(IList<TKey> propertyKeys)
        {
            if (propertyKeys == null)
            {
                return this.GetProperties();
            }

            var result = new Hashtable(propertyKeys.Count);
            this.CopyPropertiesToHashtable(result, propertyKeys);
            return result;
        }

        /// <summary>
        /// The get properties.
        /// </summary>
        /// <param name="propertyKeys">
        /// The property keys.
        /// </param>
        /// <returns>
        /// The values for the given <paramref name="propertyKeys"/>
        /// </returns>
        public Hashtable GetProperties(IEnumerable<TKey> propertyKeys)
        {
            if (propertyKeys == null)
            {
                return this.GetProperties();
            }

            var result = new Hashtable();
            this.CopyPropertiesToHashtable(result, propertyKeys);
            return result;
        }

        /// <summary>
        /// The get properties.
        /// </summary>
        /// <param name="propertyKeys">
        /// The property keys.
        /// </param>
        /// <returns>
        /// The values for the given <paramref name="propertyKeys"/>
        /// </returns>
        public Hashtable GetProperties(IEnumerable propertyKeys)
        {
            if (propertyKeys == null)
            {
                return this.GetProperties();
            }

            var result = new Hashtable();
            this.CopyPropertiesToHashtable(result, propertyKeys);
            return result;
        }

        /// <summary>
        /// The get property.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The value for the <paramref name="key"/>.
        /// </returns>
        public Property<TKey> GetProperty(TKey key)
        {
            Property<TKey> value;
            this.dictionary.TryGetValue(key, out value);
            return value;
        }

        /// <summary>
        /// The set.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        public void Set(TKey key, object value)
        {
            Property<TKey> property;

            if (this.dictionary.TryGetValue(key, out property))
            {
                property.Value = value;
            }
            else
            {
                property = new Property<TKey>(key, value);
                property.PropertyChanged += this.OnPropertyPropertyChanged;
                this.dictionary.Add(key, property);
                this.RaisePropertyChanged(key, value);
            }
        }

        /// <summary>
        /// The set properties.
        /// </summary>
        /// <param name="values">
        /// The values.
        /// </param>
        public void SetProperties(IDictionary values)
        {
            foreach (TKey key in values.Keys)
            {
                this.Set(key, values[key]);
            }
        }

        /// <summary>
        /// The set properties.
        /// </summary>
        /// <param name="values">
        /// The values.
        /// </param>
        public void SetProperties(IDictionary<TKey, object> values)
        {
            foreach (KeyValuePair<TKey, object> keyValue in values)
            {
                this.Set(keyValue.Key, keyValue.Value);
            }
        }

        public bool TryGetValue(TKey key, out object value)
        {
            Property<TKey> property;
            if (this.dictionary.TryGetValue(key, out property))
            {
                value = property.Value;
                return true;
            }

            value = null;
            return false;
        }

        /// <summary>
        /// The copy properties to hashtable.
        /// </summary>
        /// <param name="hashtable">
        /// The hashtable.
        /// </param>
        private void CopyPropertiesToHashtable(IDictionary hashtable)
        {
            foreach (KeyValuePair<TKey, Property<TKey>> keyValue in this.dictionary)
            {
                hashtable.Add(keyValue.Key, keyValue.Value.Value);
            }
        }

        /// <summary>
        /// The copy properties to hashtable.
        /// </summary>
        /// <param name="hashtable">
        /// The hashtable.
        /// </param>
        /// <param name="propertyKeys">
        /// The property keys.
        /// </param>
        private void CopyPropertiesToHashtable(IDictionary hashtable, IEnumerable<TKey> propertyKeys)
        {
            foreach (TKey key in propertyKeys)
            {
                Property<TKey> property;
                if (this.dictionary.TryGetValue(key, out property))
                {
                    hashtable.Add(key, property.Value);
                }
            }
        }

        /// <summary>
        /// The copy properties to hashtable.
        /// </summary>
        /// <param name="hashtable">
        /// The hashtable.
        /// </param>
        /// <param name="propertyKeys">
        /// The property keys.
        /// </param>
        private void CopyPropertiesToHashtable(IDictionary hashtable, IEnumerable propertyKeys)
        {
            foreach (TKey key in propertyKeys)
            {
                Property<TKey> property;
                if (this.dictionary.TryGetValue(key, out property))
                {
                    hashtable.Add(key, property.Value);
                }
            }
        }

        /// <summary>
        /// The on property property changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void OnPropertyPropertyChanged(object sender, EventArgs e)
        {
            var property = (Property<TKey>)sender;
            this.RaisePropertyChanged(property.Key, property.Value);
        }

        /// <summary>
        /// The raise property changed.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        private void RaisePropertyChanged(TKey key, object value)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs<TKey>(key, value));
            }
        }
    }
}