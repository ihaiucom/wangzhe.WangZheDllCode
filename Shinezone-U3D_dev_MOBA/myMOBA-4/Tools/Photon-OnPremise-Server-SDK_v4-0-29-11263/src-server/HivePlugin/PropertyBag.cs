// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PropertyBag.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Photon.Hive.Plugin
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
        #region Constants and Fields

        /// <summary>
        /// The dictionary.
        /// </summary>
        private readonly Dictionary<TKey, Property<TKey>> dictionary;

        #endregion

        #region Constructors and Destructors

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

        #endregion

        #region Events

        /// <summary>
        /// The property changed event.
        /// </summary>
        public event EventHandler<PropertyChangedEventArgs<TKey>> PropertyChanged;

        #endregion

        #region Properties

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

        #endregion

        #region Public Methods

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
        ///     The values.
        /// </param>
        /// <param name="expectedValues">
        ///     The expected values for properties in order to apply CAS.
        /// </param>
        /// <param name="debugMessage"></param>
        public bool SetPropertiesCAS(IDictionary values, IDictionary expectedValues, out string debugMessage)
        {
            debugMessage = string.Empty;
            if (expectedValues == null || expectedValues.Count == 0)
            {
                this.SetProperties(values);
                return true;
            }

            foreach (System.Collections.DictionaryEntry expectedValue in expectedValues)
            {
                var property = this.GetProperty((TKey)expectedValue.Key);
                if (property == null || !this.CompareValues(property.Value, expectedValue.Value))
                {
                    MakeCASDebugMessage(out debugMessage, property, new KeyValuePair<TKey, object>((TKey)expectedValue.Key, expectedValue.Value));
                    return false;
                }
            }

            this.SetProperties(values);
            return true;
        }

        private bool CompareValues(object value, object expectedValue)
        {
            var arr = value as IList;
            if (arr != null)
            {
                var arr2 = expectedValue as IList;
                if (arr2 == null)
                {
                    return false;
                }

                if (arr.Count != arr2.Count)
                {
                    return false;
                }

                for (var i = 0; i < arr.Count; ++i)
                {
                    if (!arr[i].Equals(arr2[i]))
                    {
                        return false;
                    }
                }
                return true;
            }

            if (value == null)
            {
                return expectedValue == null;
            }

            return value.Equals(expectedValue);
        }

        /// <summary>
        /// The set properties.
        /// </summary>
        /// <param name="values">
        /// The values.
        /// </param>
        public void SetProperties(IDictionary<TKey, object> values)
        {
            foreach (var keyValue in values)
            {
                this.Set(keyValue.Key, keyValue.Value);
            }
        }

        /// <summary>
        /// The set properties.
        /// </summary>
        /// <param name="values">
        /// The values.
        /// </param>
        /// <param name="expectedValues">
        /// expected values for properties, which we are going to change
        /// </param>
        /// <param name="debugMessage"></param>
        public bool SetPropertiesCAS(IDictionary<TKey, object> values, IDictionary<TKey, object> expectedValues, out string debugMessage)
        {
            debugMessage = string.Empty;
            if (expectedValues == null || expectedValues.Count == 0)
            {
                this.SetProperties(values);
                return true;
            }

            foreach (var expectedValue in expectedValues)
            {
                var property = this.GetProperty(expectedValue.Key);
                if (property == null || !object.Equals(property.Value, expectedValue.Value))
                {
                    MakeCASDebugMessage(out debugMessage, property, expectedValue);
                    return false;
                }
            }

            this.SetProperties(values);
            return true;
        }

        private static void MakeCASDebugMessage(out string debugMessage, Property<TKey> property, KeyValuePair<TKey, object> expectedValue)
        {
            if (property != null)
            {
                debugMessage = string.Format("CAS update failed: property='{0}' has value='{1}'", expectedValue.Key, property.Value);
            }
            else
            {
                debugMessage = string.Format("CAS update failed: there is no property='{0}' on server", expectedValue.Key);
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

        #endregion

        #region Methods

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

        #endregion
    }
}