// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Settings.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   The settings.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Photon.MmoDemo.Client.WinGrid
{
    using System.Drawing;

    public class Settings : Client.Settings
    {
        internal static readonly Image IslandImage;

        internal static readonly Brush IslandTextBrush = Brushes.Yellow;

        static Settings()
        {
            IslandImage = new Bitmap(typeof(GameTabPage), "Island.png");
            IslandImage.RotateFlip(RotateFlipType.Rotate180FlipNone);
        }

        public bool AutoMove { get; set; }

        public int AutoMoveInterval { get; set; }

        public int AutoMoveVelocity { get; set; }

        public int DrawInterval { get; set; }

        public int SendInterval { get; set; }

        public bool UseTcp { get; set; }
    }
}