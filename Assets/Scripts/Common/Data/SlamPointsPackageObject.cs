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
            point = new SlamPoint();
            fuse = null;
            point.id = id;
            while (offset != actions.Length)
            {
                Debug.AssertFormat(offset <= actions.Length, "[SlamPointsPackageObject.ParseActions] offset ({0}) out of range", offset);
                ActionType type = (ActionType)actions[offset++];
                if (type == ActionType.Create || type == ActionType.Move)
                {
                    point.position = SlamBitConverter.ToVector3(actions, offset);
                    offset += sizeof(float) * 3;
                }
                if (type == ActionType.Create)
                {
                    point.isNew = true;
                    point.color = Color.blue;
                }
                if (type == ActionType.Tint)
                {
                    point.color = SlamBitConverter.ToRGBColor(actions, offset);
                    offset += sizeof(byte) * 3;
                    point.justColored = true;
                }
                if (type == ActionType.Remove)
                {
                    point.color = Color.red;
                    point.isRemoved = true;
                }
                if (type == ActionType.Fuse)
                {
                    point.color = Color.magenta;
                    int fuseWithId = BitConverter.ToInt32(actions, offset);
                    offset += sizeof(int);
                    Color color1 = SlamBitConverter.ToRGBColor(actions, offset);
                    offset += sizeof(byte) * 3;
                    Color color2 = SlamBitConverter.ToRGBColor(actions, offset);
                    offset += sizeof(byte) * 3;
                    SlamLine fuseLine = new SlamLine()
                    {
                        pointId1 = point.id,
                        pointId2 = fuseWithId,
                        color1 = color1,
                        color2 = color2,
                    };
                    fuse = fuseLine;
                }
            }
        }

        

    }
}
