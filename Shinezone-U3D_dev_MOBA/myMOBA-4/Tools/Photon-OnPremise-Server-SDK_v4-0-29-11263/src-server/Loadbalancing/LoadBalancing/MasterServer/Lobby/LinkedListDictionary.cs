// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LinkedListDictionary.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   Defines the LinkedListDictionary type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Photon.LoadBalancing.MasterServer.Lobby
{
    #region using directives

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;

    #endregion

    public class LinkedListDictionary<TKey, TValue> : IEnumerable<TValue>
    {
        private readonly LinkedList<TValue> linkedList;

        private readonly Dictionary<TKey, LinkedListNode<TValue>> dict;

        public LinkedListDictionary()
        {
            this.linkedList = new LinkedList<TValue>();
            this.dict = new Dictionary<TKey, LinkedListNode<TValue>>();
        }

        public LinkedListDictionary(int capacity)
        {
            this.linkedList = new LinkedList<TValue>();
            this.dict = new Dictionary<TKey, LinkedListNode<TValue>>(capacity);
        }

        #region Properties
        public int Count
        {
            get
            {
                return this.dict.Count;
            }
        }

        public LinkedListNode<TValue> First
        {
            get
            {
                return this.linkedList.First;
            }
        }

        public IEnumerable<TValue> Values
        {
            get
            {
                return this.linkedList;
            }
        }

        public IEnumerable<TKey> Keys
        {
            get
            {
                return this.dict.Keys;
            }
        }

        #endregion

        public void Add(TKey key, TValue value)
        {
            var node = new LinkedListNode<TValue>(value);
            this.dict.Add(key, node);
            this.linkedList.AddLast(node);
        }

        public bool ContainsKey(TKey key)
        {
            return this.dict.ContainsKey(key);
        }

        public bool Remove(TKey key)
        {
            LinkedListNode<TValue> node;
            if (this.dict.TryGetValue(key, out node) == false)
            {
                return false;
            }

            this.linkedList.Remove(node);
            this.dict.Remove(key);

            return true;
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            return this.TryGet(key, out value);
        }

        public bool TryGet(TKey key, out TValue value)
        {
            LinkedListNode<TValue> node;
            if (this.dict.TryGetValue(key, out node))
            {
                value = node.Value;
                return true;
            }

            value = default(TValue);
            return false;
        }

        public LinkedListNode<TValue> GetAtIndex(int index)
        {
            if (index >= this.Count)
            {
                throw new ArgumentOutOfRangeException("index", "The specified index is out of range");
            }

            if (index < this.Count / 2)
            {
                var node = this.linkedList.First;
                for (int i = 0; i < index; i++)
                {
                    Debug.Assert(node != null, "node != null");
                    node = node.Next;
                }

                return node;
            }
            else
            {
                var node = this.linkedList.Last;
                for (int i = this.linkedList.Count - 1; i > index; i--)
                {
                    Debug.Assert(node != null, "node != null");
                    node = node.Previous;
                }

                return node;
            }
        }

        #region IEnumerable<TValue> Members

        public IEnumerator<TValue> GetEnumerator()
        {
            return this.linkedList.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.linkedList.GetEnumerator();
        }

        #endregion
    }
}