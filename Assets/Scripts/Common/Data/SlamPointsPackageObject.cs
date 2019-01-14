using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Elektronik.Common.Data
{
    public static class SlamPointsPackageObject
    {
        public static int GetSizeOfActionInBytes(ActionType actionType)
        {
            switch (actionType)
            {
                case ActionType.Create:
                    return sizeof(float) * 3; // Vector3
                case ActionType.Tint:
                    return sizeof(byte) * 3; // color
                case ActionType.Move:
                    return sizeof(float) * 3; // Vector3
                case ActionType.Remove:
                    return 0;
                case ActionType.Fuse:
                    return sizeof(int) + sizeof(byte) * 6; // id + color1 + color2
                case ActionType.Connect:
                    return sizeof(int) * 2; // id + count of common points
                default:
                    return -1;
            }
        }

        public static void ParseActions(byte[] actions, int id, out SlamPoint point, out SlamLine? fuse)
        {
            int offset = 0;

        }

        

    }
}
