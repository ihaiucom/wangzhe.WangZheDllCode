// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InterestArea.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   Client interest area.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Photon.MmoDemo.Client
{
    using Photon.MmoDemo.Common;
    using System;

    public class InterestArea
    {
        private readonly byte cameraId;

        private readonly Game game;

        public InterestArea(byte cameraId, Game game, MyItem avatar)
            : this(cameraId, game, avatar.Position)
        {
            this.AttachedItem = avatar;
            avatar.Moved += this.OnItemMoved;
        }

        public InterestArea(byte cameraId, Game game, Vector position)
        {
            this.game = game;
            this.cameraId = cameraId;
            this.Position = position;
        }

        public MyItem AttachedItem { get; private set; }

        public Game Game
        {
            get
            {
                return this.game;
            }
        }

        public byte Id
        {
            get
            {
                return this.cameraId;
            }
        }

        public Vector Position { get; private set; }

        public Vector ViewDistanceEnter { get; private set; }

        public Vector ViewDistanceExit { get; private set; }

        public void AttachItem(MyItem item)
        {
            if (this.AttachedItem != null)
            {
                this.AttachedItem.Moved -= this.OnItemMoved;
                this.AttachedItem = null;
            }

            this.AttachedItem = item;
            item.Moved += this.OnItemMoved;

            Operations.AttachInterestArea(this.game, item.Id);
            item.SetInterestAreaAttached(true);
        }

        public void Create()
        {
            this.game.AddCamera(this);
            Operations.AddInterestArea(this.game, this.Id, this.Position, this.ViewDistanceEnter, this.ViewDistanceExit);
        }

        public void Detach()
        {
            if (this.AttachedItem != null)
            {
                this.AttachedItem.Moved -= this.OnItemMoved;
                this.AttachedItem.SetInterestAreaAttached(false);
                this.AttachedItem = null;
            }

            Operations.DetachInterestArea(this.game);
        }

        public void Move(Vector newPosition)
        {
            if (this.AttachedItem == null)
            {
                this.Position = newPosition;
                Operations.MoveInterestArea(this.game, this.cameraId, newPosition);
                return;
            }

            throw new InvalidOperationException("cannot move attached interest area manually");
        }

        public void Remove()
        {
            Operations.RemoveInterestArea(this.game, this.Id);
            this.game.RemoveCamera(this.cameraId);
        }

        public void ResetViewDistance()
        {
			this.SetViewDistance(this.game.WorldData.TileDimensions / 2 + new Vector(1, 1, 0));
//            this.SetViewDistance(this.game.WorldData.TileDimensions * 0.75f);
        }

        public void SetViewDistance(Vector viewDistance)
        {
            if (viewDistance.X < 0)
            {
                viewDistance.X = 0;
            }

            if (viewDistance.Y < 0)
            {
                viewDistance.Y = 0;
            }

            this.ViewDistanceEnter = viewDistance;

            this.ViewDistanceExit = Vector.Max(
                this.ViewDistanceEnter + this.game.WorldData.TileDimensions, this.ViewDistanceEnter*1.5f
                );

            Operations.SetViewDistance(this.game, this.ViewDistanceEnter, this.ViewDistanceExit);
            this.game.Avatar.SetInterestAreaViewDistance(this);
        }

        private void OnItemMoved(Item item)
        {
            this.Position = item.Position;
        }
    }
}