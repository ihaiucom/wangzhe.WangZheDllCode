// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GridWorldTests.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   The grid world tests.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Photon.MmoDemo.Tests.Disconnected
{
    using System;
    using System.Collections.Generic;

    using NUnit.Framework;
    
    using Photon.MmoDemo.Common;
    using Photon.MmoDemo.Server;

    [TestFixture]
    public class GridWorldTests
    {
        [Test]
        public void RegionIndexesAndBorders1()
        {
            var min = new Vector { X = 1, Y = 1 };
            var max = new Vector { X = 99, Y = 999 };
            var tileDimensions = new Vector { X = 10, Y = 10 };
            var world = new GridWorld(new BoundingBox(min, max), tileDimensions);

            RegionIndexesAndBorders(world);
        }

        [Test]
        public void RegionIndexesAndBorders2()
        {
            var min = new Vector { X = -100, Y = -100 };
            var max = new Vector { X = -1, Y = -1 };
            var tileDimensions = new Vector { X = 10, Y = 10 };
            var world = new GridWorld(new BoundingBox(min, max), tileDimensions);

            RegionIndexesAndBorders(world);
        }

        [Test]
        public void RegionIndexesAndBorders3()
        {
            var min = new Vector { X = -100, Y = -100 };
            var max = new Vector { X = 100, Y = 100 };
            var tileDimensions = new Vector { X = 10, Y = 10 };
            var world = new GridWorld(new BoundingBox(min, max), tileDimensions);

            RegionIndexesAndBorders(world);
        }

        [Test]
        public void RegionIndexesAndBorders4()
        {
            var min = new Vector { X = 0, Y = 0 };
            var max = new Vector { X = 100, Y = 100 };
            var tileDimensions = new Vector { X = 1, Y = 1 };
            var world = new GridWorld(new BoundingBox(min, max), tileDimensions);

            RegionIndexesAndBorders(world);
        }

        [Test]
        public void RegionIndexesAndBorders5()
        {
            var min = new Vector { X = 0, Y = 0 };
            var max = new Vector { X = 100, Y = 100 };
            var tileDimensions = new Vector { X = 1, Y = 1 };
            var world = new GridWorld(new BoundingBox(min, max), tileDimensions);

            RegionIndexesAndBorders(world);
        }

        private static void RegionIndexesAndBorders(GridWorld world)
        {
            Region region;
            var current = new Vector();
            for (current.X = world.Area.Min.X; current.X < world.Area.Max.X; current.X++)
            {
                for (current.Y = world.Area.Min.Y; current.Y < world.Area.Max.Y; current.Y++)
                {
                    Assert.IsNotNull(world.GetRegion(current), current.ToString());
                }
            }

            try
            {
                for (current.Y = world.Area.Min.Y; current.Y < world.Area.Max.Y; current.Y += world.TileDimensions.Y)
                {
                    // current.Y = (float)Math.Round(current.Y, 2);
                    for (current.X = world.Area.Min.X; current.X < world.Area.Max.X; current.X += world.TileDimensions.X)
                    {
                        // current.X = (float)Math.Round(current.X, 2);
                        Assert.IsNotNull(region = world.GetRegion(current), "null at {0}", current);
                    }
                }
            }
            catch (IndexOutOfRangeException)
            {
                Console.WriteLine(current);
                throw;
            }

            current.X = world.Area.Min.X - 1;
            Assert.IsNull(world.GetRegion(current));
            current.Y = world.Area.Min.Y - 1;
            Assert.IsNull(world.GetRegion(current));
            current.X = world.Area.Min.X;
            Assert.IsNull(world.GetRegion(current));

            current.Y = world.Area.Max.Y + 1;
            Assert.IsNull(world.GetRegion(current));
            current.X = world.Area.Max.X + 1;
            Assert.IsNull(world.GetRegion(current));
            current.Y = world.Area.Max.Y;
            Assert.IsNull(world.GetRegion(current));

            Assert.NotNull(world.GetRegion(world.Area.Min));
            Assert.IsNull(world.GetRegion(world.Area.Max));

            world.GetRegions(world.Area);
        }
    }
}