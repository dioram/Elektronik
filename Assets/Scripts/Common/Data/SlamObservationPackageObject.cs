using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Elektronik.Common.Data
{
    public static class SlamObservationPackageObject
    {
        private static readonly int MAX_MESSAGE_LENGTH_IN_BYTES = 128;

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
                case ActionType.Message:
                    return sizeof(int) + sizeof(byte) * MAX_MESSAGE_LENGTH_IN_BYTES; // size + message bytes
                default:
                    throw new ArgumentException(String.Format("Bad action type ({0})", actionType));
            }
        }

        public static void ParseActions(byte[] actions, int id, out SlamObservation observation)
        {
            int offset = 0;
            observation = new SlamObservation()
            {
                orientation = Quaternion.identity,
            };
            SlamPoint obsPoint = observation;
            obsPoint.id = id;
            obsPoint.color = Color.gray;
            while (offset != actions.Length)
            {
                Debug.AssertFormat(offset <= actions.Length, "[SlamObservationPackageObject.ParseActions] offset ({0}) out of range", offset);
                ActionType type = (ActionType)actions[offset++];
                if (type == ActionType.Create)
                {
                    obsPoint.isNew = true;
                }
                if (type == ActionType.Create || type == ActionType.Move)
                {
                    obsPoint.position = SlamBitConverter.ToVector3(actions, offset);
                    offset += sizeof(float) * 3;
                    observation.orientation = SlamBitConverter.ToQuaternion(actions, offset);
                    offset += sizeof(float) * 4;
                }
                
                if (type == ActionType.Tint)
                {
                    obsPoint.color = SlamBitConverter.ToRGBColor(actions, offset);
                    offset += sizeof(byte) * 3;
                }
                if (type == ActionType.Remove)
                {
                    obsPoint.color = Color.red;
                    obsPoint.isRemoved = true;
                }
                if (type == ActionType.Connect)
                {
                    int covisibleId = BitConverter.ToInt32(actions, offset);
                    offset += sizeof(int);
                    int countOfCommonPoints = BitConverter.ToInt32(actions, offset);
                    offset += sizeof(int);
                    observation.m_covisibleObservationsIds.Add(covisibleId);
                    observation.m_covisibleObservationsOfCommonPointsCount.Add(countOfCommonPoints);
                }
                if (type == ActionType.Message)
                {
                    int countOfMsgBytes = BitConverter.ToInt32(actions, offset);
                    offset += sizeof(int);
                    if (countOfMsgBytes >= MAX_MESSAGE_LENGTH_IN_BYTES)
                        throw new Exception();
                    obsPoint.message = countOfMsgBytes > 0 ? Encoding.ASCII.GetString(actions, offset, countOfMsgBytes) : "";
                    offset += sizeof(byte) * MAX_MESSAGE_LENGTH_IN_BYTES;
                }
            }
            observation.Point = obsPoint;
        }

    }
}
