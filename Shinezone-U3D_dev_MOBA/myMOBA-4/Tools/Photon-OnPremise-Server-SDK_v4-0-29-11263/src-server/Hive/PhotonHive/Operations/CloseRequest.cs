// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CloseRequest.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Photon.Hive.Operations
{
    using System.Collections.Generic;

    using Photon.Hive.Plugin;

    public class CloseRequest : ICloseRequest
    {
        #region Properties

        public byte WebFlags { get; set; }

        public int EmptyRoomTTL { get; set; }

        public byte OperationCode
        {
            get
            {
                return 0;
            }
        }

        public Dictionary<byte, object> Parameters
        {
            get
            {
                return null;
            }
        }

        #endregion
    }
}