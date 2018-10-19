// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Protocol.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   (De)serialization procedures used in server and client.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Photon.MmoDemo.Common
{
    /// <summary>
    /// (De)serialization procedures used in server and client.
    /// </summary>
    public static class Protocol
    {
        public enum CustomTypeCodes
        {
            Vector = 0,
            BoundingBox
        }

        // TODO: proper de/serialization
        private static void SerializeFloat(float x, byte[] dst, int offset)
        {
            float[] memFloatBlock = new float[1];
            memFloatBlock[0] = x;
            Buffer.BlockCopy(memFloatBlock, 0, dst, offset, 4);
            if (BitConverter.IsLittleEndian)
            {
                byte temp0 = dst[0 + offset];
                byte temp1 = dst[1 + offset];
                dst[0 + offset] = dst[3 + offset];
                dst[1 + offset] = dst[2 + offset];
                dst[2 + offset] = temp1;
                dst[3 + offset] = temp0;
            }
        }

        private static float DeserializeFloat(byte[] data, int offset)
        {
            {
                if (BitConverter.IsLittleEndian)
                {
                    byte[] tmp = new byte[4];
                    tmp[3] = data[0 + offset];
                    tmp[2] = data[1 + offset];
                    tmp[1] = data[2 + offset];
                    tmp[0] = data[3 + offset];
                    return BitConverter.ToSingle(tmp, 0);
                }
                else
                {
                    return BitConverter.ToSingle(data, offset);
                }
            }
        }

        public static byte[] SerializeVector(object x)
        {
            var v = (Vector)x;
            byte[] res = new byte[12];
            SerializeFloat(v.X, res, 0);
            SerializeFloat(v.Y, res, 4);
            SerializeFloat(v.Z, res, 8);
            return res;
        }

        public static object DeserializeVector(byte[] data)
        {
            return new Vector(DeserializeFloat(data, 0), DeserializeFloat(data, 4), DeserializeFloat(data, 8));
        }

        public static byte[] SerializeBoundingBox(object x)
        {
            var b = (BoundingBox)x;
            byte[] res = new byte[24];
            SerializeFloat(b.Min.X, res, 0);
            SerializeFloat(b.Min.Y, res, 4);
            SerializeFloat(b.Min.Z, res, 8);
            SerializeFloat(b.Max.X, res, 12);
            SerializeFloat(b.Max.Y, res, 16);
            SerializeFloat(b.Max.Z, res, 20);
            return res;
        }

        public static object DeserializeBoundingBox(byte[] data)
        {
            return new BoundingBox
            {
                Min = new Vector(DeserializeFloat(data, 0), DeserializeFloat(data, 4), DeserializeFloat(data, 8)),
                Max = new Vector(DeserializeFloat(data, 12), DeserializeFloat(data, 16), DeserializeFloat(data, 20))
            };
        }

    }
}
