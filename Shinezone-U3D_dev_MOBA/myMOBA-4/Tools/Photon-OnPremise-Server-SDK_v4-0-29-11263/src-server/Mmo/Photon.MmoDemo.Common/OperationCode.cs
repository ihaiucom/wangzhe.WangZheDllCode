// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OperationCode.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   Known operation codes.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Photon.MmoDemo.Common
{
    /// <summary>
    /// Known operation codes.
    /// <summary>
    public enum OperationCode : byte
    {
        Nil = 0, 

        CreateWorld = 90, 

        EnterWorld = 91, 

        ExitWorld = 92, 

        Move = 93,

        RaiseGenericEvent = 94, 

        SetProperties = 95, 

        SpawnItem = 96, 

        DestroyItem = 97, 

        // Manually subscribes item (does not affect interest area updates).
        SubscribeItem = 98, 

        UnsubscribeItem = 99, 

        SetViewDistance = 100, 

        AttachInterestArea = 101, 

        DetachInterestArea = 102, 

        AddInterestArea = 103, 

        RemoveInterestArea = 104, 

        GetProperties = 105, 

        MoveInterestArea = 106, 

        RadarSubscribe = 107, 

        UnsubscribeCounter = 108, 

        SubscribeCounter = 109
    }
}