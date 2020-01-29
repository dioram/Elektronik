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
    public class SlamLinesParser : SlamActionPackageParser
    {
        public SlamLinesParser(ICSConverter converter) : base(converter) { }

        public override IPackage Parse(byte[] data, int startIdx, ref int offset)
        {
            Header header = ParseHeader(ref data, startIdx, ref offset);
            if (header.ObjectType == ObjectType.Line || header.ActionType == ActionType.Connect)
            {
                int connectionsCount = BitConverterEx.ToInt32(data, offset, ref offset);
                var connections = new SlamLine[connectionsCount];
                bool isCreateOrConnect = header.ActionType == ActionType.Create || header.ActionType == ActionType.Connect;
                for (int i = 0; i < connectionsCount; ++i)
                {
                    var pt1 = new SlamPoint();
                    var pt2 = new SlamPoint();

                    pt1.id = BitConverterEx.ToInt32(data, offset, ref offset);
                    if (isCreateOrConnect || header.ActionType == ActionType.Move)
                        pt1.position = BitConverterEx.ToVector3(data, offset, ref offset);
                    if (isCreateOrConnect)
                        pt1.defaultColor = BitConverterEx.ToRGBColor(data, offset, ref offset);
                    if (isCreateOrConnect || header.ActionType == ActionType.Tint)
                        pt1.color = BitConverterEx.ToRGBColor(data, offset, ref offset);
                    
                    pt2.id = BitConverterEx.ToInt32(data, offset, ref offset);
                    if (isCreateOrConnect  || header.ActionType == ActionType.Move)
                        pt2.position = BitConverterEx.ToVector3(data, offset, ref offset);
                    if (isCreateOrConnect )
                        pt2.defaultColor = BitConverterEx.ToRGBColor(data, offset, ref offset);
                    if (isCreateOrConnect  || header.ActionType == ActionType.Tint)
                        pt2.color = BitConverterEx.ToRGBColor(data, offset, ref offset);
                    if (header.ActionType == ActionType.Remove)
                    {
                        pt1.color = Color.red;
                        pt2.color = Color.red;
                    }
                    connections[i] = new SlamLine(pt1, pt2);
                }
                return new ActionDataPackage<SlamLine>(
                    header.ObjectType,
                    header.ActionType,
                    header.PackageType,
                    header.Timestamp,
                    header.IsKey,
                    connections);
            }
            return m_successor?.Parse(data, startIdx, ref offset);
        }
    }
}
