// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RadarTabPage.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   The radar tab page.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Photon.MmoDemo.Client.WinGrid
{
    using Photon.MmoDemo.Common;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Windows.Forms;

    public partial class RadarTabPage : TabPage
    {
        private readonly Dictionary<string, Vector> itemPositions = new Dictionary<string, Vector>();

        private WorldData worldData;

        public RadarTabPage()
        {
            this.SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
        }

        public override Image BackgroundImage
        {
            get
            {
                return Settings.IslandImage;
            }

            set
            {
            }
        }

        public override ImageLayout BackgroundImageLayout
        {
            get
            {
                return ImageLayout.Stretch;
            }

            set
            {
            }
        }

        public bool Initialized { get; private set; }

        public void Initialize(WorldData world)
        {
            this.worldData = world;
            this.Initialized = true;
        }

        public void OnRadarUpdate(string itemId, ItemType itemType, Vector position, bool remove)
        {
            itemId += itemType;
            if (remove)
            {
                this.itemPositions.Remove(itemId);
                return;
            }

            if (!this.itemPositions.ContainsKey(itemId))
            {
                this.itemPositions.Add(itemId, position);
                return;
            }

            this.itemPositions[itemId] = position;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (this.worldData != null)
            {
                var boardSize = new[] { e.ClipRectangle.Width, e.ClipRectangle.Height };
                GameTabPage.PaintGrid(e.Graphics, boardSize, this.worldData);

                WorldData world = this.worldData;
                float width = world.Width;
                float height = world.Height;
                float w = boardSize[0] / width;
                float h = boardSize[1] / height;
                float minW = Math.Max(4, w);
                float minH = Math.Max(4, h);

                Color color = Color.DeepPink;
                var brush = new SolidBrush(color);

                foreach (Vector entry in this.itemPositions.Values)
                {
                    float x = (entry.X * w) - 1;
                    float y = ((this.worldData.BoundingBox.Max.Y - entry.Y) * h) - 1;

                    e.Graphics.FillRectangle(brush, x - (minW / 2) + 1, y - (minH / 2) + 1, minW - 1, minH - 1);
                }

                string rttString = string.Format("Online: {0}", this.itemPositions.Count);
                e.Graphics.DrawString(rttString, this.Font, Settings.IslandTextBrush, 0, 0);
            }
        }
    }
}