using Elektronik.Common.Data.Packages.SlamActionPackages;
using Elektronik.Common;
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
    public class SlamPointsParser : SlamActionPackageParser
    {
        public SlamPointsParser(ICSConverter converter) : base(converter) { }

        public override IPackage Parse(byte[] data, int startIdx, ref int offset)
        {
            Header header = ParseHeader(ref data, startIdx, ref offset);
            if (header.ObjectType == ObjectType.Point && header.ActionType != ActionType.Connect)
            {
                int pointsCount = BitConverterEx.ToInt32(data, offset, ref offset);
                var points = new SlamPoint[pointsCount];

                for (int i = 0; i < pointsCount; ++i)
                {
                    points[i].id = BitConverterEx.ToInt32(data, offset, ref offset);
                    if (header.ActionType == ActionType.Create || header.ActionType == ActionType.Move)
                    {
                        points[i].position = BitConverterEx.ToVector3(data, offset, ref offset);
                    }
                    if (header.ActionType == ActionType.Create)
                    {
                        points[i].defaultColor = BitConverterEx.ToRGBColor32(data, offset, ref offset);
                        points[i].color = BitConverterEx.ToRGBColor32(data, offset, ref offset);
                    }
                    if (header.ActionType == ActionType.Tint)
                    {
                        points[i].color = BitConverterEx.ToRGBColor32(data, offset, ref offset);
                    }
                    if (header.ActionType == ActionType.Remove)
                    {
                        points[i].color = Color.red;
                    }
                    if (header.ActionType == ActionType.Message)
                    {
                        int countOfMsgBytes = BitConverterEx.ToInt32(data, offset, ref offset);
                        points[i].message = countOfMsgBytes > 0 ? Encoding.ASCII.GetString(data, offset, countOfMsgBytes) : "";
                        offset += sizeof(byte) * countOfMsgBytes;
                    }
                    var stub = Quaternion.identity;
                    m_converter.Convert(ref points[i].position, ref stub);
                }

                return new ActionDataPackage<SlamPoint>(
                    header.ObjectType,
                    header.ActionType,
                    header.PackageType,
                    header.Timestamp,
                    header.IsKey,
                    points);
            }
            return m_successor?.Parse(data, startIdx, ref offset);
        }
    }
}
