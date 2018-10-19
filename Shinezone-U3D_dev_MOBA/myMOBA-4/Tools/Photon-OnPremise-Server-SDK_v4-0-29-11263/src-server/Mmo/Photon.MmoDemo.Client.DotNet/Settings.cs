// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Settings.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   The settings.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Photon.MmoDemo.Common;
namespace Photon.MmoDemo.Client
{
    public class Settings
    {
        static Settings()
        {
            RadarChannel = 0;
            DiagnosticsChannel = 0;
            OperationChannel = 0;
            ItemChannel = 0;
        }

        public static byte DiagnosticsChannel { get; set; }

        public static byte ItemChannel { get; set; }

        public static byte OperationChannel { get; set; }

        public static byte RadarChannel { get; set; }

        public string ApplicationName { get; set; }

        public Vector GridSize { get; set; }

        public bool SendReliable { get; set; }

        public string ServerAddress { get; set; }

        public Vector TileDimensions { get; set; }

        public string WorldName { get; set; }
    }
}