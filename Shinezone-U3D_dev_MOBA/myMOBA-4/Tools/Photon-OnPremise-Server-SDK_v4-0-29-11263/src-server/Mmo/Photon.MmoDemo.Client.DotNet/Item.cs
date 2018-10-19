// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Item.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   The item base.
// </summary>
// --------------------------------------------------------------------------------------------------------------------



namespace Photon.MmoDemo.Client
{
    using System;
    using System.Collections.Generic;
    using Photon.MmoDemo.Common;

    public class Item
    {
        public static readonly string PropertyKeyColor = "color";

        public static readonly string PropertyKeyInterestAreaAttached = "attached";

        public static readonly string PropertyKeyText = "text";

        public static readonly string PropertyKeyViewDistanceEnter = "enter";

        public static readonly string PropertyKeyViewDistanceExit = "exit";


        private readonly Game game;

        private readonly string id;
        
        /// <summary>
        /// Keeps all interest areas which cover the item.
        /// </summary>
        public readonly List<byte> subscribedInterestAreas;
        private readonly ItemType type;

        private readonly bool isMine;

        public Item(Game game, string id, ItemType type, bool isMine = false)
        {
            this.game = game;
            this.id = id;            
            this.type = type;
            this.isMine = isMine;
            this.subscribedInterestAreas = new List<byte>();
        }

        public virtual bool IsMine { get { return isMine; } }

        public event Action<Item> Moved;


        public int Color { get; private set; }


        public Game Game
        {
            get { return this.game; }
        }


        public string Id
        {
            get { return this.id; }
        }


        public bool InterestAreaAttached { get; private set; }


        public bool IsDestroyed { get; set; }

        // item local state is up-to-date, we can show item
        public bool IsUpToDate
        {
            get { return this.subscribedInterestAreas.Count > 0 && (this.IsMine || this.PropertyRevisionLocal == this.PropertyRevisionRemote); }
        }


        public Vector Position { get; private set; }


        public Vector Rotation { get; private set; }


        public Vector? PreviousPosition { get; private set; }


        public Vector PreviousRotation { get; private set; }


        public int PropertyRevisionLocal { get; set; }

        public int PropertyRevisionRemote { get; set; }

        public string Text { get; private set; }


        public ItemType Type
        {
            get { return this.type; }
        }

        /// <summary>
        /// Subscribe to area-updates of all areas as far from item.Position as this value.
        /// </summary>
        /// <remarks>
        /// If you set 1000, the server sends updates from areas +1000 units away and -1000 units away.
        /// </remarks>
        public Vector ViewDistanceEnter { get; private set; }

        /// <summary>
        /// Unsubscribe area-updates of all areas further away from item.Position than this value.
        /// </summary>
        public Vector ViewDistanceExit { get; private set; }


        public bool AddSubscribedInterestArea(byte cameraId)
        {
            if (this.subscribedInterestAreas.Contains(cameraId))
            {
                return false;
            }

            this.subscribedInterestAreas.Add(cameraId);
            return true;
        }

        public void GetInitialProperties()
        {
            Operations.GetProperties(this.game, this.id, null);
        }


        public void GetProperties()
        {
            Operations.GetProperties(this.game, this.id, this.PropertyRevisionLocal);
        }

        public bool RemoveSubscribedInterestArea(byte cameraId)
        {
            return this.subscribedInterestAreas.Remove(cameraId);
        }

        public void ResetPreviousPosition()
        {
            PreviousPosition = null;
        }


        public virtual void SetColor(int color)
        {
            Color = color;
        }


        public virtual void SetInterestAreaAttached(bool attached)
        {
            InterestAreaAttached = attached;
        }


        public virtual void SetInterestAreaViewDistance(Vector viewDistanceEnter, Vector viewDistanceExit)
        {
            ViewDistanceEnter = viewDistanceEnter;
            ViewDistanceExit = viewDistanceExit;
        }


        public void SetPositions(Vector position, Vector previousPosition, Vector rotation, Vector previousRotation)
        {
            Position = position;
            PreviousPosition = previousPosition;
            Rotation = rotation;
            PreviousRotation = previousPosition;

            OnMoved();
        }


        public virtual void SetText(string text)
        {
            Text = text;
        }


        private void OnMoved()
        {
            if (Moved != null)
            {
                Moved(this);
            }
        }
    }
}