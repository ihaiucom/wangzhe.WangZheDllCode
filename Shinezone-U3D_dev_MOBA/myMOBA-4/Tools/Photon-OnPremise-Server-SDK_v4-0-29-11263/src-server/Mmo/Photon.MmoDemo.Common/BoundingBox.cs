// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BoundingBox.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   The 3D floating point bounding box.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Photon.MmoDemo.Common
{
    using System;

    /// <summary>
    /// The 3D floating point bounding box.
    /// </summary>
    public struct BoundingBox
    {

        public Vector Max { get; set; }

        public Vector Min { get; set; }

        public BoundingBox(Vector min, Vector max) : this()
        {
            this.Min = min;
            this.Max = max;
        }

        public static BoundingBox CreateFromPoints(params Vector[] points)
        {
            if (points == null)
            {
                throw new ArgumentNullException("points");
            }

            if (points.Length == 0)
            {
                throw new ArgumentException("points");
            }

            Vector min = points[0];
            Vector max = points[1];
            for (int i = 1; i < points.Length; i++)
            {
                min = Vector.Min(min, points[i]);
                max = Vector.Max(max, points[i]);
            }

            return new BoundingBox { Min = min, Max = max };
        }

        public Vector Size { get { return this.Max - this.Min; } }

        public bool Contains(Vector point)
        {
            // not outside of box?
            return (point.X < this.Min.X || point.X > this.Max.X || point.Y < this.Min.Y || point.Y > this.Max.Y || point.Z < this.Min.Z || point.Z > this.Max.Z) ==
                   false;
        }

        public bool Contains2d(Vector point)
        {
            // not outside of box?
            return (point.X < this.Min.X || point.X > this.Max.X || point.Y < this.Min.Y || point.Y > this.Max.Y) ==
                   false;
        }

        public BoundingBox IntersectWith(BoundingBox other)
        {
            return new BoundingBox { Min = Vector.Max(this.Min, other.Min), Max = Vector.Min(this.Max, other.Max) };
        }

        public BoundingBox UnionWith(BoundingBox other)
        {
            return new BoundingBox { Min = Vector.Min(this.Min, other.Min), Max = Vector.Max(this.Max, other.Max) };
        }

        public bool IsValid()
        {
            return (this.Max.X < this.Min.X || this.Max.Y < this.Min.Y || this.Max.Z < this.Min.Z) == false;
        }

        public override string ToString()
        {
            return string.Format("{0}({1},{2},{3})({4},{5},{6})", base.ToString(), Min.X, Min.Y, Min.Z, Max.X, Max.Y, Max.Z);
        }
    }
}