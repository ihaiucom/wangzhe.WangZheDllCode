// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Settings.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   The settings.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Photon.MmoDemo.Common;

public class DemoSettings : Photon.MmoDemo.Client.Settings
{
    public static DemoSettings GetDefaultSettings()
    {
        DemoSettings result = new DemoSettings();

        // photon
        result.ServerAddress = "localhost:5055";
        result.UseTcp = false;
        result.ApplicationName = "MmoDemo";

        // grid
        result.WorldName = "Mmo Grid Demo";
        result.TileDimensions = new Vector(1000, 1000);
        result.GridSize = result.TileDimensions*20; // 20 tiles. each is TileDimensions of size

        // game engine
        result.AutoMoveInterval = 1000;


        result.SendReliable = false;
        result.AutoMoveVelocity = 10;
        result.AutoMove = false;

        return result;
    }

    private bool autoMove;

    private int autoMoveInterval;

    private int autoMoveVelocity;

    private bool useTcp;

    public bool AutoMove
    {
        get { return this.autoMove; }

        set { this.autoMove = value; }
    }

    public int AutoMoveInterval
    {
        get { return this.autoMoveInterval; }

        set { this.autoMoveInterval = value; }
    }

    public int AutoMoveVelocity
    {
        get { return this.autoMoveVelocity; }

        set { this.autoMoveVelocity = value; }
    }

    public bool UseTcp
    {
        get { return this.useTcp; }

        set { this.useTcp = value; }
    }
}