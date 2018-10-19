// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorldData.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   The world data.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Photon.MmoDemo.Common;
namespace Photon.MmoDemo.Client
{
    public class WorldData
    {
        public BoundingBox BoundingBox { get; set; }

        public float Height { get { return this.BoundingBox.Max.Y - this.BoundingBox.Min.Y; } }

        public string Name { get; set; }

        public Vector TileDimensions { get; set; }

        public float Width { get { return this.BoundingBox.Max.X - this.BoundingBox.Min.X; } }

        public override string ToString()
        {
            return string.Format("min {0}:{1}, max {2}:{3}, tiledimensions {4}:{5}", BoundingBox.Min.X, BoundingBox.Min.Y, BoundingBox.Max.X, BoundingBox.Max.Y, TileDimensions.X, TileDimensions.Y);
        }
    }
}