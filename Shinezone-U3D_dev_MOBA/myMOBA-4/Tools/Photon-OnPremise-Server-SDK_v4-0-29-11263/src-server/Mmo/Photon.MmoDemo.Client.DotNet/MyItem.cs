// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MyItem.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   The mmo item.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Photon.MmoDemo.Client
{
    using Photon.MmoDemo.Common;
    using System;
    using System.Collections;
#if Unity
    using Hashtable = ExitGames.Client.Photon.Hashtable;
#endif

    public class MyItem : Item
    {
        public MyItem(Game game, string id, ItemType type, string text)
            : base(game, id, type, true)
        {
            var r = new Random(Guid.NewGuid().GetHashCode());
            const int b0 = 127;
            var c1 = (uint)r.Next(b0, 256);
            var c2 = (uint)r.Next(b0, 256);
            var c3 = (uint)r.Next(b0, 256);
            base.SetColor((int)(((c1 << 16) + (c2 << 8) + c3) | 0xFF000000));
            base.SetText(text);
        }

        public override bool IsMine { get { return true; } }

        public bool IsMoving { get; set; }

        public void Destroy()
        {
            this.IsDestroyed = true;
            Operations.DestroyItem(this.Game, this.Id);
        }

        public Hashtable BuildProperties() 
        {
            return new Hashtable
                {
                    { PropertyKeyInterestAreaAttached, this.InterestAreaAttached }, 
                    { PropertyKeyViewDistanceEnter, this.ViewDistanceEnter }, 
                    { PropertyKeyViewDistanceExit, this.ViewDistanceExit }, 
                    { PropertyKeyColor, this.Color }, 
                    { PropertyKeyText, this.Text }
                };
        }

        public bool MoveAbsolute(Vector newPosition, Vector rotation)
        {
            if (!this.Game.WorldData.BoundingBox.Contains2d(newPosition))
            {
                return false;
            }
            this.SetPositions(newPosition, this.Position, rotation, this.Rotation);
            Operations.Move(this.Game, this.Id, newPosition, rotation, this.Game.Settings.SendReliable);
            return true;
        }

        public bool MoveRelative(Vector offset, Vector rotation)
        {
            return this.MoveAbsolute(this.Position + offset, rotation);
        }

        public override void SetColor(int color)
        {
            if (color != this.Color)
            {
                base.SetColor(color);
                Operations.SetProperties(this.Game, this.Id, new Hashtable { { PropertyKeyColor, color } }, null, true);
            }
        }

        public override void SetInterestAreaAttached(bool attached)
        {
            if (attached != this.InterestAreaAttached)
            {
                base.SetInterestAreaAttached(attached);
                Operations.SetProperties(this.Game, this.Id, new Hashtable { { PropertyKeyInterestAreaAttached, attached } }, null, true);
            }
        }

        public override void SetInterestAreaViewDistance(Vector viewDistanceEnter, Vector viewDistanceExit)
        {
            base.SetInterestAreaViewDistance(viewDistanceEnter, viewDistanceExit);
            Operations.SetProperties(
                this.Game, 
                this.Id, 
                new Hashtable { { PropertyKeyViewDistanceEnter, viewDistanceEnter }, { PropertyKeyViewDistanceExit, viewDistanceExit } }, 
                null, 
                true);
        }

        public void SetInterestAreaViewDistance(InterestArea camera)
        {
            this.SetInterestAreaViewDistance(camera.ViewDistanceEnter, camera.ViewDistanceExit);
        }

        public override void SetText(string text)
        {
            if (text != this.Text)
            {
                base.SetText(text);
                Operations.SetProperties(this.Game, this.Id, new Hashtable { { PropertyKeyText, text } }, null, true);
            }
        }

        public void Spawn(Vector position, Vector rotation, int color, bool subscribe)
        {
            this.SetPositions(position, position, rotation, rotation);
            base.SetInterestAreaViewDistance(Vector.Zero, Vector.Zero);
            base.SetColor(color);
            Operations.SpawnItem(this.Game, this.Id, this.Type, position, rotation, this.BuildProperties(), subscribe);
        }
    }
}