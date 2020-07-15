using Elektronik.Common.Data.Packages.SlamActionPackages;
using Elektronik.Common.Data.Converters;
using Elektronik.Common.Data.Packages;
using Elektronik.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elektronik.Common.Data.Parsers.Slam
{
    public class SlamMessageParser : SlamActionPackageParser
    {
        public SlamMessageParser(ICSConverter converter) : base(converter) { }
        public override IPackage Parse(byte[] data, int startIdx, ref int offset)
        {
            Header header = ParseHeader(ref data, startIdx, ref offset);
            if (header.ObjectType == ObjectType.Message)
            {
                int messageSize = BitConverterEx.ToInt32(data, startIdx, ref offset);
                char[] messageData = new char[messageSize];
                for (int i = 0; i < messageSize; ++i)
                {
                    messageData[i] = (char)data[offset++];
                }
                return new ActionDataPackage<char>(
                    header.ObjectType,
                    header.ActionType,
                    header.PackageType,
                    header.Timestamp,
                    header.IsKey,
                    messageData);
            }
            return m_successor?.Parse(data, startIdx, ref offset);
        }
    }
}
