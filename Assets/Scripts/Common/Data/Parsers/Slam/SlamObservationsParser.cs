using Elektronik.Common.Data.Packages.SlamActionPackages;
using Elektronik.Common.Data.Converters;
using Elektronik.Common.Data.PackageObjects;
using Elektronik.Common.Data.Packages;
using Elektronik.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Elektronik.Common.Data.Parsers.Slam
{
    public class SlamObservationsParser : SlamActionPackageParser
    {
        public SlamObservationsParser(ICSConverter converter) : base(converter) { }

        public override IPackage Parse(byte[] data, int startIdx, ref int offset)
        {
            Header header = ParseHeader(ref data, startIdx, ref offset);
            if (header.ObjectType == ObjectType.Observation && header.ActionType != ActionType.Connect)
            {
                int observationsCount = BitConverterEx.ToInt32(data, offset, ref offset);
                var observations = new SlamObservation[observationsCount];
                for (int i = 0; i < observationsCount; ++i)
                {
                    var pt = new SlamPoint();
                    var rot = Quaternion.identity;
                    pt.id = BitConverterEx.ToInt32(data, offset, ref offset);
                    if (header.ActionType == ActionType.Create || header.ActionType == ActionType.Move)
                    {
                        pt.position = BitConverterEx.ToVector3(data, offset, ref offset);
                        rot = BitConverterEx.ToQuaternion(data, offset, ref offset);
                    }
                    if (header.ActionType == ActionType.Create)
                    {
                        pt.defaultColor = BitConverterEx.ToRGBColor(data, offset, ref offset);
                    }
                    if (header.ActionType == ActionType.Create || header.ActionType == ActionType.Tint)
                    {
                        pt.color = BitConverterEx.ToRGBColor32(data, offset, ref offset);
                    }
                    if (header.ActionType == ActionType.Remove)
                    {
                        pt.color = Color.red;
                    }
                    if (header.ActionType == ActionType.Message)
                    {
                        int countOfMsgBytes = BitConverterEx.ToInt32(data, offset, ref offset);
                        pt.message = countOfMsgBytes > 0 ? Encoding.ASCII.GetString(data, offset, countOfMsgBytes) : "";
                        offset += sizeof(byte) * countOfMsgBytes;
                    }
                    m_converter.Convert(ref pt.position, ref rot);
                    observations[i] = new SlamObservation(pt, rot);
                }

                return new ActionDataPackage<SlamObservation>(
                    header.ObjectType,
                    header.ActionType,
                    header.PackageType,
                    header.Timestamp,
                    header.IsKey,
                    observations);
            }
            return m_successor?.Parse(data, startIdx, ref offset);
        }
    }
}
