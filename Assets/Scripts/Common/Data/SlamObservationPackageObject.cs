using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Elektronik.Common.Data
{
    public static class SlamObservationPackageObject
    {
        public static int GetSizeOfActionInBytes(ActionType actionType)
        {
            switch (actionType)
            {
                case ActionType.Create:
                    return sizeof(float) * 3 + sizeof(float) * 4; // Vector3
                case ActionType.Tint:
                    return sizeof(byte) * 3; // color
                case ActionType.Move:
                    return sizeof(float) * 3 + sizeof(float) * 4; // Vector3
                case ActionType.Remove:
                    return 0;
                case ActionType.Connect:
                    return sizeof(int) * 2; // id + count of common points
                default:
                    throw new ArgumentException(String.Format("Bad action type ({0})", actionType));
            }
        }

        public static void ParseActions(byte[] actions, int id, out SlamObservation observation)
        {
            int offset = 0;
            observation = new SlamObservation();
            observation.orientation = Quaternion.identity;
            observation.id = id;
            observation.color = Color.magenta;
            while (offset != actions.Length)
            {
                Debug.AssertFormat(offset <= actions.Length, "[SlamObservationPackageObject.ParseActions] offset ({0}) out of range", offset);
                ActionType type = (ActionType)actions[offset++];
                if (type == ActionType.Create)
                {
                    observation.isNew = true;
                    observation.color = Color.magenta;
                }
                if (type == ActionType.Create || type == ActionType.Move)
                {
                    observation.position = SlamBitConverter.ToVector3(actions, offset);
                    offset += sizeof(float) * 3;
                    observation.orientation = SlamBitConverter.ToQuaternion(actions, offset);
                    offset += sizeof(float) * 4;
                }
                
                if (type == ActionType.Tint)
                {
                    observation.color = SlamBitConverter.ToRGBColor(actions, offset);
                    offset += sizeof(byte) * 3;
                }
                if (type == ActionType.Remove)
                {
                    observation.color = Color.red;
                    observation.isRemoved = true;
                }
                if (type == ActionType.Connect)
                {
                    int covisibleId = BitConverter.ToInt32(actions, offset);
                    offset += sizeof(int);
                    int countOfCommonPoints = BitConverter.ToInt32(actions, offset);
                    offset += sizeof(int);
                    observation.covisibleObservationsIds.Add(covisibleId);
                    observation.covisibleObservationsOfCommonPointsCount.Add(countOfCommonPoints);
                }

            }
        }

    }
}
